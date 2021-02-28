using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FantasyBaseball.Common.Models;
using FantasyBaseball.PlayerServiceDatabase.Database;
using FantasyBaseball.PlayerServiceDatabase.Entities;
using Microsoft.EntityFrameworkCore;

namespace FantasyBaseball.PlayerServiceDatabase.Services
{
    /// <summary>Service for upsert players.</summary>
    public class UpsertPlayersService : IUpsertPlayersService
    {                
        private readonly IPlayerEntityMergerService _entityMerger;
        private readonly IPlayerContext _context;

        /// <summary>Creates a new instance of the service.</summary>
        /// <param name="context">The player context.</param>
        /// <param name="entityMerger">Service for converting a BaseballPlayer to a PlayerEntity.</param>
        public UpsertPlayersService(IPlayerContext context, IPlayerEntityMergerService entityMerger) 
        {
            _context = context;
            _entityMerger = entityMerger;
        }

        /// <summary>Gets the players from the underlying source.</summary>
        /// <param name="players">All of the players to upsert into the source.</param>
        public async Task UpsertPlayers(List<BaseballPlayer> players)
        {
            try
            {
                var positions = await _context.Positions.ToListAsync();
                var teams = await _context.MlbTeams.ToListAsync();
                await _context.BeginTransaction();
                await UpsertPlayers(players.Select(p => _entityMerger.MergePlayerEntity(p, FindEntity(p), positions, teams)));
                await _context.Commit();
            }
            catch(Exception)
            {
                await _context.Rollback();
                throw;
            }
        }

        private PlayerEntity FindEntity(BaseballPlayer player) =>
            player.Id != default
                ? _context.Players
                    .Include(p => p.LeagueStatuses)
                    .Include(p => p.MlbTeam)
                    .Include(p => p.Positions).ThenInclude(p => p.Position)
                    .Include(p => p.BattingStats)
                    .Include(p => p.PitchingStats)
                    .FirstOrDefault(p => p.Id == player.Id)
                : _context.Players.AsQueryable()
                    .Include(p => p.LeagueStatuses)
                    .Include(p => p.MlbTeam)
                    .Include(p => p.Positions).ThenInclude(p => p.Position)
                    .Include(p => p.BattingStats)
                    .Include(p => p.PitchingStats)
                    .Where(p => p.BhqId == player.BhqId)
                    .Where(p => p.Type == player.Type)
                    .FirstOrDefault();

        private async Task UpsertPlayers(IEnumerable<PlayerEntity> entities)
        {
            await _context.Players.AddRangeAsync(entities.Where(p => p.Id == default));
            _context.Players.UpdateRange(entities.Where(p => p.Id != default));
        }
    }
}