using System.Collections.Generic;

namespace Guppi.Domain.Entities.Dictionary;

public class ThesaurusResponse
{
    public string Id { get; set; }

    public string PartOfSpeech { get; set; }

    public IList<ThesaurusAlternative> Alternatives { get; } = new List<ThesaurusAlternative>();
}

public class ThesaurusAlternative
{
    public string ShortDefinition { get; set; }

    public IList<string> Synonyms { get; set; }

    public IList<string> Antonyms { get; set; }

    public bool Offensive { get; set; }
}
