using Guppi.Application;
using Guppi.Application.Attributes;

namespace ActionProvider.Notes
{
    public class NotesConfiguration : Configuration
    {
        [Display("Notes Directory")]
        public string NotesDirectory { get; set; }
    }
}
