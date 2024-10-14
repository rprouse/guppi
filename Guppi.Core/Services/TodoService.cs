using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alteridem.Todo.Application.Commands;
using Alteridem.Todo.Application.Queries;
using Alteridem.Todo.Domain.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using Google.Apis.Util.Store;
using Guppi.Core.Exceptions;
using Guppi.Core.Extensions;
using MediatR;
using Spectre.Console;
using GoogleTask = Google.Apis.Tasks.v1.Data.Task;
using GoogleTaskList = Google.Apis.Tasks.v1.Data.TaskList;

namespace Guppi.Core.Services;

public class TodoService(IMediator mediator, ITaskConfiguration configuration) : ITodoService
{
    static string[] Scopes = { TasksService.Scope.Tasks };
    static string ApplicationName = "Guppi ActionProvider.Tasks";

    const string TaskListName = "todo.txt";
    const string IdTag = "id";
    const string UpdatedTag = "updated";

    public static string Name => "Google Tasks";

    TasksService _service;
    private IMediator Mediator { get; } = mediator;
    private ITaskConfiguration Configuration { get; } = configuration;

    public async Task Sync()
    {
        await LogIntoGoogle();

        GoogleTaskList taskList = await GetTaskListFromGoogle();
        AnsiConsole.WriteLine($"Retrieved task list {taskList.Title}");

        List<GoogleTask> googleTasks = new ();
        string nextPageToken = null;
        do
        {
            var request = _service.Tasks.List(taskList.Id);
            request.MaxResults = 100;
            request.PageToken = nextPageToken;
            request.ShowCompleted = false;
            var tasks = await request.ExecuteAsync();
            if (tasks.Items is not null)
            {
                googleTasks.AddRange(tasks.Items);
            }
            nextPageToken = tasks.NextPageToken;
        } while (!string.IsNullOrEmpty(nextPageToken));
        AnsiConsole.WriteLine($"{googleTasks.Count} tasks retrieved from Google");

        var todo = (await GetTaskListFromTodoTxt(Configuration.TodoFile)).Tasks;
        var done = (await GetTaskListFromTodoTxt(Configuration.DoneFile)).Tasks;
        AnsiConsole.WriteLine($"{todo.Count} tasks retrieved from todo.txt");

        // Local tasks without an ID are new and need to be added to Google
        var notSynced = todo.Where(t => !t.SpecialTags.ContainsKey(IdTag)).ToList();
        foreach (var task in notSynced)
        {
            // Update on Google
            var newTask = new Google.Apis.Tasks.v1.Data.Task { Title = task.Text };
            var result = await _service.Tasks.Insert(newTask, taskList.Id).ExecuteAsync();
            task.Description += $" {IdTag}:{result.Id} {UpdatedTag}:{result.Updated}";
            AnsiConsole.WriteLine($"Added task {task.LineNumber} to Google");

            // Update locally
            var replace = new ReplaceCommand { ItemNumber = task.LineNumber, Text = task.ToString() };
            await Mediator.Send(replace);
        }

        // Tasks in Google that are in the done list need to be deleted from Google
        var doneLocally = done.Where(t => t.SpecialTags.ContainsKey(IdTag)).ToList();
        foreach (var task in doneLocally.Where(t => googleTasks.Any(g => g.Id == t.SpecialTags[IdTag])))
        {
            var gTask = googleTasks.First(g => g.Id == task.SpecialTags[IdTag]);
            await _service.Tasks.Delete(taskList.Id, gTask.Id).ExecuteAsync();
            AnsiConsole.WriteLine($"Deleted task {task.SpecialTags[IdTag]} from Google");
            googleTasks.Remove(gTask);
        }

        // Tasks in Google that are not in the local list need to be added to the local list
        var notLocal = googleTasks
            .Where(t => string.IsNullOrEmpty(t.Completed))
            .Where(t => !todo.Any(l => l.SpecialTags.ContainsKey(IdTag) && l.SpecialTags[IdTag] == t.Id))
            .ToList();
        foreach (var task in notLocal)
        {
            string taskStr = $"{task.Updated.GetRfc3339Date().ToString("yyyy-MM-dd")} {task.Title}";
            if (!string.IsNullOrEmpty(task.Due))
            {
                taskStr += $" due:{task.Due}";
            }
            taskStr += $" {IdTag}:{task.Id} {UpdatedTag}:{task.Updated}";
            var append = new AddTaskCommand { Filename = Configuration.TodoFile, Task = taskStr, AddCreationDate = false };
            await Mediator.Send(append);
            AnsiConsole.WriteLine($"Added task {task.Id} to todo.txt");
        }

        // Local tasks that are newer than the Google tasks need to be updated in Google

        // Google tasks that are newer than the local tasks need to be updated in the local tasks
    }

    private async Task LogIntoGoogle()
    {
        string credentials = Core.Configuration.GetConfigurationFile("task_credentials");
        if (!File.Exists(credentials))
        {
            throw new UnconfiguredException("Please download the credentials. See the Readme.");
        }

        UserCredential credential = null;

        using (var stream = new FileStream(credentials, FileMode.Open, FileAccess.Read))
        {
            string token = Core.Configuration.GetConfigurationFile("task_token");
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                (await GoogleClientSecrets.FromStreamAsync(stream)).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(token, true));
        }

        if (credential is null)
        {
            throw new UnauthorizedException("Failed to login to Google Tasks");
        }

        _service = new TasksService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    private async Task<GoogleTaskList> GetTaskListFromGoogle()
    {
        TasklistsResource.ListRequest listRequest = _service.Tasklists.List();

        var lists = await listRequest.ExecuteAsync();

        var work = lists.Items?.FirstOrDefault(x => x.Title == TaskListName);

        // If the list does not exist, create it
        if (work is null)
        {
            GoogleTaskList newList = new GoogleTaskList { Title = TaskListName };
            work = await _service.Tasklists.Insert(newList).ExecuteAsync();
        }

        if (work is null)
        {
            throw new ErrorException("Failed to fetch or create task list");
        }
        return work;
    }

    private async Task<ListTasksResponse> GetTaskListFromTodoTxt(string filename)
    {
        var query = new ListTasksQuery { Filename = filename, Terms = [] };
        return await Mediator.Send(query);
    }
}
