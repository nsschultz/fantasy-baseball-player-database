using System.Collections.Generic;
using FantasyBaseball.Common.Models;

namespace FantasyBaseball.PlayerServiceDatabase.Services
{
    /// <summary>Service for sorting the players.</summary>
    public interface ISortService
    {
        /// <summary>Sorts the collection of players.</summary>
        /// <param name="players">All of the players to sort.</param>
        /// <returns>The sorted collection of players.</returns>
        List<BaseballPlayer> SortPlayers(List<BaseballPlayer> players);
    }
}