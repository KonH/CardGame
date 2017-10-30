﻿using System.Collections.Generic;
using SharedLibrary.Models.Game;

namespace SharedLibrary.Models {
	public class GameState {
		public string Session { get; private set; }
		public int Version { get; set; }
		public List<UserState> Users { get; private set; }

		public GameState() {}

		public GameState(string session, IEnumerable<UserState> users) {
			Session = session;
			Users = new List<UserState>(users);
		}
	}
}
