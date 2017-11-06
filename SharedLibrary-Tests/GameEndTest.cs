using Xunit;

namespace SharedLibrary_Tests {
    public class GameEndTest {
		[Fact]
		public void GameIsEnded() {
			var state = Common.GameState;
			var user  = state.Users[0];
			user.Health = 0;
			Assert.True(state.IsEnded);
			Assert.True(state.Winner == state.Users[1].Name);
		}
	}
}
