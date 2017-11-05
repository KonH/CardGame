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

		public override void Apply(GameState state) {
			var user = state.Users.Find(u => u.Name == User);
			if ( user != null ) {
				var hiddenCard = user.TableSet.Find(c => c.Type == CardType.Hidden);
				if ( hiddenCard != null ) {
					user.TableSet[user.TableSet.IndexOf(hiddenCard)] = Card;
				}
			}
		}
	}
}
