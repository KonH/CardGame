using SharedLibrary.Models;

namespace SharedLibrary.Actions {
	public class TurnAction : BaseGameAction {
		public TurnAction() {}

		public override void Apply(GameState state) {
			var user = state.Users.Find(u => u.Name != state.TurnOwner);
			if ( user != null ) {
				state.TurnOwner = user.Name;
			}
		}
	}
}
