using System.Collections.Generic;
using System.Linq;
using FantasyBaseball.CommonModels.Enums;
using FantasyBaseball.CommonModels.Player;
using FantasyBaseball.CommonModels.Stats;
using FantasyBaseball.PlayerServiceDatabase.Entities;
using FantasyBaseball.PlayerServiceDatabase.Services;
using Xunit;

namespace FantasyBaseball.PlayerServiceDatabase.UnitTests.Controllers
{
    public class PlayerEntityMergerServiceTest
    {
        private static readonly List<PositionEntity> POSITIONS = new List<PositionEntity> 
        {
            new PositionEntity { SortOrder = 87 , Code = "POS87"  , PlayerType = PlayerType.U },
            new PositionEntity { SortOrder = 100, Code = "POS100" , PlayerType = PlayerType.P },
            new PositionEntity { SortOrder = 5  , Code = "POS5"   , PlayerType = PlayerType.U },
            new PositionEntity { SortOrder = 10 , Code = "POS10"  , PlayerType = PlayerType.B },
            new PositionEntity { SortOrder = 123, Code = "DEFAULT", PlayerType = PlayerType.U },
            new PositionEntity { SortOrder = 1,   Code = "POS1"   , PlayerType = PlayerType.B },
        };

        [Fact] public void MergePlayerEntityNullTest() => Assert.Null(new PlayerEntityMergerService().MergePlayerEntity(null, new PlayerEntity(), POSITIONS));

        [Theory]
        [InlineData(10 , PlayerType.B)]
        [InlineData(100, PlayerType.P)]
        [InlineData(123, PlayerType.U)]
        public void MergePlayerEntityMatchMissingEntriesTest(int value, PlayerType type)
        {
            var player = BuildPlayer(value, type);
            var otherEntity = new PlayerEntityMergerService().MergePlayerEntity(BuildPlayer(value == 10 ? 100 : 10, PlayerType.U), null, POSITIONS);
            ValidatePlayer(value, player, new PlayerEntityMergerService().MergePlayerEntity(player, otherEntity, POSITIONS));
        }

        [Theory]
        [InlineData(10 , PlayerType.B)]
        [InlineData(100, PlayerType.P)]
        [InlineData(123, PlayerType.U)]
        public void MergePlayerEntityMatchSameEntriesTest(int value, PlayerType type)
        {
            var player = BuildPlayer(value, type);
            var otherEntity = new PlayerEntityMergerService().MergePlayerEntity(BuildPlayer(value, type), null, POSITIONS);
            ValidatePlayer(value, player, new PlayerEntityMergerService().MergePlayerEntity(player, otherEntity, POSITIONS));
        }

        [Theory]
        [InlineData(10 , PlayerType.B)]
        [InlineData(100, PlayerType.P)]
        [InlineData(123, PlayerType.U)]
        public void MergePlayerEntityNoMatchTest(int value, PlayerType type)
        {
            var player = BuildPlayer(value, type);
            ValidatePlayer(value, player, new PlayerEntityMergerService().MergePlayerEntity(player, null, POSITIONS));
        }

        private static BattingStats BuildBattingStats() =>
            new BattingStats
            {
                AtBats = 300,
                Runs = 75,
                Hits = 96,
                Doubles = 24,
                Triples = 6,
                HomeRuns = 12,
                RunsBattedIn = 48,
                BaseOnBalls = 30,
                StrikeOuts = 60,
                StolenBases = 9,
                CaughtStealing = 3,
                Power = 100,
                Speed = 61 
            };

        private static PitchingStats BuildPitchingStats() =>
            new PitchingStats
            {
                Wins = 12,
                Losses = 6,
                QualityStarts = 18,
                Saves = 9,
                BlownSaves = 3,
                Holds = 15,
                InningsPitched = 60,
                HitsAllowed = 45,
                EarnedRuns = 24,
                HomeRunsAllowed = 1,
                BaseOnBallsAllowed = 30,
                StrikeOuts = 120,
                FlyBallRate = 0.2,
                GroundBallRate = 0.31 
            };

        private static BaseballPlayer BuildPlayer(int value, PlayerType type) =>
            new BaseballPlayer 
            {
                PlayerInfo = new PlayerInfo 
                {
                    Id = value,
                    FirstName = $"First-{value}",
                    LastName = $"Last-{value}",
                    Age = value,
                    Type = type,
                    Positions = type == PlayerType.U ? "abc" : $"Pos{value}",
                    Team = $"Team-{value}",
                    Status = value == 10 ? PlayerStatus.XX : PlayerStatus.DL
                },
                DraftInfo = new DraftInfo
                {
                    DraftRank = value + 1,
                    AverageDraftPick = value + 2,
                    HighestPick = value + 3,
                    DraftedPercentage = value + 4
                },
                BhqScores = new BhqScores { MayberryMethod = value + 5, Reliability = value + 6 },
                LeagueInfo = new LeagueInfo 
                {
                    League1 = value == 10 ? LeagueStatus.R : LeagueStatus.A,
                    League2 = value != 10 ? LeagueStatus.X : LeagueStatus.A,
                },
                YearToDateBattingStats = value == 10 ? BuildBattingStats() : new BattingStats(),
                ProjectedBattingStats = value != 10 ? BuildBattingStats() : new BattingStats(),
                YearToDatePitchingStats = value == 10 ? BuildPitchingStats() : new PitchingStats(),
                ProjectedPitchingStats = value != 10 ? BuildPitchingStats() : new PitchingStats()
            };

        private static string BuildPositionString(List<PlayerPositionEntity> positions) => string.Join("-", positions.Select(p => p.PositionCode));

        private static void ValidatePlayer(int value, BaseballPlayer expected, PlayerEntity actual)
        {
            Assert.Equal(expected.PlayerInfo.Id, actual.BhqId);
            Assert.Equal(expected.PlayerInfo.FirstName, actual.FirstName);
            Assert.Equal(expected.PlayerInfo.LastName, actual.LastName);
            Assert.Equal(expected.PlayerInfo.Age, actual.Age);
            Assert.Equal(expected.PlayerInfo.Type, actual.Type);
            Assert.Equal(expected.PlayerInfo.Type == PlayerType.U ? "DEFAULT" : expected.PlayerInfo.Positions.ToUpper(), BuildPositionString(actual.Positions));
            Assert.Equal(expected.PlayerInfo.Team, actual.Team);
            Assert.Equal(expected.PlayerInfo.Status, actual.Status);
            Assert.Equal(expected.DraftInfo.DraftRank, actual.DraftRank);
            Assert.Equal(expected.DraftInfo.AverageDraftPick, actual.AverageDraftPick);
            Assert.Equal(expected.DraftInfo.HighestPick, actual.HighestPick);
            Assert.Equal(expected.DraftInfo.DraftedPercentage, actual.DraftedPercentage);
            Assert.Equal(expected.BhqScores.MayberryMethod, actual.MayberryMethod);
            Assert.Equal(expected.BhqScores.Reliability, actual.Reliability);
            Assert.Equal(value == 10 ? expected.LeagueInfo.League1 : expected.LeagueInfo.League2, actual.LeagueStatuses.First().LeagueStatus);
            Assert.Single(actual.BattingStats);
            Assert.Single(actual.PitchingStats);
            ValidatePlayerBattingStats(value == 10 ? expected.YearToDateBattingStats : expected.ProjectedBattingStats, actual.BattingStats.First());
            ValidatePlayerPitchingStats(value == 10 ? expected.YearToDatePitchingStats : expected.ProjectedPitchingStats, actual.PitchingStats.First());
        }

        private static void ValidatePlayerBattingStats(BattingStats expected, BattingStatsEntity actual)
        {
            Assert.Equal(expected.AtBats, actual.AtBats);
            Assert.Equal(expected.Runs, actual.Runs);
            Assert.Equal(expected.Hits, actual.Hits);
            Assert.Equal(expected.Doubles, actual.Doubles);
            Assert.Equal(expected.Triples, actual.Triples);
            Assert.Equal(expected.HomeRuns, actual.HomeRuns);
            Assert.Equal(expected.RunsBattedIn, actual.RunsBattedIn);
            Assert.Equal(expected.BaseOnBalls, actual.BaseOnBalls);
            Assert.Equal(expected.StrikeOuts, actual.StrikeOuts);
            Assert.Equal(expected.StolenBases, actual.StolenBases);
            Assert.Equal(expected.CaughtStealing, actual.CaughtStealing);
            Assert.Equal(expected.Power, actual.Power);
            Assert.Equal(expected.Speed, actual.Speed);
        }

        private static void ValidatePlayerPitchingStats(PitchingStats expected, PitchingStatsEntity actual)
        {
            Assert.Equal(expected.Wins, actual.Wins);
            Assert.Equal(expected.Losses, actual.Losses);
            Assert.Equal(expected.QualityStarts, actual.QualityStarts);
            Assert.Equal(expected.Saves, actual.Saves);
            Assert.Equal(expected.BlownSaves, actual.BlownSaves);
            Assert.Equal(expected.Holds, actual.Holds);
            Assert.Equal(expected.InningsPitched, actual.InningsPitched);
            Assert.Equal(expected.HitsAllowed, actual.HitsAllowed);
            Assert.Equal(expected.EarnedRuns, actual.EarnedRuns);
            Assert.Equal(expected.HomeRunsAllowed, actual.HomeRunsAllowed);
            Assert.Equal(expected.BaseOnBallsAllowed, actual.BaseOnBallsAllowed);
            Assert.Equal(expected.StrikeOuts, actual.StrikeOuts);
            Assert.Equal(expected.FlyBallRate, actual.FlyBallRate);
            Assert.Equal(expected.GroundBallRate, actual.GroundBallRate);
        }
    }
}