using Guppi.Core.Entities.Dictionary;

namespace Guppi.Core.Services.Dictionary;

#pragma warning disable IDE1006 // Naming Styles
internal class DictionaryRawResponse
{
    public DictionaryMeta meta { get; set; }
    public DictionaryHwi hwi { get; set; }
    public string fl { get; set; }
    public DictionaryDef[] def { get; set; }
    public DictionaryUro[] uros { get; set; }
    public string[][] et { get; set; }
    public string date { get; set; }
    public string[] shortdef { get; set; }

    /// <summary>
    /// Converts a raw JSON response to a clean internal format
    /// </summary>
    /// <returns></returns>
    public DictionaryResponse GetDictionaryResponse()
    {
        var response = new DictionaryResponse
        {
            Id = meta.id,
            PartOfSpeech = fl
        };
        for (int i = 0; i < shortdef.Length; i++)
        {
            response.Alternatives.Add(new DictionaryAlternative
            {
                ShortDefinition = shortdef[i],
                Offensive = meta.offensive,
            });
        }
        return response;
    }
}

public class DictionaryMeta
{
    public string id { get; set; }
    public string uuid { get; set; }
    public string sort { get; set; }
    public string src { get; set; }
    public string section { get; set; }
    public string[] stems { get; set; }
    public bool offensive { get; set; }
}

public class DictionaryHwi
{
    public string hw { get; set; }
    public DictionaryPr[] prs { get; set; }
}

public class DictionaryPr
{
    public string mw { get; set; }
    public Sound sound { get; set; }
}

public class Sound
{
    public string audio { get; set; }
    public string _ref { get; set; }
    public string stat { get; set; }
}

public class DictionaryDef
{
    public object[][][] sseq { get; set; }
}

public class DictionaryUro
{
    public string ure { get; set; }
    public string fl { get; set; }
}

#pragma warning restore IDE1006 // Naming Styles
