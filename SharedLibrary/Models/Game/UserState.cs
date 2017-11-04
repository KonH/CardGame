using System.Collections.Generic;

namespace SharedLibrary.Models.Game {
	public class UserState {
		public string Name { get; private set; }
		public int Health { get; set; }
		public int MaxHealth { get; private set; }
		public List<CardState> TableSet { get; private set; }
		public List<CardState> HandSet { get; private set; }
		public List<CardState> GlobalSet { get; private set; }

		public UserState(string name, int health) {
			Name = name;
			MaxHealth = Health = health;
			TableSet = new List<CardState>();
			HandSet = new List<CardState>();
			GlobalSet = new List<CardState>();
			// TODO: Use concrete set
			for ( var i = 0; i < 10; i++ ) {
				GlobalSet.Add(new CardState() { Type = CardType.Creature });
			}
		}

		public void TryGetCard() {
			if ( GlobalSet.Count > 0 ) {
				var firstCard = GlobalSet[0];
				GlobalSet.RemoveAt(0);
				HandSet.Add(firstCard);
			}
		}
	}
}
