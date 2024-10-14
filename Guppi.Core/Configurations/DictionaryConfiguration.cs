using Guppi.Core.Attributes;

namespace Guppi.Core.Configurations;

public class DictionaryConfiguration : Configuration
{
    [Display("Dictionary API Key")]
    public string DictionaryApiKey { get; set; }

    [Display("Thesaurus API Key")]
    public string ThesaurusApiKey { get; set; }
}
