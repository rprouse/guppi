using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Application.Exceptions;
using Guppi.Application.Services.Dictionary;
using Guppi.Domain.Entities.Dictionary;
using Guppi.Domain.Interfaces;

namespace Guppi.Application.Services;

internal class DictionaryService : IDictionaryService
{
    private readonly IHttpRestService _restService;

    public DictionaryService(IHttpRestService restService)
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
