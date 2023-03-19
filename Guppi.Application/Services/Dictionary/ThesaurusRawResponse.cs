namespace Guppi.Application.Services.Dictionary;

#pragma warning disable IDE1006 // Naming Styles
using System.Collections.Generic;

public class Target
{
    public string tuuid { get; set; }
    public string tsrc { get; set; }
}

public class Meta
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

public class ThesaurusRawResponse
{
    public Meta meta { get; set; }
    public Hwi hwi { get; set; }
    public string fl { get; set; }
    public List<Def> def { get; set; }
    public string[] shortdef { get; set; }
}

#pragma warning restore IDE1006 // Naming Styles
