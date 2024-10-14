using Guppi.Core.Attributes;

namespace Guppi.Core.Configurations
{
    internal class AdventOfCodeConfiguration : Configuration
    {
        string _loginToken;

        [Display("Login Token")]
        public string LoginToken 
        {
            get => _loginToken;
            set
            {
                _loginToken = value?.StartsWith("session=", System.StringComparison.OrdinalIgnoreCase) == true ?
                    value.Substring(8) : value;
            }
        }

        [Display("AoC Solution Directory")]
        public string SolutionDirectory { get; set; } = @"C:\Src\Alteridem\AdventOfCode";
    }
}
