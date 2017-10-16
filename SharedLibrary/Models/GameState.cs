using SharedLibrary.Models.Game;
using System.Collections.Generic;

namespace SharedLibrary.Models {
	public class GameState {
		public string Session;
		public int Version;
		public List<UserState> Users = new List<UserState>();
	}
}
