using SharedLibrary.Actions;
using SharedLibrary.Common;
using Xunit;

namespace SharedLibrary_Tests {
    public class CardSpawnTest {
		[Fact]
		public void StartSetIsAdded() {
			var state = Common.GameState;
			Assert.True(state.Users.TrueForAll(u => u.HandSet.Count == GameRules.StartupHandSet));
		}

		[Fact]
		public void CardAddedOnNextTurn() {
			var state = Common.GameState;
			var u1hand = state.Users[0].HandSet;
			var u2hand = state.Users[1].HandSet;
			var u1StartCount = u1hand.Count;
			var u2StartCount = u2hand.Count;
			// turn: u1, 0
			state.TryApply(new TurnAction("1"));
			// turn: u2, 0
			Assert.Equal(u2StartCount, u2hand.Count);
			state.TryApply(new TurnAction("2"));
			// turn: u1, 1
			Assert.Equal(u1StartCount + 1, u1hand.Count);
			state.TryApply(new TurnAction("1"));
			// turn: u2, 1
			Assert.Equal(u2StartCount + 1, u2hand.Count);
		}

		[Fact]
		public void CardsNotOverspawned() {
			var state = Common.GameState;
			var turns = GameRules.MaxHandSet - GameRules.StartupHandSet + 1;
			while ( state.Turn < turns ) {
				state.TryApply(new TurnAction(state.TurnOwner));
				Assert.True(state.Users.TrueForAll(u => u.HandSet.Count <= GameRules.MaxHandSet));
			}
		}
	}
}
