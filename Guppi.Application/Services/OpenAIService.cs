using System;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Application.Exceptions;
using OpenAI_API;

namespace Guppi.Application.Services;

internal class OpenAIService : IOpenAIService
{
    public void Configure()
    {
        var configuration = Configuration.Load<OpenAIConfiguration>("openai");
        configuration.RunConfiguration("OpenAI", "Enter the OpenAI API key");
    }

    public async Task Chat(Func<string> readline, Action<string> write)
    {
        var configuration = Configuration.Load<OpenAIConfiguration>("openai");
        if (!configuration.Configured)
        {
            throw new UnconfiguredException("Please configure the OpenAI provider");
        }

        OpenAIAPI api = new OpenAIAPI(configuration.ApiKey);
        var chat = api.Chat.CreateConversation();

        write("> ");
        string input = readline();
        while (input.ToLowerInvariant() != "exit")
        {
            chat.AppendUserInput(input);

            write(Environment.NewLine);
            await foreach (var res in chat.StreamResponseEnumerableFromChatbotAsync())
            {
                write(res);
            }

            write(Environment.NewLine);
            write(Environment.NewLine);
            write("> ");
            input = readline();
        }
    }
}
