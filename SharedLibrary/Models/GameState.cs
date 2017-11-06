using System.Collections.Generic;
using SharedLibrary.Common;
using SharedLibrary.Models.Game;

namespace SharedLibrary.Models {
	public class GameState {
		public string          TurnOwner { get; set; }
		public int             Version   { get; set; }
		public int             Turn      { get; set; }
		public List<UserState> Users     { get; private set; }

		public bool IsEnded {
			get {
				var liveUsers = 0;
				foreach ( var user in Users ) {
					if ( user.Health > 0 ) {
						liveUsers++;
					}
				}
				return liveUsers <= 1;
			}
		}

		public string Winner {
			get {
				if ( IsEnded ) {
					foreach ( var user in Users ) {
						if ( user.Health > 0 ) {
							return user.Name;
						}
					}
				}
				return null;
			}
		}

		public GameState() {}

		public GameState(IEnumerable<UserState> users, string turnOwner): this(turnOwner, 0, 0) {
			SetupUsers(users);
			PrepareStartSets();
		}

		GameState(string turnOwner, int version, int turn) {
			TurnOwner = turnOwner;
			Version   = version;
			Turn      = turn;
			Users     = new List<UserState>();
		}

		void SetupUsers(IEnumerable<UserState> users) {
			foreach ( var user in users ) {
				if ( user.Name == TurnOwner ) {
					Users.Insert(0, user);
				} else {
					Users.Add(user);
				}
			}
		}

		void PrepareStartSets() {
			foreach ( var user in Users ) {
				for ( var i = 0; i < GameRules.StartHandSet; i++ ) {
					user.TryGetCard();
				}
			}
		}

		List<CardState> HideCards(List<CardState> cards) {
			var newCards = new List<CardState>();
			for ( var i = 0; i < cards.Count; i++ ) {
				newCards.Add(new CardState(CardType.Hidden));
			}
			return newCards;
		}

		public GameState Filter(string userName) {
			var state = new GameState(TurnOwner, Version, Turn);
			foreach ( var user in Users ) {
				var table   = user.TableSet;
				var hand    = user.Name == userName ? user.HandSet : HideCards(user.HandSet);
				var global  = HideCards(user.GlobalSet);
				var newUser = new UserState(
					user.Name,
					user.Health,
					user.MaxHealth,
					user.Power,
					user.MaxPower,
					table,
					hand,
					global);
				state.Users.Add(newUser);
			}
			return state;
		}
	}
}
