using System;
using FantasyBaseball.CommonModels.Enums;

namespace FantasyBaseball.PlayerServiceDatabase.Entities
{
    /// <summary>A single type of pitching stats for a player.</summary>
    public class PitchingStatsEntity
    {
        /// <summary>The player's ID.</summary>
        public Guid PlayerId { get; set; }

        /// <summary>The type of stats. Year to Date (YTD) or Projected (PROJ). </summary>
        public StatsType StatsType { get; set; }

        /// <summary>Wins (W)</summary>
        public int Wins { get; set; }

        /// <summary>Losses (L)</summary>
        public int Losses { get; set; }

        /// <summary>Quality Starts (QS)</summary>
        public int QualityStarts { get; set; }

        /// <summary>Saves (SV)</summary>
        public int Saves { get; set; }

        /// <summary>Blown Saves (BS)</summary>
        public int BlownSaves { get; set; }

        /// <summary>Holds (HLD)</summary>
        public int Holds { get; set; }

        /// <summary>Innings Pitched (IP)</summary>
        public double InningsPitched { get; set; }

        /// <summary>Hits Allowed (H)</summary>
        public int HitsAllowed { get; set; }

        /// <summary>Earned Runs (ER)</summary>
        public int EarnedRuns { get; set; }

        /// <summary>Home Runs Allowed (HR)</summary>
        public int HomeRunsAllowed { get; set; }

        /// <summary>Base on Balls Allowed (BB)</summary>
        public int BaseOnBallsAllowed { get; set; }

        /// <summary>Strike Outs (K)</summary>
        public int StrikeOuts { get; set; }

        /// <summary>Fly Ball Rate (F%)</summary>
        public double FlyBallRate { get; set; }

        /// <summary>Ground Ball Rate (G%)</summary>
        public double GroundBallRate { get; set; }

        /// <summary>All of the information that makes up a baseball player.</summary>
        public PlayerEntity Player { get; set; }
    }
}