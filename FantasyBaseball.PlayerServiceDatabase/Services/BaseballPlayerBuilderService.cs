using System.Collections.Generic;
using System.Linq;
using FantasyBaseball.CommonModels.Builders;
using FantasyBaseball.CommonModels.Enums;
using FantasyBaseball.CommonModels.Player;
using FantasyBaseball.CommonModels.Stats;
using FantasyBaseball.PlayerServiceDatabase.Entities;

namespace FantasyBaseball.PlayerServiceDatabase.Services
{
    /// <summary>Service for converting a PlayerEntity to a BaseballPlayer.</summary>
    public class BaseballPlayerBuilderService : IBaseballPlayerBuilderService
    {
        /// <summary>Converts a PlayerEntity to a BaseballPlayer.</summary>
        /// <param name="player">The database values.</param>
        /// <returns>A BaseballPlayer based off the database values.</returns>
        public BaseballPlayer BuildBaseballPlayer(PlayerEntity player)
        {
            return player == null 
                ? new BaseballPlayer() 
                : BuildStats(new BaseballPlayer 
                    {
                        PlayerInfo = BuildPlayerInfo(player),
                        LeagueInfo = BuildLeagueInfo(player),
                        DraftInfo = BuildDraftInfo(player),
                        BhqScores = BuildBhqScores(player),
                        YearToDateBattingStats = BuildBattingStats(player, StatsType.YTD),
                        YearToDatePitchingStats = BuildPitchingStats(player, StatsType.YTD),
                        ProjectedBattingStats = BuildBattingStats(player, StatsType.PROJ),
                        ProjectedPitchingStats = BuildPitchingStats(player, StatsType.PROJ)
                    });
        }

        private static BattingStats BuildBattingStats(PlayerEntity player, StatsType statsType)
        {
            var stats = player.BattingStats.FirstOrDefault(b => b.StatsType == statsType);
            return stats == null ? new BattingStats() : new BattingStats
            {
                AtBats = stats.AtBats,
                Runs = stats.Runs,
                Hits = stats.Hits,
                Doubles = stats.Doubles,
                Triples = stats.Triples,
                HomeRuns = stats.HomeRuns,
                RunsBattedIn = stats.RunsBattedIn,
                BaseOnBalls = stats.BaseOnBalls,
                StrikeOuts = stats.StrikeOuts,
                StolenBases = stats.StolenBases,
                CaughtStealing = stats.CaughtStealing,
                Power = stats.Power,
                Speed = stats.Speed
            };
        }

        private static BhqScores BuildBhqScores(PlayerEntity player) =>
            new BhqScores
            {
                MayberryMethod = player.MayberryMethod,
                Reliability = player.Reliability
            };

        private static DraftInfo BuildDraftInfo(PlayerEntity player) =>
            new DraftInfo
            {
                DraftRank = player.DraftRank,
                AverageDraftPick = player.AverageDraftPick,
                HighestPick = player.HighestPick,
                DraftedPercentage = player.DraftedPercentage
            };

        private static LeagueInfo BuildLeagueInfo(PlayerEntity player) =>
            new LeagueInfo
            {
                League1 = player.LeagueStatuses.Where(p => p.LeagueId == 1).Select(l => l.LeagueStatus).FirstOrDefault(),
                League2 = player.LeagueStatuses.Where(p => p.LeagueId == 2).Select(l => l.LeagueStatus).FirstOrDefault()
            };

        private static PitchingStats BuildPitchingStats(PlayerEntity player, StatsType statsType)
        {
            var stats = player.PitchingStats.FirstOrDefault(b => b.StatsType == statsType);
            return stats == null ? new PitchingStats() : new PitchingStats
            {
                Wins = stats.Wins,
                Losses = stats.Losses,
                QualityStarts = stats.QualityStarts,
                Saves = stats.Saves,
                BlownSaves = stats.BlownSaves,
                Holds = stats.Holds,
                InningsPitched = stats.InningsPitched,
                HitsAllowed = stats.HitsAllowed,
                EarnedRuns = stats.EarnedRuns,
                HomeRunsAllowed = stats.HomeRunsAllowed,
                BaseOnBallsAllowed = stats.BaseOnBallsAllowed,
                StrikeOuts = stats.StrikeOuts,
                FlyBallRate = stats.FlyBallRate,
                GroundBallRate = stats.GroundBallRate
            };
        }

        private static PlayerInfo BuildPlayerInfo(PlayerEntity player) =>
            new PlayerInfo
            {
                Id = player.BhqId,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Age = player.Age,
                Type = player.Type,
                Positions = BuildPositionString(player.Positions),
                Team = player.Team,
                Status = player.Status
            };

        private static string BuildPositionString(List<PlayerPositionEntity> positions) =>
            string.Join("-", positions.Select(p => p.Position).OrderBy(p => p.SortOrder).Select(p => p.Code));
        
        private static BaseballPlayer BuildStats(BaseballPlayer player)
        {
            player.YearToDateBattingStats = new BattingStatsBuilder().AddStats(player.YearToDateBattingStats).Build();
            player.YearToDatePitchingStats = new PitchingStatsBuilder().AddStats(player.YearToDatePitchingStats).Build();
            player.ProjectedBattingStats = new BattingStatsBuilder().AddStats(player.ProjectedBattingStats).Build();
            player.ProjectedPitchingStats = new PitchingStatsBuilder().AddStats(player.ProjectedPitchingStats).Build();
            player.CombinedBattingStats = new BattingStatsBuilder().AddStats(player.YearToDateBattingStats).AddStats(player.ProjectedBattingStats).Build();
            player.CombinedPitchingStats = new PitchingStatsBuilder().AddStats(player.YearToDatePitchingStats).AddStats(player.ProjectedPitchingStats).Build();
            return player;
        }
    }
}