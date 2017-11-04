using SharedLibrary.Common;
using SharedLibrary.Models;
using SharedLibrary.Models.Game;

namespace SharedLibrary.Actions {
	public class TurnAction : BaseGameAction {
		public TurnAction() {}

		public override void Apply(GameState state) {
			var user = state.Users.Find(u => u.Name != state.TurnOwner);
			if ( user != null ) {
				state.TurnOwner = user.Name;
				if ( state.Users.IndexOf(user) == 0 ) {
					state.Turn++;
				}
				if ( state.Turn > 0 ) {
					if ( user.HandSet.Count < GameRules.MaxHandSet ) {
						user.TryGetCard();
					}
				}
			}
		}
	}
}
