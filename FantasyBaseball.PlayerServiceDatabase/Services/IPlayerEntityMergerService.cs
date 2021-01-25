using System.Collections.Generic;
using FantasyBaseball.CommonModels.Player;
using FantasyBaseball.PlayerServiceDatabase.Entities;

namespace FantasyBaseball.PlayerServiceDatabase.Services
{
    /// <summary>Service for converting a BaseballPlayer to a PlayerEntity.</summary>
    public interface IPlayerEntityMergerService
    {        
        /// <summary>Merges a BaseballPlayer into a PlayerEntity.</summary>
        /// <param name="incoming">The incoming player values.</param>
        /// <param name="existing">The existing player values.</param>
        /// <param name="positions">The collection of all of the available positions.</param>
        /// <returns>An object that can be saved to the database.</returns>
        PlayerEntity MergePlayerEntity(BaseballPlayer incoming, PlayerEntity existing, List<PositionEntity> positions);
    }
}