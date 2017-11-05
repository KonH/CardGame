using SharedLibrary.Models;

namespace SharedLibrary.Actions {
	public abstract class BaseGameAction : IGameAction {
		public string User { get; set; }

		public virtual bool CanApply(GameState state) {
			return User == state.TurnOwner;
		}

		public abstract void Apply(GameState state);
	}
}
