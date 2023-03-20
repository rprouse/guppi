using System.Collections.Generic;
using Guppi.Domain.Entities.Dictionary;

namespace Guppi.Application.Services.Dictionary;

#pragma warning disable IDE1006 // Naming Styles
public class ThesaurusRawResponse
{
    public ThesaurusMeta meta { get; set; }
    public Hwi hwi { get; set; }
    public string fl { get; set; }
    public List<Def> def { get; set; }
    public string[] shortdef { get; set; }

    /// <summary>
    /// Converts a raw JSON response to a clean internal format
    /// </summary>
    /// <returns></returns>
    public ThesaurusResponse GetThesaurusResponse()
    {
        var response = new ThesaurusResponse
        {
            Id = meta.id,
            PartOfSpeech = fl
        };
        for (int i = 0; i < shortdef.Length; i++)
        {
            response.Alternatives.Add(new ThesaurusAlternative
            {
                ShortDefinition = shortdef[i],
                Offensive = meta.offensive,
                Synonyms = i < meta.syns.Length ? meta.syns[i] : new List<string>(),
                Antonyms = i < meta.ants.Length ? meta.ants[i] : new List<string>()
            });
        }
        return response;
    }
}


public class Target
{
    public string tuuid { get; set; }
    public string tsrc { get; set; }
}

public class ThesaurusMeta
{
    public string id { get; set; }
    public string uuid { get; set; }
    public string src { get; set; }
    public string section { get; set; }
    public Target target { get; set; }
    public List<string> stems { get; set; }
    public List<string>[] syns { get; set; }
    public List<string>[] ants { get; set; }
    public bool offensive { get; set; }
}

public class Hwi
{
    public string hw { get; set; }
}

public class Text
{
    public string text { get; set; }
}

public class Vis
{
    public string t { get; set; }
}

public class Sense
{
    public List<object> dt { get; set; }
    public List<Dictionary<string, string>> syn_list { get; set; }
    public List<Dictionary<string, string>> rel_list { get; set; }
    public List<Dictionary<string, string>> near_list { get; set; }
    public List<Dictionary<string, string>> ant_list { get; set; }
}

public class Def
{
    public List<List<object>> sseq { get; set; }
}

#pragma warning restore IDE1006 // Naming Styles
