using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Domain.Entities.Voices;

namespace Guppi.Domain.Interfaces
{
    public interface ISpeechService
    {
        void Speak(string textToSpeak);

        void SpeakSsml(string textToSpeak);

        IEnumerable<Voice> ListVoices();

        void SetVoice(Voice voice);

        Task Wait();
    }
}
