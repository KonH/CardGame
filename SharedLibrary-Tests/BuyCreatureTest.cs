using SharedLibrary.Actions;
using SharedLibrary.Common;
using Xunit;

namespace SharedLibrary_Tests {
    public class BuyCreatureTest {
		[Fact]
		public void UserCanBuyCreature() {
			var state = Common.GameState;
			var user  = state.Users[0];
			var card  = user.HandSet.Find(c => c.Price <= user.Power);
			Assert.NotNull(card);
			var action = new BuyCreatureAction(user.Name, user.HandSet.IndexOf(card), 0);
			Assert.True(action.CanApply(state));
		}

		[Fact]
		public void CreatureIsPlacedOnTable() {
			var state  = Common.GameState;
			var user   = state.Users[0];
			var card   = user.HandSet.Find(c => c.Price <= user.Power);
			var action = new BuyCreatureAction(user.Name, user.HandSet.IndexOf(card), 0);
			Assert.True(action.CanApply(state));
			action.Apply(state);
			Assert.True((user.TableSet.Count > 0) && (user.TableSet[0] == card));
		}

		[Fact]
		public void CreatureIsRemovedFromHand() {
			var state  = Common.GameState;
			var user   = state.Users[0];
			var card   = user.HandSet.Find(c => c.Price <= user.Power);
			var action = new BuyCreatureAction(user.Name, user.HandSet.IndexOf(card), 0);
			Assert.True(action.CanApply(state));
			action.Apply(state);
			Assert.False(user.HandSet.Contains(card));
		}

		[Fact]
		public void PowerIsSpent() {
			var state      = Common.GameState;
			var user       = state.Users[0];
			var card       = user.HandSet.Find(c => (c.Price > 0) && (c.Price <= user.Power));
			var startPower = user.Power;
			var action     = new BuyCreatureAction(user.Name, user.HandSet.IndexOf(card), 0);
			Assert.True(action.CanApply(state));
			action.Apply(state);
			Assert.True(user.Power == (startPower - card.Price));
		}

		[Fact]
		public void CantBuyWithoutPower() {
			var state  = Common.GameState;
			var user   = state.Users[0];
			var card   = user.HandSet.Find(c => c.Price <= user.Power);
			user.Power = 0;
			var action = new BuyCreatureAction(user.Name, user.HandSet.IndexOf(card), 0);
			Assert.False(action.CanApply(state));
		}

		[Fact]
		public void CantBuyNotExistCard() {
			var state  = Common.GameState;
			var user   = state.Users[0];
			var action = new BuyCreatureAction(user.Name, -1, 0);
			Assert.False(action.CanApply(state));
			action = new BuyCreatureAction(user.Name, user.HandSet.Count, 0);
			Assert.False(action.CanApply(state));
		}

		[Fact]
		public void CantBuyToWrongPosition() {
			var state  = Common.GameState;
			var user   = state.Users[0];
			var card   = user.HandSet.Find(c => c.Price <= user.Power);
			var action = new BuyCreatureAction(user.Name, user.HandSet.IndexOf(card), -1);
			Assert.False(action.CanApply(state));
			action = new BuyCreatureAction(user.Name, user.HandSet.IndexOf(card), GameRules.MaxTableSet);
			Assert.False(action.CanApply(state));
		}

		[Fact]
		public void CantBuyToBusyPosition() {
			var state = Common.GameState;
			var user  = state.Users[0];
			var card  = user.HandSet.Find(c => c.Price <= user.Power);
			Assert.NotNull(card);
			var action = new BuyCreatureAction(user.Name, user.HandSet.IndexOf(card), 0);
			Assert.True(action.CanApply(state));
			action.Apply(state);
			user.Power = user.MaxPower;
			card       = user.HandSet.Find(c => c.Price <= user.Power);
			action     = new BuyCreatureAction(user.Name, user.HandSet.IndexOf(card), 0);
			Assert.False(action.CanApply(state));
		}
	}
}
