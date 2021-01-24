using System;

namespace FantasyBaseball.PlayerServiceDatabase.Entities
{
    /// <summary>Entity for mapping the many to many relationship between players and positions.</summary>
    public class PlayerPositionEntity
    {
        /// <summary>The player's ID.</summary>
        public Guid PlayerId { get; set; }

        /// <summary>The position's code.</summary>
        public string PositionCode { get; set; }

        /// <summary>The player.</summary>
        public PlayerEntity Player { get; set; }

        /// <summary>The position.</summary>
        public PositionEntity Position { get; set; }
    }
}