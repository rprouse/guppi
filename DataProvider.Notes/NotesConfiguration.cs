using Guppi.Core;
using Guppi.Core.Attributes;

namespace DataProvider.Notes
{
    public class NotesConfiguration : Configuration
    {
        [Display("Notes Directory")]
        public string NotesDirectory { get; set; }
    }
}
