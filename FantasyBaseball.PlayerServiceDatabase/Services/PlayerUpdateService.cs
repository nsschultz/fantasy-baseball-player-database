using System;
using System.Threading.Tasks;
using FantasyBaseball.Common.Exceptions;
using FantasyBaseball.Common.Models;
using FantasyBaseball.PlayerServiceDatabase.Database;
using Microsoft.EntityFrameworkCore;

namespace FantasyBaseball.PlayerServiceDatabase.Services
{
    /// <summary>Service for updaing a player.</summary>
    public class PlayerUpdateService : IPlayerUpdateService
    {
        private readonly IPlayerEntityMergerService _entityMerger;
        private readonly IPlayerContext _context;

        /// <summary>Creates a new instance of the service.</summary>
        /// <param name="context">The player context.</param>
        /// <param name="entityMerger">Service for converting a BaseballPlayer to a PlayerEntity.</param>
        public PlayerUpdateService(IPlayerContext context, IPlayerEntityMergerService entityMerger) 
        {
            _context = context;
            _entityMerger = entityMerger;
        }

        /// <summary>Updates the given player.</summary>
        /// <param name="player">The player to update.</param>
        public async Task UpdatePlayer(BaseballPlayer player)
        {
            var existingPlayer = await _context.Players
                .Include(p => p.LeagueStatuses)
                .Include(p => p.MlbTeam)
                .Include(p => p.Positions).ThenInclude(p => p.Position)
                .Include(p => p.BattingStats)
                .Include(p => p.PitchingStats)
                .FirstOrDefaultAsync(p => p.Id == player.Id);
            if (existingPlayer == null) throw new BadRequestException("This player does not exist");
            var positions = await _context.Positions.ToListAsync();
            var teams = await _context.Teams.ToListAsync();
            var updatedPlayer = _entityMerger.MergePlayerEntity(player, existingPlayer, positions, teams);
            try
            {
                await _context.BeginTransaction();
                _context.Players.Update(updatedPlayer);
                await _context.Commit();
            }
            catch(Exception)
            {
                await _context.Rollback();
                throw;
            }
        }
    }
}