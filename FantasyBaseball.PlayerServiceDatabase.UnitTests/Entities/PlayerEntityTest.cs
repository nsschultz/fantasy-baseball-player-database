using System;
using FantasyBaseball.CommonModels.Enums;
using FantasyBaseball.PlayerServiceDatabase.Entities;
using Xunit;

namespace FantasyBaseball.PlayerServiceDatabase.UnitTests.Entities
{
    public class PlayerEntityTest
    {
        [Fact] public void DefaultsSetTest()
        {
            var obj = new PlayerEntity();
            Assert.Equal((Guid) default, obj.Id);
            Assert.Equal(0, obj.BhqId);
            Assert.Equal(PlayerType.U, obj.Type);
            Assert.Null(obj.FirstName);
            Assert.Null(obj.LastName);
            Assert.Equal(0, obj.Age);
            Assert.Null(obj.Team);
            Assert.Equal(PlayerStatus.XX, obj.Status);
            Assert.Equal(0, obj.DraftRank);
            Assert.Equal(0, obj.AverageDraftPick);
            Assert.Equal(0, obj.HighestPick);
            Assert.Equal(0, obj.DraftedPercentage);
            Assert.Equal(0, obj.Reliability);
            Assert.Equal(0, obj.MayberryMethod);
            Assert.Empty(obj.LeagueStatuses);
            Assert.Empty(obj.BattingStats);
            Assert.Empty(obj.PitchingStats);
            Assert.Empty(obj.Positions);
        }
    }
}