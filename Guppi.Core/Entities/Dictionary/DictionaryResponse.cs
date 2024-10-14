using System.Collections.Generic;

namespace Guppi.Core.Entities.Dictionary;

public class DictionaryResponse
{
    public string Id { get; set; }

    public string PartOfSpeech { get; set; }

    public IList<DictionaryAlternative> Alternatives { get; } = new List<DictionaryAlternative>();
}

public class DictionaryAlternative
{
    public string ShortDefinition { get; set; }

    public bool Offensive { get; set; }
}
