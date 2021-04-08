using System.Threading.Tasks;

namespace Guppi.Domain.Interfaces
{
    public interface ISpeechService
    {
        void Speak(string textToSpeak);

        void SpeakSsml(string textToSpeak);

        Task Wait();
    }
}
