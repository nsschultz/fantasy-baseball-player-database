using System;
using System.Collections.Generic;
using System.Linq;
using FantasyBaseball.Common.Enums;
using FantasyBaseball.Common.Models;
using FantasyBaseball.PlayerServiceDatabase.Entities;
using Xunit;

namespace FantasyBaseball.PlayerServiceDatabase.Services.UnitTests
{
    public class BaseballPlayerBuilderServiceTest
    {
        [Fact] public void BuildBaseballPlayerNullTest() => Assert.Equal(0, new BaseballPlayerBuilderService().BuildBaseballPlayer(null).BhqId);

        [Theory]
        [InlineData(10 , PlayerType.B)]
        [InlineData(100, PlayerType.P)]
        public void BuildBaseballPlayerValidTest(int value, PlayerType type)
        {
            var player = BuildPlayer(value, type);
            ValidatePlayer(value, player, new BaseballPlayerBuilderService().BuildBaseballPlayer(player));
        }

        private static PlayerEntity BuildPlayer(int value, PlayerType type) =>
            new PlayerEntity 
            {
                Id = Guid.NewGuid(),
                BhqId = value,
                FirstName = $"First-{value}",
                LastName = $"Last-{value}",
                Age = value,
                Type = type,
                Positions = new List<PlayerPositionEntity> 
                { 
                    new PlayerPositionEntity { Position = new PositionEntity { SortOrder = value * 100, Code = $"Pos-{value * 100}" } },
                    new PlayerPositionEntity { Position = new PositionEntity { SortOrder = value * 10, Code = $"Pos-{value * 10}" } },
                    new PlayerPositionEntity { Position = new PositionEntity { SortOrder = value, Code = $"Pos-{value}" } },
                },
                Team = $"Team-{value}",
                Status = value == 10 ? PlayerStatus.XX : PlayerStatus.DL,
                DraftRank = value + 1,
                AverageDraftPick = value + 2,
                HighestPick = value + 3,
                DraftedPercentage = value + 4,
                MayberryMethod = value + 5,
                Reliability = value + 6,
                LeagueStatuses = new List<PlayerLeagueStatusEntity> 
                {
                    new PlayerLeagueStatusEntity 
                    { 
                        LeagueId = value == 10 ? 1 : 2, 
                        LeagueStatus = value == 10 ? LeagueStatus.R : LeagueStatus.X 
                    } 
                },
                BattingStats = new List<BattingStatsEntity> 
                {
                    new BattingStatsEntity 
                    { 
                        StatsType = value == 10 ? StatsType.YTD : StatsType.PROJ,
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
                    }
                },
                PitchingStats = new List<PitchingStatsEntity> 
                {
                    new PitchingStatsEntity 
                    { 
                        StatsType = value == 10 ? StatsType.YTD : StatsType.PROJ,
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
                    }
                }
            };

        private static string BuildPositionString(List<PlayerPositionEntity> positions) =>
            string.Join("-", positions.Select(p => p.Position).OrderBy(p => p.SortOrder).Select(p => p.Code));

        private static void ValidatePlayer(int value, PlayerEntity expected, BaseballPlayer actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.BhqId, actual.BhqId);
            Assert.Equal(expected.FirstName, actual.FirstName);
            Assert.Equal(expected.LastName, actual.LastName);
            Assert.Equal(expected.Age, actual.Age);
            Assert.Equal(expected.Type, actual.Type);
            Assert.Equal(BuildPositionString(expected.Positions), actual.Positions);
            Assert.Equal(expected.Team, actual.Team);
            Assert.Equal(expected.Status, actual.Status);
            Assert.Equal(expected.DraftRank, actual.DraftRank);
            Assert.Equal(expected.AverageDraftPick, actual.AverageDraftPick);
            Assert.Equal(expected.HighestPick, actual.HighestPick);
            Assert.Equal(expected.DraftedPercentage, actual.DraftedPercentage);
            Assert.Equal(expected.MayberryMethod, actual.MayberryMethod);
            Assert.Equal(expected.Reliability, actual.Reliability);
            Assert.Equal(value == 10 ? expected.LeagueStatuses.First().LeagueStatus : LeagueStatus.A, actual.League1);
            Assert.Equal(value != 10 ? expected.LeagueStatuses.First().LeagueStatus : LeagueStatus.A, actual.League2);
            ValidatePlayerBattingStats(value == 10 ? expected.BattingStats.First() : new BattingStatsEntity(), actual.YearToDateBattingStats);
            ValidatePlayerBattingStats(value != 10 ? expected.BattingStats.First() : new BattingStatsEntity(), actual.ProjectedBattingStats);
            ValidatePlayerBattingStats(expected.BattingStats.First(), actual.CombinedBattingStats);
            ValidatePlayerPitchingStats(value == 10 ? expected.PitchingStats.First() : new PitchingStatsEntity(), actual.YearToDatePitchingStats);
            ValidatePlayerPitchingStats(value != 10 ? expected.PitchingStats.First() : new PitchingStatsEntity(), actual.ProjectedPitchingStats);
            ValidatePlayerPitchingStats(expected.PitchingStats.First(), actual.CombinedPitchingStats);
        }

        private static void ValidatePlayerBattingStats(BattingStatsEntity expected, BattingStats actual)
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

        private static void ValidatePlayerPitchingStats(PitchingStatsEntity expected, PitchingStats actual)
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