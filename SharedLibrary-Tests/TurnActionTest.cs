using SharedLibrary.Common;
using SharedLibrary.Models;
using SharedLibrary.Actions;
using Xunit;

namespace SharedLibrary_Tests {
	public class TurnActionTest {
		[Fact]
		public void TurnOwnerOnFirstPlace() {
			var state = new GameState(Common.Users, "2");
			Assert.Same("2", state.Users[0].Name);
		}

		[Fact]
		public void UserCanChangeTurn() {
			var state  = Common.GameState;
			var action = new TurnAction("1");
			Assert.True(state.TryApply(action));
			Assert.Same("2", state.TurnOwner);
		}

		[Fact]
		public void TurnActionChangeTurnValue() {
			var state = Common.GameState;
			Assert.Equal(0, state.Turn);
			var action = new TurnAction("1");
			Assert.True(state.TryApply(action));
			Assert.Equal(0, state.Turn);
			action = new TurnAction("2");
			Assert.True(state.TryApply(action));
			Assert.Equal(1, state.Turn);
		}

		[Fact]
		public void WrongUserCantChangeTurn() {
			var state  = Common.GameState;
			var action = new TurnAction("2");
			Assert.False(state.TryApply(action));
			Assert.Same("1", state.TurnOwner);
		}
	}
}
