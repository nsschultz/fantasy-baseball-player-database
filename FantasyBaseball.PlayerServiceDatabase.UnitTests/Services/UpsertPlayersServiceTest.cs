using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FantasyBaseball.Common.Enums;
using FantasyBaseball.Common.Models;
using FantasyBaseball.PlayerServiceDatabase.Database;
using FantasyBaseball.PlayerServiceDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;

namespace FantasyBaseball.PlayerServiceDatabase.Services.UnitTests
{
    public class UpdatePlayerServiceTest : IDisposable
    {
        private readonly Guid PlayerMatchingId = Guid.NewGuid();
        private readonly Guid PlayerMissingId = Guid.NewGuid();
        private PlayerContext _context;

        public UpdatePlayerServiceTest() => _context = CreateContext().Result;

        [Fact] public async void UpsertPlayersTestException()
        {
            var values = new List<BaseballPlayer> 
            {
                new BaseballPlayer { BhqId = 1, Type = PlayerType.B, Team = "MIL" },
                new BaseballPlayer { BhqId = 2, Type = PlayerType.B, Team = "MIL" },
                new BaseballPlayer { BhqId = 4, Type = PlayerType.P, Team = "MIL" },
                new BaseballPlayer { Id = PlayerMatchingId, BhqId = 5, Type = PlayerType.P, Team = "MIL" },
                new BaseballPlayer { Id = PlayerMissingId, BhqId = 1, Type = PlayerType.B, Team = "MIL" },
            };
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await new UpsertPlayersService(_context, BuildMergeService()).UpsertPlayers(values));
            Assert.Equal(4, _context.Players.Count());
            Assert.Equal(4, _context.LeagueStatuses.Count());
            Assert.Equal(3, _context.BattingStats.Count());
            Assert.Equal(1, _context.PitchingStats.Count());
            Assert.Equal(31, _context.MlbTeams.Count());
            Assert.Equal(15, _context.Positions.Count());
        }

        [Fact] public async void UpsertPlayersTestValid()
        {
            var values = new List<BaseballPlayer> 
            {
                new BaseballPlayer { BhqId = 1, Type = PlayerType.B, Team = "MIL" },
                new BaseballPlayer { BhqId = 2, Type = PlayerType.B, Team = "MIL" },
                new BaseballPlayer { BhqId = 4, Type = PlayerType.P, Team = "MIL" },
                new BaseballPlayer { Id = PlayerMatchingId, BhqId = 5, Type = PlayerType.P, Team = "MIL" },
                new BaseballPlayer { Id = PlayerMissingId, BhqId = 1, Type = PlayerType.P, Team = "MIL" },
            };
            await new UpsertPlayersService(_context, BuildMergeService()).UpsertPlayers(values);
            Assert.Equal(7, _context.Players.Count());
            Assert.Equal(5, _context.LeagueStatuses.Count());
            Assert.Equal(3, _context.BattingStats.Count());
            Assert.Equal(1, _context.PitchingStats.Count());
            Assert.Equal(31, _context.MlbTeams.Count());
            Assert.Equal(15, _context.Positions.Count());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private IPlayerEntityMergerService BuildMergeService() 
        {
            var mergeService = new Mock<IPlayerEntityMergerService>();
            mergeService.Setup(o => o.MergePlayerEntity(It.IsAny<BaseballPlayer>(), It.IsAny<PlayerEntity>(), It.IsAny<List<PositionEntity>>(), It.IsAny<List<MlbTeamEntity>>()))
                .Returns((BaseballPlayer player, PlayerEntity entity, List<PositionEntity> positions, List<MlbTeamEntity> teams) => 
                    entity != null 
                    ? ModifyExisting(entity) 
                    : new PlayerEntity 
                    { 
                        BhqId = player.BhqId, 
                        Type = player.Type, 
                        Team = player.Team,
                        LeagueStatuses = new List<PlayerLeagueStatusEntity> { new PlayerLeagueStatusEntity { LeagueId = 1, LeagueStatus = LeagueStatus.X } }
                    });
            return mergeService.Object;
        }

        private async Task<PlayerContext> CreateContext()
        {
            var options = new DbContextOptionsBuilder<PlayerContext>()
                .UseInMemoryDatabase(databaseName: "UpdatePlayerServiceTest")
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
                },
                new PlayerEntity
                {
                    Id = PlayerMatchingId,
                    BhqId = 5, 
                    Type = PlayerType.B, 
                    Team = "MIL", 
                    BattingStats = new List<BattingStatsEntity> { new BattingStatsEntity { StatsType = StatsType.UNKN, AtBats = 5 } },
                    LeagueStatuses = new List<PlayerLeagueStatusEntity> { new PlayerLeagueStatusEntity { LeagueId = 5, LeagueStatus = LeagueStatus.R } }
                }
            );
            await context.SaveChangesAsync();
            Assert.Equal(4, await context.Players.CountAsync());
            return context;
        }

        private static PlayerEntity ModifyExisting(PlayerEntity player)
        {
            player.LeagueStatuses.Clear();
            return player;
        }
    }
}