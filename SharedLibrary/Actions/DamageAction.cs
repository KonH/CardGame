using SharedLibrary.Models;

namespace SharedLibrary.Actions {
	public class DamageAction : BaseGameAction {
		public string VictimUser;
		public int    Count;

		public DamageAction() {}

		public DamageAction(string victimUser, int count) {
			VictimUser = victimUser;
			Count      = count;
		}

		public override void Apply(GameState state) {
			var user = state.Users.Find(u => u.Name == VictimUser);
			if ( user != null ) {
				user.Health -= Count;
			}
		}
	}
}
