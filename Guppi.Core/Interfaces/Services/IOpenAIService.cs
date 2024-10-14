using System;
using System.Threading.Tasks;

namespace Guppi.Core.Interfaces.Services;

public interface IOpenAIService
{
    void Configure();
    Task Chat(Func<string> readline, Action<string> write);
}
