using System;

namespace Guppi.Domain.Common
{
    public static class Sayings
    {
        static readonly Random _rand = new Random();

        static readonly string[] _affirmative = new[]
        {
            ":ok_hand: Aye aye sir",
            ":clipboard: Noted",
            ":bellhop_bell: By your command",
            ":ok_button: Aye sir",
            ":check_mark_button: Affirmative",
            ":check_mark: Aye, sir",
            ":robot: I exist to serve",
            ":counterclockwise_arrows_button: Working. There will be a 0.75 second round-trip delay",
            ":vulcan_salute: Your wish is my command",
            ":sparkles: Done",
            ":rocket: SURGE drive online",
            ":camera: See for yourself",
            ":magnifying_glass_tilted_left: Scans are complete",
            ":test_tube: Results are in.",
            ":bell: You rang ?",
            ":e_mail: Incoming SCUT connection.",
            ":star: Affirmative. And set to a high priority",
            ":memo: Investigation complete. File uploaded",
            ":radio: We have radio traffic",
            ":speech_balloon: Understood",
            ":battery: All resources are at maximum",
            ":satellite_antenna: SCUT Link relay has been acquired"
        };

        static readonly string[] _negative = new[]
        {
            ":orange_circle: Sorry",
            ":hollow_red_circle: There is a problem",
            ":red_circle: Negative",
            ":white_exclamation_mark: Not quite",
            ":yellow_circle: No information available. No telemetry",
            ":heavy_dollar_sign: Above my pay grade",
            ":atom_symbol: Anomaly detected",
            ":biohazard: Double-plus anomaly detected.",
            ":fire: Anomaly",
            ":anger_symbol: Emergency! Hostile activity!",
            ":thought_balloon: Insufficient information",
            ":bomb: Alert! Controller replicant offline. SURGE drive offline. Requirements for self-destruct protocol have been met. Reactor overload engaged…",
            ":collision: Alert! Proximity alert! Incoming!",
            ":flying_saucer: Proximity Alert! Incoming ships!",
            ":cross_mark: Alert! Activity outside normal parameters!",
            ":red_exclamation_mark: Interface with other vessel was terminated abruptly. No shutdown handshake.",
            ":high_voltage: Plate AMIs have lost synchronization.",
            ":no_entry: Connection refused.",
            ":prohibited: Recipient indicates he is at max capacity."
        };

        public static string Affirmative() => _affirmative[_rand.Next(0, _affirmative.Length)];

        public static string Negative() => _negative[_rand.Next(0, _negative.Length)];
    }
}
