using System.Collections.Generic;
using Spectre.Console;

namespace Guppi.Infrastructure.Services.Strava
{
    internal static class FitnessIcon
    {
        internal static Dictionary<string, string> Icons { get; } = new Dictionary<string, string>
        {
            { "AlpineSki", Emoji.Known.Skier },
            { "BackcountrySki", Emoji.Known.Skier },
            { "Canoeing", Emoji.Known.Canoe },
            { "Crossfit", Emoji.Known.PersonLiftingWeights },
            { "EBikeRide", Emoji.Known.Bicycle },
            { "Elliptical", Emoji.Known.PersonLiftingWeights },
            { "Golf", Emoji.Known.PersonGolfing },
            { "Handcycle", Emoji.Known.Bicycle },
            { "Hike", Emoji.Known.HikingBoot },
            { "IceSkate", Emoji.Known.IceSkate },
            { "InlineSkate", Emoji.Known.RollerSkate },
            { "Kayaking", Emoji.Known.Canoe },
            { "Kitesurf", Emoji.Known.Kite },
            { "NordicSki", Emoji.Known.Skis },
            { "Ride", Emoji.Known.Bicycle },
            { "RockClimbing", Emoji.Known.PersonClimbing },
            { "RollerSki", Emoji.Known.RollerSkate },
            { "Rowing", Emoji.Known.PersonRowingBoat },
            { "Run", Emoji.Known.RunningShoe },
            { "Sail", Emoji.Known.Sailboat },
            { "Skateboard", Emoji.Known .Skateboard},
            { "Snowboard", Emoji.Known.Snowboarder },
            { "Snowshoe", Emoji.Known.Snowflake },
            { "Soccer", Emoji.Known.SoccerBall },
            { "StairStepper", Emoji.Known.PersonLiftingWeights },
            { "StandUpPaddling", Emoji.Known.PersonSurfing },
            { "Surfing", Emoji.Known.PersonSurfing },
            { "Swim", Emoji.Known.PersonSwimming },
            { "Velomobile", Emoji.Known.Bicycle },
            { "VirtualRide", Emoji.Known.Bicycle },
            { "VirtualRun", Emoji.Known.RunningShoe },
            { "Walk", Emoji.Known.PersonWalking },
            { "WeightTraining", Emoji.Known.PersonLiftingWeights },
            { "Wheelchair", Emoji.Known.WheelchairSymbol },
            { "Windsurf", Emoji.Known.PersonSurfing },
            { "Workout", Emoji.Known.PersonLiftingWeights },
            { "Yoga", Emoji.Known.PersonInLotusPosition },
            { "", "" }  // Unknown
        };
    }
}
