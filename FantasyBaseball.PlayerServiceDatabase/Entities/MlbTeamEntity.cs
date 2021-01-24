using System.Collections.Generic;

namespace FantasyBaseball.PlayerServiceDatabase.Entities
{
    /// <summary>Info for a given MLB team.</summary>
    public class MlbTeamEntity
    {
        /// <summary>The team's main code.</summary>
        public string Code { get; set; }

        /// <summary>The team's alternative code.</summary>
        public string AlternativeCode { get; set; }

        /// <summary>The MLB league a team belongs to (AL or NL).</summary>
        public string MlbLeagueId { get; set; }

        /// <summary>The team's city.</summary>
        public string City { get; set; }

        /// <summary>The team's nickname.</summary>
        public string Nickname { get; set; }

        /// <summary>Collection of player's that belong to this team.</summary>
        public List<PlayerEntity> Players { get; set ;} = new List<PlayerEntity>();
    }
}