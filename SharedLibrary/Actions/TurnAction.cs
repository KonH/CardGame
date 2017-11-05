using SharedLibrary.Common;
using SharedLibrary.Models;

namespace SharedLibrary.Actions {
	public class TurnAction : BaseGameAction, IExpandCardAction {
		public string ExpandUser  { get; private set; }
		public bool   ExpandHand  { get { return true ; } }
		public bool   ExpandTable { get { return false; } }

		public TurnAction() {}

		public TurnAction(string user) {
			User = user;
		}

		public override void Apply(GameState state) {
			var user = state.Users.Find(u => u.Name != state.TurnOwner);
			if ( user != null ) {
				state.TurnOwner = user.Name;
				if ( state.Users.IndexOf(user) == 0 ) {
					state.Turn++;
				}
				if ( state.Turn > 0 ) {
					user.MaxPower++;
					if ( user.HandSet.Count < GameRules.MaxHandSet ) {
						var result = user.TryGetCard();
						if ( result ) {
							ExpandUser = user.Name;
						}
					}
				}
				user.Power = user.MaxPower;
			}
		}
	}
}
