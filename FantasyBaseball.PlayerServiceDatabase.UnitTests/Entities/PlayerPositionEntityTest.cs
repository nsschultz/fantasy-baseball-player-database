using System;
using FantasyBaseball.PlayerServiceDatabase.Entities;
using Xunit;

namespace FantasyBaseball.PlayerServiceDatabase.UnitTests.Entities
{
    public class PlayerPositionEntityTest
    {
        [Fact] public void DefaultsSetTest()
        {
            var obj = new PlayerPositionEntity();
            Assert.Null(obj.PositionCode);
            Assert.Null(obj.Position);
            Assert.Equal((Guid) default, obj.PlayerId);
            Assert.Null(obj.Player);
        }
    }
}