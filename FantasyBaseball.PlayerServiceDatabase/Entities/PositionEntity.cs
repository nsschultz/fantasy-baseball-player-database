using System.Collections.Generic;
using FantasyBaseball.Common.Enums;

namespace FantasyBaseball.PlayerServiceDatabase.Entities
{
    /// <summary>Info for a given position.</summary>
    public class PositionEntity
    {
        /// <summary>The position's code.</summary>
        public string Code { get; set; }

        /// <summary>The full name of the position.</summary>
        public string FullName { get; set; }

        /// <summary>The type of player (batter or pitcher) that plays this position.</summary>
        public PlayerType PlayerType { get; set; }

        /// <summary>The order this position should be sorted in.</summary>
        public int SortOrder { get; set; }

        /// <summary>Collection of players that play this position.</summary>
        public List<PlayerPositionEntity> Players { get; set; } = new List<PlayerPositionEntity>();
    }
}