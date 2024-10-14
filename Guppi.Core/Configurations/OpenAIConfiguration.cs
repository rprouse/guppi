using Guppi.Core.Attributes;

namespace Guppi.Core.Configurations;

public class OpenAIConfiguration : Configuration
{
    [Display("API Key")]
    public string ApiKey { get; set; }
}
