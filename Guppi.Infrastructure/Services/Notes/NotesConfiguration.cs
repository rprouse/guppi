using Guppi.Application;
using Guppi.Application.Attributes;

namespace Guppi.Infrastructure.Services.Notes
{
    public class NotesConfiguration : Configuration
    {
        [Display("Notes Directory")]
        public string NotesDirectory { get; set; }
    }
}
