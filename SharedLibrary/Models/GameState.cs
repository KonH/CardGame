using System.Collections.Generic;
using SharedLibrary.Models.Game;

namespace SharedLibrary.Models {
	public class GameState {
		public string TurnOwner { get; set; }
		public int Version { get; set; }
		public List<UserState> Users { get; private set; }

		public GameState() {}

		public GameState(IEnumerable<UserState> users, string turnOwner) {
			Users = new List<UserState>(users);
			TurnOwner = turnOwner;
		}
	}
}
