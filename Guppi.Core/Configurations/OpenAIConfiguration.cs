using Guppi.Application.Attributes;

namespace Guppi.Application.Configurations;

public class OpenAIConfiguration : Configuration
{
    [Display("API Key")]
    public string ApiKey { get; set; }
}
