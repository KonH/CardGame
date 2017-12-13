using System.Collections.Generic;
using SharedLibrary.AI;
using SharedLibrary.Common;
using SharedLibrary.Models;
using SharedLibrary.Models.Game;

namespace SharedLibrary_Tests {
    static class Common {
		public static ICollection<UserState> Users {
			get {
				return new List<UserState>().With("1", 1, 1).With("2", 1, 1);
			}
		}
		public static GameState GameState {
			get {
				return new GameState(Users, "1");
			}
		}

		public static (GameState, GameAI) GameStateWithBot {
			get {
				var state = GameState;
				var bot = new GameAI(state.Users[1].Name);
				return (state, bot);
			}
		}
	}
}
