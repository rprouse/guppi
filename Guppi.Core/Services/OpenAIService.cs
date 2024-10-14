using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Core.Configurations;
using Guppi.Core.Exceptions;
using Guppi.Core.Interfaces;
using OpenAI;
using OpenAI.Chat;

namespace Guppi.Core.Services;

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

        OpenAIClient client = new (configuration.ApiKey);
        var chat = client.GetChatClient("gpt-4o");

        write("> ");
        string input = readline();
        while (input.ToLowerInvariant() != "exit")
        {
            write(Environment.NewLine);
            AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = chat.CompleteChatStreamingAsync(input);
            await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
            {
                if (completionUpdate.ContentUpdate.Count > 0)
                {
                    write(completionUpdate.ContentUpdate[0].Text);
                }
            }

            write(Environment.NewLine);
            write(Environment.NewLine);
            write("> ");
            input = readline();
        }
    }
}
