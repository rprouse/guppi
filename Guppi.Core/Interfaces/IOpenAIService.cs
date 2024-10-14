using System;
using System.Threading.Tasks;

namespace Guppi.Core.Interfaces;

public interface IOpenAIService
{
    void Configure();
    Task Chat(Func<string> readline, Action<string> write);
}
