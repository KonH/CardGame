using SharedLibrary.Common;
using SharedLibrary.Actions;
using Xunit;

namespace SharedLibrary_Tests {
	public class AITest {
		[Fact]
		public void BotCanBuy() {
			var (state, bot) = Common.GameStateWithBot;
			new TurnAction(state.Users[0].Name).Apply(state);
			var botAction = bot.GetAction(state);
			Assert.NotNull(botAction);
			Assert.IsType(typeof(BuyCreatureAction), botAction);
		}

		[Fact]
		public void BotCantBuyNotExistingCard() {
			var (state, bot) = Common.GameStateWithBot;
			new TurnAction(state.Users[0].Name).Apply(state);
			state.Users[1].HandSet.Clear();
			var botAction = bot.GetAction(state);
			Assert.NotNull(botAction);
			Assert.IsNotType(typeof(BuyCreatureAction), botAction);
		}

		[Fact]
		public void BotCanAttackPlayer() {
			var (state, bot) = Common.GameStateWithBot;
			new TurnAction(state.Users[0].Name).Apply(state);
			while ( true ) {
				var botAction = bot.GetAction(state);
				if ( botAction != null ) {
					Assert.True(state.TryApply(botAction));
				} else {
					break;
				}
			}
			new TurnAction(state.Users[0].Name).Apply(state);
			var attackActionIsFound = false;
			while ( true ) {
				var botAction = bot.GetAction(state);
				if ( botAction != null ) {
					Assert.True(state.TryApply(botAction));
					if ( botAction is AttackPlayerAction ) {
						attackActionIsFound = true;
					}
				} else {
					break;
				}
			}
			Assert.True(attackActionIsFound);
		}
	}
}
