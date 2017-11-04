using SharedLibrary.Models;
using SharedLibrary.Models.Game;

namespace SharedLibrary.Actions {
	public class ShowHandCardAction : BaseGameAction, ILimitedAction {
		public string LimitedUser { get; private set; }
		public CardState Card { get; private set; }

		public ShowHandCardAction() {}

		public ShowHandCardAction(string user, CardState card) {
			LimitedUser = User = user;
			Card = card;
		}

		public override void Apply(GameState state) {
			var user = state.Users.Find(u => u.Name == User);
			if ( user != null ) {
				var hiddenCard = user.HandSet.Find(c => c.Type == CardType.Hidden);
				if ( hiddenCard != null ) {
					user.HandSet[user.HandSet.IndexOf(hiddenCard)] = Card;
				}
			}
		}
	}
}
