using SharedLibrary.Models;

namespace SharedLibrary.Actions {
	public class AttackPlayerAction : BaseGameAction {
		public int DealerIndex { get; set; }

		public AttackPlayerAction() {}

		public AttackPlayerAction(string user, int dealerIndex) {
			User        = user;
			DealerIndex = dealerIndex;
		}

		public override bool CanApply(GameState state) {
			if ( base.CanApply(state) ) {
				var user = FindCurrentPlayer(state);
				if ( (DealerIndex >= 0) && (DealerIndex < user.TableSet.Count) ) {
					var dealer = user.TableSet[DealerIndex];
					if ( dealer != null ) {
						return dealer.Actions > 0;
					}
				}
			}
			return false;
		}

		protected override void ApplyInternal(GameState state) {
			var dealer = FindCurrentPlayer(state).TableSet[DealerIndex];
			var victim = FindAnotherPlayer(state);
			victim.Health -= dealer.Damage;
			dealer.Actions--;
		}
	}
}
