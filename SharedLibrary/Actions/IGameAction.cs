using SharedLibrary.Models;

namespace SharedLibrary.Actions {
	public interface IGameAction {
		bool CanApply(GameState state);
		void Apply(GameState state);
	}
}
