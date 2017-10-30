using SharedLibrary.Actions;
using SharedLibrary.Models;

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
	}
}
