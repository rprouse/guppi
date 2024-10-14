using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Core.Entities.Voices;

namespace Guppi.Core.Interfaces
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
