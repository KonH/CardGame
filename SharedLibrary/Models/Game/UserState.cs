namespace SharedLibrary.Models.Game {
	public class UserState {
		public string Name { get; private set; }
		public int Health { get; set; }
		public int MaxHealth { get; private set; }

		public UserState(string name, int health) {
			Name = name;
			MaxHealth = Health = health;
		}
	}
}
