using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using Guppi.Application.Extensions;
using Guppi.Application.Services;

namespace Guppi.Console.Skills;

internal class OpenAISkill : ISkill
{
    IOpenAIService _service;

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

        return new[]
        {
            new Command("openai", "Chat with Guppi")
            {
                chat,
                configure
            }
        };
    }

    private async Task Chat()
    {
        AnsiConsoleHelper.TitleRule(":alien_monster: How can I help you today? Type 'exit' to quit.");
        await _service.Chat(System.Console.ReadLine, System.Console.Write);
    }
}
