using SharedLibrary.Models;

namespace SharedLibrary.Actions {
	public abstract class BaseGameAction : IGameAction {
		public virtual bool CanApply(GameState state) {
			return true;
		}

		public abstract void Apply(GameState state);
	}
}
