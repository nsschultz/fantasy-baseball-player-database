using FantasyBaseball.Common.Enums;
using Xunit;

namespace FantasyBaseball.PlayerServiceDatabase.Entities.UnitTests
{
    public class PositionEntityTest
    {
        [Fact] public void DefaultsSetTest()
        {
            var obj = new PositionEntity();
            Assert.Null(obj.Code);
            Assert.Null(obj.FullName);
            Assert.Equal(PlayerType.U, obj.PlayerType);
            Assert.Equal(0, obj.SortOrder);
            Assert.Empty(obj.Players);
        }
    }
}