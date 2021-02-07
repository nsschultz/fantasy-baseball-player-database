using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FantasyBaseball.Common.Enums;
using FantasyBaseball.Common.Exceptions;
using FantasyBaseball.Common.Models;
using FantasyBaseball.PlayerServiceDatabase.Database;
using FantasyBaseball.PlayerServiceDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;

namespace FantasyBaseball.PlayerServiceDatabase.Services.UnitTests
{
    public class PlayerUpdateServiceTest : IDisposable
    {
        private readonly Guid PlayerMatchingId = Guid.NewGuid();
        private readonly Guid PlayerMissingId = Guid.NewGuid();
        private PlayerContext _context;

        public PlayerUpdateServiceTest() => _context = CreateContext().Result;
        
        [Fact] public async void UpdatePlayerTestDatabaseException()
        {
            Assert.Equal("TB", _context.Players.Find(PlayerMatchingId).Team);
            var player = new BaseballPlayer { Id = PlayerMatchingId, BhqId = 2, Type = PlayerType.B, Team = "TB" };
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await new PlayerUpdateService(_context, BuildMergeService()).UpdatePlayer(player));
            Assert.Equal("TB", _context.Players.Find(PlayerMatchingId).Team);
        }

        [Fact] public async void UpdatePlayerTestMissingIdException()
        {
            Assert.Equal("TB", _context.Players.Find(PlayerMatchingId).Team);
            var player = new BaseballPlayer { Id = PlayerMissingId, BhqId = 1, Type = PlayerType.B, Team = "MIL" };
            await Assert.ThrowsAsync<BadRequestException>(async () => await new PlayerUpdateService(_context, BuildMergeService()).UpdatePlayer(player));
            Assert.Equal("TB", _context.Players.Find(PlayerMatchingId).Team);
        }

        [Fact] public async void UpdatePlayerTestValid()
        {
            Assert.Equal("TB", _context.Players.Find(PlayerMatchingId).Team);
            var player = new BaseballPlayer { Id = PlayerMatchingId, BhqId = 1, Type = PlayerType.B, Team = "MIL" };
            await new PlayerUpdateService(_context, BuildMergeService()).UpdatePlayer(player);
            Assert.Equal("MIL", _context.Players.Find(PlayerMatchingId).Team);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private IPlayerEntityMergerService BuildMergeService() 
        {
            var mergeService = new Mock<IPlayerEntityMergerService>();
            mergeService.Setup(o => o.MergePlayerEntity(It.IsAny<BaseballPlayer>(), It.IsAny<PlayerEntity>(), It.IsAny<List<PositionEntity>>()))
                .Returns((BaseballPlayer player, PlayerEntity entity, List<PositionEntity> positions) =>
                    { 
                        entity.BhqId = player.BhqId;
                        entity.Type = player.Type;
                        entity.Team = player.Team;
                        return entity;
                    });
            return mergeService.Object;
        }

        private async Task<PlayerContext> CreateContext()
        {
            var options = new DbContextOptionsBuilder<PlayerContext>()
                .UseInMemoryDatabase(databaseName: "PlayerUpdateServiceTest")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var context = new PlayerContext(options);
            context.Database.EnsureCreated();
            Assert.Equal(31, await context.MlbTeams.CountAsync());
            await context.AddRangeAsync(
                new PlayerEntity 
                {
                    Id = PlayerMatchingId,
                    BhqId = 1, 
                    Type = PlayerType.B, 
                    Team = "TB", 
                    BattingStats = new List<BattingStatsEntity> { new BattingStatsEntity { StatsType = StatsType.UNKN, AtBats = 1 } },
                    LeagueStatuses = new List<PlayerLeagueStatusEntity> { new PlayerLeagueStatusEntity { LeagueId = 1, LeagueStatus = LeagueStatus.R } }
                },
                new PlayerEntity
                {
                    BhqId = 2, 
                    Type = PlayerType.B, 
                    Team = "MIL", 
                    BattingStats = new List<BattingStatsEntity> { new BattingStatsEntity { StatsType = StatsType.UNKN, AtBats = 2 } },
                    LeagueStatuses = new List<PlayerLeagueStatusEntity> { new PlayerLeagueStatusEntity { LeagueId = 2, LeagueStatus = LeagueStatus.R } }
                }
            );
            await context.SaveChangesAsync();
            Assert.Equal(2, await context.Players.CountAsync());
            return context;
        }
    }
}