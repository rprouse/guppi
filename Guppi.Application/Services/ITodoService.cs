using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Guppi.Application.Exceptions;
using System.IO;
using System.Threading;
using Spectre.Console;
using Google.Apis.Tasks.v1;
using Google.Apis.Services;
using System.Threading.Tasks;
using System.Linq;

using TaskList = Google.Apis.Tasks.v1.Data.TaskList;

namespace Guppi.Application.Services;

public interface ITodoService
{
    Task Sync();
}

public class TodoService : ITodoService
{
    static string[] Scopes = { TasksService.Scope.Tasks };
    static string ApplicationName = "Guppi ActionProvider.Tasks";
    static string TaskListName = "todo.txt";

    public string Name => "Google Tasks";

    TasksService _service;

    public async Task Sync()
    {
        LogIntoGoogle();

        TaskList taskList = await GetTaskList();
        AnsiConsole.WriteLine($"Retrieved task list {taskList.Title}");

        var tasks = await _service.Tasks.List(taskList.Id).ExecuteAsync();
        AnsiConsole.WriteLine($"{tasks.Items.Count} tasks retrieved from Google");
    }

    private void LogIntoGoogle()
    {
        string credentials = Configuration.GetConfigurationFile("task_credentials");
        if (!File.Exists(credentials))
        {
            throw new UnconfiguredException("Please download the credentials. See the Readme.");
        }

        UserCredential credential = null;

        using (var stream = new FileStream(credentials, FileMode.Open, FileAccess.Read))
        {
            string token = Configuration.GetConfigurationFile("task_token");
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(token, true)).Result;
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

    private async Task<TaskList> GetTaskList()
    {
        TasklistsResource.ListRequest listRequest = _service.Tasklists.List();

        var lists = await listRequest.ExecuteAsync();

        var work = lists.Items.FirstOrDefault(x => x.Title == TaskListName);

        // If the list does not exist, create it
        if (work is null)
        {
            TaskList newList = new TaskList { Title = TaskListName };
            work = await _service.Tasklists.Insert(newList).ExecuteAsync();
        }

        if (work is null)
        {
            throw new ErrorException("Failed to fetch or create task list");
        }
        return work;
    }
}
