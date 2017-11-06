using SharedLibrary.Models;

namespace SharedLibrary.Actions {
	public class AttackCreatureAction : BaseGameAction {
		public int DealerIndex { get; set; }
		public int VictimIndex { get; set; }

		public AttackCreatureAction() {}

		public AttackCreatureAction(string user, int dealerIndex, int victimIndex) {
			User        = user;
			DealerIndex = dealerIndex;
			VictimIndex = victimIndex;
		}

		public override bool CanApply(GameState state) {
			if ( base.CanApply(state) ) {
				var user = FindCurrentPlayer(state);
				if ( (DealerIndex >= 0) && (DealerIndex < user.TableSet.Count) ) {
					var dealer = user.TableSet[DealerIndex];
					if ( dealer != null ) {
						if ( dealer.Actions > 0 ) {
							var anotherUser = FindAnotherPlayer(state);
							if ( (VictimIndex >= 0) && (VictimIndex < anotherUser.TableSet.Count) ) {
								var vicim = anotherUser.TableSet[VictimIndex];
								if ( vicim != null ) {
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		protected override void ApplyInternal(GameState state) {
			var dealer = FindCurrentPlayer(state).TableSet[DealerIndex];
			var victim = FindAnotherPlayer(state).TableSet[VictimIndex];
			victim.Health -= dealer.Damage;
			dealer.Actions--;
		}
	}
}
