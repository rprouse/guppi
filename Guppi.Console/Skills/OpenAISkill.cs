using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using Guppi.Core.Extensions;
using Guppi.Core.Interfaces.Services;

namespace Guppi.Console.Skills;

internal class OpenAISkill : ISkill
{
    readonly IOpenAIService _service;

    public OpenAISkill(IOpenAIService service)
    {
        _service = service;
    }

    public IEnumerable<Command> GetCommands()
    {
        var chat = new Command("chat", "Chat with Guppi");
        chat.Handler = CommandHandler.Create(async () => await Chat());

        var configure = new Command("configure", "Configures the OpenAI provider");
        configure.AddAlias("config");
        configure.Handler = CommandHandler.Create(() => _service.Configure());

        var openai = new Command("openai", "Chat with Guppi")
        {
            chat,
            configure
        };
        openai.AddAlias("ai");

        return new[] { openai };
    }

    private async Task Chat()
    {
        AnsiConsoleHelper.TitleRule(":alien_monster: How can I help you today? Type 'exit' to quit.");
        await _service.Chat(System.Console.ReadLine, System.Console.Write);
    }
}
