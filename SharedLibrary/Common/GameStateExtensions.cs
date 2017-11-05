using System.Collections.Generic;
using SharedLibrary.Models;
using SharedLibrary.Actions;
using SharedLibrary.Models.Game;

namespace SharedLibrary.Common {
	public static class GameStateExtensions {
		public static bool TryApply(this GameState state, IGameAction action) {
			if ( (action != null) && action.CanApply(state) ) {
				state.Version++;
				action.Apply(state);
				return true;
			}
			return false;
		}

		public static ICollection<UserState> With(this ICollection<UserState> users, string name, int health) {
			users.Add(new UserState(name, health));
			return users;
		}
	}
}
