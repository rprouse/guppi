using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Domain.Entities.Dictionary;

namespace Guppi.Core.Services;

public interface IDictionaryService
{
    void Configure();
    Task<IEnumerable<DictionaryResponse>> LookupDictionaryFor(string word);
    Task<IEnumerable<ThesaurusResponse>> LookupThesaurusFor(string word);
}
