using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using Guppi.Application.Services;

namespace Guppi.Console.Skills
{
    public class NotesSkill : ISkill
    {
        private readonly INoteService _service;

        public NotesSkill(INoteService service)
        {
            _service = service;
        }

        public IEnumerable<Command> GetCommands()
        {
            var add = new Command("add", "Adds a new note.")
            {
                new Option<string>( new string[]{"--title", "-t"}, "Sets the note title"),
                //new Option<string>( new string[]{"--folder", "-f"}, () => string.Empty, "Sub-folder for the note"),
                new Option<string>( new string[]{"--vault", "-v"}, () => string.Empty, "Use a vault other than the default")
            };
            add.Handler = CommandHandler.Create((string title, /* string folder, */ string vault) => Add(title, null, vault));

            //var daily = new Command("daily", "Adds a new daily note")
            //{
            //    new Option<string>( new string[]{"--vault", "-v"}, () => string.Empty, "Use a vault other than the default")
            //};
            //daily.Handler = CommandHandler.Create(async (string vault) => Daily(vault));

            var view = new Command("view", "Opens Obsidian")
            {
                new Option<string>( new string[]{"--vault", "-v"}, () => string.Empty, "Use a vault other than the default")
            };
            view.AddAlias("open");
            view.Handler = CommandHandler.Create((string vault) => View(vault));

            var code = new Command("code", "Opens the notes folder in VS Code");
            code.Handler = CommandHandler.Create(() => Code());

            var config = new Command("configure", "Configure the notes skill.");
            config.AddAlias("config");
            config.Handler = CommandHandler.Create(() => Configure());

            var notes = new Command("notes", "Works with Notes in Obsidian")
            {
                add,
                // daily,
                view,
                code,
                config
            };
            return new[] { notes };
        }

        private void Add(string title, string folder, string vault) =>
            _service.AddFile(vault, title);

        private Task Daily(string vault)
        {
            // TODO
            return Task.CompletedTask;
        }

        private void View(string vault) =>
            _service.OpenObsidian(vault, null);

        private void Code() => 
            _service.OpenVsCode();

        private void Configure() =>
            _service.Configure();
    }
}
