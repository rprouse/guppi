using Guppi.Core.Attributes;

namespace Guppi.Core.Configurations
{
    public class NotesConfiguration : Configuration
    {
        [Display("Notes Directory")]
        public string NotesDirectory { get; set; }

        [Display("Obsidian Vault")]
        public string VaultName { get; set; } = "Notes";
    }
}
