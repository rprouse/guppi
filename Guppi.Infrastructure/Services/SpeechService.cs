using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;

namespace Guppi.Infrastructure.Services
{
    public class SpeechService : ISpeechService
    {
        IList<Prompt> _prompts;
        SpeechSynthesizer _synth;

        public SpeechService()
        {
            if (OperatingSystem.IsWindows())
            {
                _prompts = new List<Prompt>();
                _synth = new SpeechSynthesizer();
                _synth.SelectVoiceByHints(VoiceGender.Male);
                _synth.SetOutputToDefaultAudioDevice();
            }
        }

        public void Speak(string textToSpeak)
        {
            if (OperatingSystem.IsWindows())
            {
                var prompt = _synth.SpeakAsync(textToSpeak);
                _prompts.Add(prompt);
            }
        }

        public void SpeakSsml(string textToSpeak)
        {
            if (OperatingSystem.IsWindows())
            {
                var prompt = _synth.SpeakSsmlAsync(textToSpeak);
                _prompts.Add(prompt);
            }
        }

        public async Task Wait()
        {
            if (OperatingSystem.IsWindows())
            {
#pragma warning disable CA1416 // Validate platform compatibility
                while (_prompts.Any(p => !p.IsCompleted))
                {
                    await Task.Delay(50);
                }
#pragma warning restore CA1416 // Validate platform compatibility
            }
        }
    }
}
