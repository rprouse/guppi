using Guppi.Core;
using Guppi.Core.Attributes;

namespace DataProvider.AdventOfCode
{
    class AdventOfCodeConfiguration : Configuration
    {
        [Display("Login Token")]
        public string LoginToken { get; set; }
    }
}
