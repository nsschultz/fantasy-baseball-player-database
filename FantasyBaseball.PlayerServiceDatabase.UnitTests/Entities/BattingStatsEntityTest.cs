using System;
using FantasyBaseball.Common.Enums;
using Xunit;

namespace FantasyBaseball.PlayerServiceDatabase.Entities.UnitTests
{
    public class BattingStatsEntityTest
    {
        [Fact] public void DefaultsSetTest()
        {
            var obj = new BattingStatsEntity();
            Assert.Equal((Guid) default, obj.PlayerId);
            Assert.Equal(StatsType.UNKN, obj.StatsType);
            Assert.Equal(0, obj.AtBats);
            Assert.Equal(0, obj.Runs);
            Assert.Equal(0, obj.Hits);
            Assert.Equal(0, obj.Doubles);
            Assert.Equal(0, obj.Triples);
            Assert.Equal(0, obj.HomeRuns);
            Assert.Equal(0, obj.RunsBattedIn);
            Assert.Equal(0, obj.BaseOnBalls);
            Assert.Equal(0, obj.StrikeOuts);
            Assert.Equal(0, obj.StolenBases);
            Assert.Equal(0, obj.CaughtStealing);
            Assert.Equal(0, obj.Power);
            Assert.Equal(0, obj.Speed);
            Assert.Null(obj.Player);
        }
    }
}