using System;
using FantasyBaseball.CommonModels.Enums;
using FantasyBaseball.PlayerServiceDatabase.Entities;
using Xunit;

namespace FantasyBaseball.PlayerServiceDatabase.UnitTests.Entities
{
    public class PlayerLeagueStatusEntityTest
    {
        [Fact] public void DefaultsSetTest()
        {
            var obj = new PlayerLeagueStatusEntity();
            Assert.Equal((Guid) default, obj.PlayerId);
            Assert.Equal(0, obj.LeagueId);
            Assert.Equal(LeagueStatus.A, obj.LeagueStatus);
            Assert.Null(obj.Player);
        }
    }
}