using System;

namespace Guppi.Core
{
    public class Sayings
    {
        static readonly Random _rand = new Random();

        static readonly string[] _affirmative = new[]
        {
            "[Aye aye sir]",
            "[Noted]",
            "[By your command]",
            "[Aye sir]",
            "[Affirmative]",
            "[Aye, sir]",
            "[I exist to serve]",
            "[Working. There will be a 0.75 second round-trip delay]",
            "[Your wish is my command]",
            "[Done]",
            "[SURGE drive online]",
            "[See for yourself]",
            "[Scans are complete]",
            "[Results are in.]",
            "[You rang ?]",
            "[Incoming SCUT connection.]",
            "[Affirmative. And set to a high priority]",
            "[Investigation complete. File uploaded]",
            "[We have radio traffic]",
            "[Understood]",
            "[All resources are at maximum]",
            "[SCUT Link relay has been acquired]"
        };

        static readonly string[] _negative = new[]
        {
            "[Sorry]",
            "[There is a problem]",
            "[Negative]",
            "[Not quite]",
            "[No information available. No telemetry]",
            "[Above my pay grade]",
            "[Anomaly detected]",
            "[Double-plus anomaly detected.]",
            "[Anomaly]",
            "[Emergency! Hostile activity!]",
            "[Insufficient information]",
            "[Alert! Controller replicant offline. SURGE drive offline. Requirements for self-destruct protocol have been met. Reactor overload engaged…]",
            "[Alert! Proximity alert! Incoming!]",
            "[Proximity Alert! Incoming ships!]",
            "[Alert! Activity outside normal parameters!]",
            "[Interface with other vessel was terminated abruptly. No shutdown handshake.]",
            "[Plate AMIs have lost synchronization.]",
            "[Connection refused.]",
            "[Recipient indicates he is at max capacity.]"
        };

        public static string Affirmative() => _affirmative[_rand.Next(0, _affirmative.Length)];

        public static string Negative() => _negative[_rand.Next(0, _negative.Length)];
    }
}
