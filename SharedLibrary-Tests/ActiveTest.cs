using SharedLibrary.Actions;
using SharedLibrary.Models.Game;
using Xunit;

namespace SharedLibrary_Tests {
    public class ActiveTest {
		[Fact]
		public void IsNotActiveOnFirstTurn() {
			var state = Common.GameState;
			var user  = state.Users[0];
			var card  = user.HandSet.Find(
				c => (c.Type == CardType.Creature) && (user.Power >= c.Price) && (c.MaxActions > 0)
			);
			Assert.NotNull(card);
			var buyAction = new BuyCreatureAction(user.Name, user.HandSet.IndexOf(card), 0);
			buyAction.Apply(state);
			Assert.True(user.TableSet[0].Actions == 0);
		}

		[Fact]
		public void ActiveRestoredOnNextTurn() {
			var state = Common.GameState;
			var user  = state.Users[0];
			var card  = user.HandSet.Find(
				c => (c.Type == CardType.Creature) && (user.Power >= c.Price) && (c.MaxActions > 0)
			);
			var buyAction = new BuyCreatureAction(user.Name, user.HandSet.IndexOf(card), 0);
			buyAction.Apply(state);
			new TurnAction("1").Apply(state);
			new TurnAction("2").Apply(state);
			Assert.True(user.TableSet[0].Actions > 0);
			Assert.True(user.TableSet[0].Actions == user.TableSet[0].MaxActions);
		}
	}
}
