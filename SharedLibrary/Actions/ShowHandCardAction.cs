﻿using SharedLibrary.Models;
using SharedLibrary.Models.Game;

namespace SharedLibrary.Actions {
	public class ShowHandCardAction : BaseGameAction, ILimitedAction {
		public string    LimitedUser { get; private set; }
		public CardState Card        { get; private set; }

		public ShowHandCardAction() {}

		public ShowHandCardAction(string user, CardState card) {
			LimitedUser = User = user;
			Card        = card;
		}

		protected override void ApplyInternal(GameState state) {
			var user = FindCurrentPlayer(state);
			if ( user != null ) {
				var hiddenCard = user.HandSet.Find(c => c.Type == CardType.Hidden);
				if ( hiddenCard != null ) {
					user.HandSet[user.HandSet.IndexOf(hiddenCard)] = Card;
				}
			}
		}
	}
}
