using Xunit;
using System.Collections.Generic;
using SharedLibrary.Common;
using SharedLibrary.Actions;
using SharedLibrary.Models;
using SharedLibrary.Models.Game;

namespace SharedLibrary_Tests {
	public class TurnActionTest {
		[Fact]
		public void UserCanChangeTurn() {
			var state = new GameState(new List<UserState>() { new UserState("1", 1), new UserState("2", 1) }, "1" );
			var action = new TurnAction() { User = "1" };
			Assert.True(state.TryApply(action));
			Assert.Same("2", state.TurnOwner);
		}

		[Fact]
		public void WrongUserCantChangeTurn() {
			var state = new GameState(new List<UserState>() { new UserState("1", 1), new UserState("2", 1) }, "1");
			var action = new TurnAction() { User = "2" };
			Assert.False(state.TryApply(action));
			Assert.Same("1", state.TurnOwner);
		}
	}
}
