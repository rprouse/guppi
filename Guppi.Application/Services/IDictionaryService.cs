using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Domain.Interfaces;

namespace Guppi.Application.Services;

public interface IDictionaryService
{
    void Configure();
}

internal class DictionaryService : IDictionaryService
{
    public void Configure()
    {
        var configuration = Configuration.Load<DictionaryConfiguration>("Dictionary");
        configuration.RunConfiguration("Dictionary", "Configure the Dictionary and Thesaurus API keys.");
    }
}
