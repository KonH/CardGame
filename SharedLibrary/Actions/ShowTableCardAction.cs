using SharedLibrary.Models;
using SharedLibrary.Models.Game;

namespace SharedLibrary.Actions {
	public class ShowTableCardAction : BaseGameAction, ILimitedAction {
		public string    LimitedUser { get; private set; }
		public CardState Card        { get; private set; }

		public ShowTableCardAction() {}

		public ShowTableCardAction(string user, CardState card) {
			LimitedUser = User = user;
			Card        = card;
		}

		protected override void ApplyInternal(GameState state) {
			var user = FindCurrentPlayer(state);
			if ( user != null ) {
				var hiddenCard = user.TableSet.Find(c => c.Type == CardType.Hidden);
				if ( hiddenCard != null ) {
					user.TableSet[user.TableSet.IndexOf(hiddenCard)] = Card;
				}
			}
		}
	}
}
