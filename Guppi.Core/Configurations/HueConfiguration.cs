using Guppi.Core.Attributes;

namespace Guppi.Core.Configurations
{
    public class HueConfiguration : Configuration
    {
        uint _defaultLight = 0;

        [Display("Default Light")]
        public string DefaultLight
        {
            get => _defaultLight.ToString();
            set
            {
                _defaultLight = 0;
                if (string.IsNullOrWhiteSpace(value))
                    return;

                uint.TryParse(value, out _defaultLight);
            }
        }

        public uint GetDefaultLight() => _defaultLight;
    }
}
