using SharedLibrary.Models;

namespace SharedLibrary.Actions {
	public interface IGameAction {
		string User { get; set; }
		bool CanApply(GameState state);
		void Apply(GameState state);
	}
}
