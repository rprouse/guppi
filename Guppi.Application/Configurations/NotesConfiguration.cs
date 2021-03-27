using Guppi.Application.Attributes;

namespace Guppi.Application.Configurations
{
    public class NotesConfiguration : Configuration
    {
        [Display("Notes Directory")]
        public string NotesDirectory { get; set; }
    }
}
