using System.Collections.Generic;
using System.Threading.Tasks;
using FantasyBaseball.Common.Models;

namespace FantasyBaseball.PlayerServiceDatabase.Services
{
    /// <summary>Service for upsert players.</summary>
    public interface IUpsertPlayersService
    {        
        /// <summary>Gets the players from the underlying source.</summary>
        /// <param name="players">All of the players to upsert into the source.</param>
        Task UpsertPlayers(List<BaseballPlayer> players);
    }
}