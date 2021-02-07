using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FantasyBaseball.Common.Enums;
using FantasyBaseball.PlayerServiceDatabase.Database;
using FantasyBaseball.PlayerServiceDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace FantasyBaseball.PlayerServiceDatabase.Services.UnitTests
{
    public class GetPlayerServiceTest : IDisposable
    {
        private PlayerContext _context;

        public GetPlayerServiceTest() => _context = CreateContext().Result;

        [Fact] public async void GetPlayersTest()
        {
            var players = await new GetPlayersService(_context).GetPlayers();
            Assert.Equal(3, players.Count);
            players.ForEach(player => 
            {
                Assert.Equal(player.BhqId < 3 ? "Brewers" : "Rays", player.MlbTeam.Nickname);
                Assert.Equal(player.BhqId, player.Type == PlayerType.B ? player.BattingStats.First().AtBats : player.PitchingStats.First().InningsPitched);
                Assert.Equal(player.BhqId, player.LeagueStatuses.First().LeagueId);
            });
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private async Task<PlayerContext> CreateContext()
        {
            var options = new DbContextOptionsBuilder<PlayerContext>()
                .UseInMemoryDatabase(databaseName: "GetPlayersTest")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var context = new PlayerContext(options);
            context.Database.EnsureCreated();
            Assert.Equal(31, await context.MlbTeams.CountAsync());
            await context.AddRangeAsync(
                new PlayerEntity 
                {
                    BhqId = 1, 
                    Type = PlayerType.B, 
                    Team = "MIL",  
                    BattingStats = new List<BattingStatsEntity> { new BattingStatsEntity { StatsType = StatsType.UNKN, AtBats = 1 } },
                    LeagueStatuses = new List<PlayerLeagueStatusEntity> { new PlayerLeagueStatusEntity { LeagueId = 1, LeagueStatus = LeagueStatus.R } }
                },
                new PlayerEntity 
                {
                    BhqId = 2, 
                    Type = PlayerType.P, 
                    Team = "MIL", 
                    PitchingStats = new List<PitchingStatsEntity> { new PitchingStatsEntity { StatsType = StatsType.UNKN, InningsPitched = 2 } },
                    LeagueStatuses = new List<PlayerLeagueStatusEntity> { new PlayerLeagueStatusEntity { LeagueId = 2, LeagueStatus = LeagueStatus.R } }
                },
                new PlayerEntity
                {
                    BhqId = 3, 
                    Type = PlayerType.B, 
                    Team = "TB", 
                    BattingStats = new List<BattingStatsEntity> { new BattingStatsEntity { StatsType = StatsType.UNKN, AtBats = 3 } },
                    LeagueStatuses = new List<PlayerLeagueStatusEntity> { new PlayerLeagueStatusEntity { LeagueId = 3, LeagueStatus = LeagueStatus.R } }
                }
            );
            await context.SaveChangesAsync();
            Assert.Equal(3, await context.Players.CountAsync());
            return context;
        }
    }
}