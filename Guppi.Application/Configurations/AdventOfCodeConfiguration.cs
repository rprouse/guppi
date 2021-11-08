using Guppi.Application.Attributes;

namespace Guppi.Application.Configurations
{
    class AdventOfCodeConfiguration : Configuration
    {
        string _loginToken;

        [Display("Login Token")]
        public string LoginToken 
        {
            get => _loginToken;
            set
            {
                if(value?.StartsWith("session=", System.StringComparison.OrdinalIgnoreCase) == true)
                {
                    _loginToken = value.Substring(8);
                }
                else
                {
                    _loginToken = value;
                }
            }
        }

        [Display("AoC Solution Directory")]
        public string SolutionDirectory { get; set; } = @"C:\Src\Alteridem\AdventOfCode";
    }
}
