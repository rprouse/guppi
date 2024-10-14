using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Core.Configurations;
using Guppi.Core.Exceptions;
using Guppi.Core.Services.Dictionary;
using Guppi.Core.Entities.Dictionary;
using Guppi.Core.Interfaces;

namespace Guppi.Core.Services;

internal class DictionaryService : IDictionaryService
{
    private readonly IHttpRestProvider _restService;

    public DictionaryService(IHttpRestProvider restService)
    {
        _restService = restService;
    }

    public void Configure()
    {
        var configuration = Configuration.Load<DictionaryConfiguration>("Dictionary");
        configuration.RunConfiguration("Dictionary", "Configure the Dictionary and Thesaurus API keys.");
    }

    public async Task<IEnumerable<DictionaryResponse>> LookupDictionaryFor(string word)
    {
        var configuration = Configuration.Load<DictionaryConfiguration>("Dictionary");
        if (!configuration.Configured)
        {
            throw new UnconfiguredException("Please configure the Dictionary provider.");
        }

        var url = $@"https://www.dictionaryapi.com/api/v3/references/collegiate/json/{word}?key={configuration.DictionaryApiKey}";
        var result = await _restService.GetData<List<DictionaryRawResponse>>(url);
        return result.Select(r => r.GetDictionaryResponse());
    }

    public async Task<IEnumerable<ThesaurusResponse>> LookupThesaurusFor(string word)
    {
        var configuration = Configuration.Load<DictionaryConfiguration>("Dictionary");
        if (!configuration.Configured)
        {
            throw new UnconfiguredException("Please configure the Dictionary provider.");
        }

        var url = $@"https://www.dictionaryapi.com/api/v3/references/thesaurus/json/{word}?key={configuration.ThesaurusApiKey}";
        var result = await _restService.GetData<List<ThesaurusRawResponse>>(url);
        return result.Select(r => r.GetThesaurusResponse());
    }
}
