using SharedLibrary.Models;

namespace SharedLibrary.Actions {
	public class EmptyAction : BaseGameAction {

		public EmptyAction() {}

		public override bool CanApply(GameState state) {
			return true;
		}

		public override void Apply(GameState state) { }
	}
}
