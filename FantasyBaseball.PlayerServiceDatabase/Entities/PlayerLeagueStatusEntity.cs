using System;
using FantasyBaseball.Common.Enums;

namespace FantasyBaseball.PlayerServiceDatabase.Entities
{
    /// <summary>Info for a given Player within a given League.</summary>
    public class PlayerLeagueStatusEntity
    {
        /// <summary>The player's ID.</summary>
        public Guid PlayerId { get; set; }

        /// <summary>The league's ID.</summary>
        public int LeagueId { get; set; }

        /// <summary>The player's status within the given league.</summary>
        public LeagueStatus LeagueStatus { get; set; }

        /// <summary>All of the information that makes up a baseball player.</summary>
        public PlayerEntity Player { get; set; }
    }
}