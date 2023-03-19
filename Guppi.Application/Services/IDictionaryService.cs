using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Application.Exceptions;
using Guppi.Application.Services.Dictionary;
using Guppi.Domain.Entities.Dictionary;
using Guppi.Domain.Interfaces;

namespace Guppi.Application.Services;

public interface IDictionaryService
{
    void Configure();
    Task<IEnumerable<ThesaurusResponse>> LookupThesaurusFor(string word);
}

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

    public async Task<IEnumerable<ThesaurusResponse>> LookupThesaurusFor(string word)
    {
        var configuration = Configuration.Load<DictionaryConfiguration>("Dictionary");
        if (!configuration.Configured)
        {
            throw new UnconfiguredException("Please configure the Dictionary provider.");
        }

        var url = $@"https://www.dictionaryapi.com/api/v3/references/thesaurus/json/{word}?key={configuration.ThesaurusApiKey}";
        var result = await _restService.GetData<List<ThesaurusRawResponse>>(url);
        var responses = new List<ThesaurusResponse>();
        foreach (var response in result)
        {
            var res = new ThesaurusResponse
            {
                Id = response.meta.id,
                PartOfSpeech = response.fl
            };
            for (int i = 0; i < response.shortdef.Length; i++)
            {
                res.Alternatives.Add(new Alternative
                {
                    ShortDefinition = response.shortdef[i],
                    Offensive = response.meta.offensive,
                    Synonyms = response.meta.syns[i],
                    Antonyms = response.meta.ants[i]
                });
            }
            responses.Add(res);
        }
        return responses;
    }
}
