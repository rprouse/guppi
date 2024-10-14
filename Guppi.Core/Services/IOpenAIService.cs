using System;
using System.Threading.Tasks;

namespace Guppi.Core.Services;

public interface IOpenAIService
{
    void Configure();
    Task Chat(Func<string> readline, Action<string> write);
}
