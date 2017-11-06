using SharedLibrary.Models;
using SharedLibrary.Models.Game;

namespace SharedLibrary.Actions {
	public abstract class BaseGameAction : IGameAction {
		public string User { get; set; }

		public virtual bool CanApply(GameState state) {
			return User == state.TurnOwner;
		}

		public void Apply(GameState state) {
			ApplyInternal(state);
			PostActionChecks(state);
		}

		protected abstract void ApplyInternal(GameState state);

		void PostActionChecks(GameState state) {
			foreach ( var user in state.Users ) {
				for ( var i = 0; i < user.TableSet.Count; i++ ) {
					var card = user.TableSet[i];
					if ( (card != null) && (card.Health <= 0) ) {
						user.TableSet[i] = null;
					}
				}
			}
		}

		protected UserState FindCurrentPlayer(GameState state) {
			return state.Users.Find(u => u.Name == User);
		}

		protected UserState FindAnotherPlayer(GameState state) {
			return state.Users.Find(u => u.Name != User);
		}
	}
}
