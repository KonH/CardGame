using SharedLibrary.Actions;
using Xunit;

namespace SharedLibrary_Tests {
    public class PowerTest {
		[Fact]
		public void UsersHasPower() {
			var state = Common.GameState;
			Assert.True(state.Users.TrueForAll(u => (u.Power > 0) && (u.MaxPower > 0)));
		}

		[Fact]
		public void UsersPowerIsRestored() {
			var state = Common.GameState;
			var user = state.Users[0];
			var startPower = user.Power;
			user.Power--;
			new TurnAction("1").Apply(state);
			new TurnAction("2").Apply(state);
			Assert.True((user.Power >= startPower) && (user.Power == user.MaxPower));
		}

		[Fact]
		public void UsersMaxPowerIsIncresed() {
			var state = Common.GameState;
			var user = state.Users[0];
			var startMaxPower = user.MaxPower;
			new TurnAction("1").Apply(state);
			new TurnAction("2").Apply(state);
			Assert.True(user.MaxPower > startMaxPower);
		}
	}
}
