using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using Guppi.Core;
using Guppi.Core.Configurations;
using Guppi.Core.Entities.Voices;
using Guppi.Core.Interfaces.Providers;

namespace Guppi.Core.Providers;

public class SpeechProvider : ISpeechProvider
{
    readonly List<Prompt> _prompts;
    readonly SpeechSynthesizer _synth;
    readonly VoiceConfiguration _config;

    public SpeechProvider()
    {
        if (OperatingSystem.IsWindows())
        {
            _config = Configuration.Load<VoiceConfiguration>("voice");
            _prompts = [];
            _synth = new SpeechSynthesizer();
            
            if (!string.IsNullOrWhiteSpace(_config?.Voice) && IsVoiceInstalled(_config.Voice))
                _synth.SelectVoice(_config.Voice);
            else
                _synth.SelectVoiceByHints(VoiceGender.Male);
            
            _synth.SetOutputToDefaultAudioDevice();
        }
    }

#pragma warning disable CA1416 // Validate platform compatibility
    bool IsVoiceInstalled(string name)
    {
        if (!OperatingSystem.IsWindows())
            return false;

        return _synth.GetInstalledVoices().Any(v => v.Enabled && v.VoiceInfo.Name == name);
    }
    
    public IEnumerable<Voice> ListVoices()
    {
        if (OperatingSystem.IsWindows())
        {
            return _synth
                .GetInstalledVoices()
                .Where(v => v.Enabled)
                .Select(v => new Voice
            {
                Id = v.VoiceInfo.Id,
                Name = v.VoiceInfo.Name,
                Description = v.VoiceInfo.Description,
            });
        }
        return Enumerable.Empty<Voice>();
    }

    public void SetVoice(Voice voice)
    {
        _config.Voice = voice.Name;
        _config.Save();
    }
#pragma warning restore CA1416 // Validate platform compatibility

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
            while (_prompts.Exists(p => !p.IsCompleted))
            {
                await Task.Delay(50);
            }
#pragma warning restore CA1416 // Validate platform compatibility
        }
    }
}
