using System.Collections.Generic;
using SharedLibrary.Models.Game;
using SharedLibrary.Common;

namespace SharedLibrary.Models {
	public class GameState {
		public string TurnOwner { get; set; }
		public int Version { get; set; }
		public int Turn { get; set; }
		public List<UserState> Users { get; private set; }

		public GameState() {}

		public GameState(IEnumerable<UserState> users, string turnOwner) {
			Users = new List<UserState>();
			TurnOwner = turnOwner;
			SetupUsers(users);
			PrepareStartSets();
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
				for ( var i = 0; i < GameRules.StartupHandSet; i++ ) {
					user.TryGetCard();
				}
			}
		}
	}
}
