using System;
using System.Threading.Tasks;

namespace Guppi.Application.Services;

public interface IOpenAIService
{
    void Configure();
    Task Chat(Func<string> readline, Action<string> write);
}
