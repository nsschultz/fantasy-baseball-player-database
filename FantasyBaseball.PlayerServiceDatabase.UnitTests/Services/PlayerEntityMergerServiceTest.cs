using System.Collections.Generic;
using System.Linq;
using FantasyBaseball.Common.Enums;
using FantasyBaseball.Common.Models;
using FantasyBaseball.PlayerServiceDatabase.Entities;
using Xunit;

namespace FantasyBaseball.PlayerServiceDatabase.Services.UnitTests
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

        private static readonly List<MlbTeamEntity> TEAMS = new List<MlbTeamEntity> 
        {
            new MlbTeamEntity { Code = ""        , MlbLeagueId = ""  , City = "Default", Nickname = "Default"                             },
            new MlbTeamEntity { Code = "TEAM-123", MlbLeagueId = "AL", City = "Team"   , Nickname = "1"                                   },
            new MlbTeamEntity { Code = "TEAM-100", MlbLeagueId = "NL", City = "Team"   , Nickname = "2"      ,  AlternativeCode = "100-TM"}
        };

        private static readonly Dictionary<int, string> EXPECTED_TEAMS = new Dictionary<int, string> { { 10, "" }, { 100, "TEAM-100" }, { 123, "TEAM-123" } };

        [Fact] public void MergePlayerEntityNullTest() => Assert.Null(new PlayerEntityMergerService().MergePlayerEntity(null, new PlayerEntity(), POSITIONS, TEAMS));

        [Theory]
        [InlineData(10 , PlayerType.B)]
        [InlineData(100, PlayerType.P)]
        [InlineData(123, PlayerType.U)]
        public void MergePlayerEntityMatchMissingEntriesTest(int value, PlayerType type)
        {
            var player = BuildPlayer(value, type);
            var otherEntity = new PlayerEntityMergerService().MergePlayerEntity(BuildPlayer(value == 10 ? 100 : 10, PlayerType.U), null, POSITIONS, TEAMS);
            ValidatePlayer(value, player, new PlayerEntityMergerService().MergePlayerEntity(player, otherEntity, POSITIONS, TEAMS));
        }

        [Theory]
        [InlineData(10 , PlayerType.B)]
        [InlineData(100, PlayerType.P)]
        [InlineData(123, PlayerType.U)]
        public void MergePlayerEntityMatchSameEntriesTest(int value, PlayerType type)
        {
            var player = BuildPlayer(value, type);
            var otherEntity = new PlayerEntityMergerService().MergePlayerEntity(BuildPlayer(value, type), null, POSITIONS, TEAMS);
            ValidatePlayer(value, player, new PlayerEntityMergerService().MergePlayerEntity(player, otherEntity, POSITIONS, TEAMS));
        }

        [Theory]
        [InlineData(10 , PlayerType.B)]
        [InlineData(100, PlayerType.P)]
        [InlineData(123, PlayerType.U)]
        public void MergePlayerEntityNoMatchTest(int value, PlayerType type)
        {
            var player = BuildPlayer(value, type);
            ValidatePlayer(value, player, new PlayerEntityMergerService().MergePlayerEntity(player, null, POSITIONS, TEAMS));
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
                BhqId = value,
                FirstName = $"First-{value}",
                LastName = $"Last-{value}",
                Age = value,
                Type = type,
                Positions = type == PlayerType.U ? "abc" : $"Pos{value}",
                Team = value == 100 ? $"{value}-tm" : $"Team-{value}",
                Status = value == 10 ? PlayerStatus.XX : PlayerStatus.DL,
                DraftRank = value + 1,
                AverageDraftPick = value + 2,
                HighestPick = value + 3,
                DraftedPercentage = value + 4,
                MayberryMethod = value + 5, 
                Reliability = value + 6,
                League1 = value == 10 ? LeagueStatus.R : LeagueStatus.A,
                League2 = value != 10 ? LeagueStatus.X : LeagueStatus.A,
                YearToDateBattingStats = value == 10 ? BuildBattingStats() : new BattingStats(),
                ProjectedBattingStats = value != 10 ? BuildBattingStats() : new BattingStats(),
                YearToDatePitchingStats = value == 10 ? BuildPitchingStats() : new PitchingStats(),
                ProjectedPitchingStats = value != 10 ? BuildPitchingStats() : new PitchingStats()
            };

        private static string BuildPositionString(List<PlayerPositionEntity> positions) => string.Join("-", positions.Select(p => p.PositionCode));

        private static void ValidatePlayer(int value, BaseballPlayer expected, PlayerEntity actual)
        {
            Assert.Equal(expected.BhqId, actual.BhqId);
            Assert.Equal(expected.FirstName, actual.FirstName);
            Assert.Equal(expected.LastName, actual.LastName);
            Assert.Equal(expected.Age, actual.Age);
            Assert.Equal(expected.Type, actual.Type);
            Assert.Equal(expected.Type == PlayerType.U ? "DEFAULT" : expected.Positions.ToUpper(), BuildPositionString(actual.Positions));
            Assert.Equal(EXPECTED_TEAMS[value], actual.Team);
            Assert.Equal(expected.Status, actual.Status);
            Assert.Equal(expected.DraftRank, actual.DraftRank);
            Assert.Equal(expected.AverageDraftPick, actual.AverageDraftPick);
            Assert.Equal(expected.HighestPick, actual.HighestPick);
            Assert.Equal(expected.DraftedPercentage, actual.DraftedPercentage);
            Assert.Equal(expected.MayberryMethod, actual.MayberryMethod);
            Assert.Equal(expected.Reliability, actual.Reliability);
            Assert.Equal(value == 10 ? expected.League1 : expected.League2, actual.LeagueStatuses.First().LeagueStatus);
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