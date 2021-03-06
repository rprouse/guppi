using Guppi.Application.Attributes;

namespace Guppi.Application.Configurations
{
    class AdventOfCodeConfiguration : Configuration
    {
        [Display("Login Token")]
        public string LoginToken { get; set; }

        [Display("AoC Solution Directory")]
        public string SolutionDirectory { get; set; } = @"C:\Src\Alteridem\AdventOfCode";
    }
}
