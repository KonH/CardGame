using SharedLibrary.Common;
using SharedLibrary.Utils;
using System.Collections.Generic;

namespace SharedLibrary.Models.Game {
	public class UserState {
		public string          Name      { get; private set; }
		public int             Health    { get; set; }
		public int             MaxHealth { get; private set; }
		public int             Power     { get; set; }
		public int             MaxPower  { get; set; }
		public List<CardState> TableSet  { get; private set; }
		public List<CardState> HandSet   { get; private set; }
		public List<CardState> GlobalSet { get; private set; }

		public UserState(string name, int health, int power) :
			this(
				name,
				health, health,
				power, power,
				table : new List<CardState>().Fill(null, GameRules.MaxTableSet), 
				hand  : new List<CardState>(),
				global: new List<CardState>())
		{
			// TODO: Use concrete set
			GlobalSet.Fill(() => 
				new CardState(CardType.Creature)
				.WithPrice(1)
				.WithHealth(1)
				.WithDamage(1)
				.WithActions(0, 1), 
			10);
		}

		public UserState(
			string          name,
			int             health,
			int             maxHealth,
			int             power,
			int             maxPower,
			List<CardState> table,
			List<CardState> hand,
			List<CardState> global)
		{
			Name      = name;
			Health    = health;
			MaxHealth = maxHealth;
			Power     = power;
			MaxPower  = maxPower;
			TableSet  = table;
			HandSet   = hand;
			GlobalSet = global;
		}

		public bool TryGetCard() {
			if ( GlobalSet.Count > 0 ) {
				var firstCard = GlobalSet[0];
				GlobalSet.RemoveAt(0);
				HandSet.Add(firstCard);
				return true;
			}
			return false;
		}
	}
}
