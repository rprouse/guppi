using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Core.Entities.Dictionary;

namespace Guppi.Core.Interfaces;

public interface IDictionaryService
{
    void Configure();
    Task<IEnumerable<DictionaryResponse>> LookupDictionaryFor(string word);
    Task<IEnumerable<ThesaurusResponse>> LookupThesaurusFor(string word);
}
