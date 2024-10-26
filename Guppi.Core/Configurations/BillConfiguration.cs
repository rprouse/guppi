using Guppi.Core.Attributes;

namespace Guppi.Core.Configurations;

public class BillConfiguration : Configuration
{
    [Display("Alectra Username")]
    public string AlectraUsername { get; set; }

    [Display("Alectra Password")]
    public string AlectraPassword { get; set; }

    [Display("Enbridge Username")]
    public string EnbridgeUsername { get; set; }

    [Display("Enbridge Password")]
    public string EnbridgePassword { get; set; }
}
