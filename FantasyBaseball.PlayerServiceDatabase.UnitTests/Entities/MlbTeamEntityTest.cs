using FantasyBaseball.PlayerServiceDatabase.Entities;
using Xunit;

namespace FantasyBaseball.PlayerServiceDatabase.UnitTests.Entities
{
    public class MlbTeamEntityTest
    {
        [Fact] public void DefaultsSetTest()
        {
            var obj = new MlbTeamEntity();
            Assert.Null(obj.Code);
            Assert.Null(obj.AlternativeCode);
            Assert.Null(obj.MlbLeagueId);
            Assert.Null(obj.City);
            Assert.Null(obj.Nickname);
            Assert.Empty(obj.Players);
        }
    }
}