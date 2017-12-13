using System.Collections.Generic;

namespace SharedLibrary.Models.Game {
	public class CardState {
		public CardType                Type  { get; private set; }
		public Dictionary<string, int> Stats { get; private set; }

		int GetStatValue(string name) {
			var value = 0;
			Stats.TryGetValue(name, out value);
			return value;
		}

		void SetStatValue(string name, int value) {
			if ( Stats.ContainsKey(name) ) {
				Stats[name] = value;
			} else {
				Stats.Add(name, value);
			}
		}

		// TODO: Find way to disable serialization with ability to use in Unity client
		public int Price      { get { return GetStatValue("Price");      } set { SetStatValue("Price",      value); } }
		public int Health     { get { return GetStatValue("Health");     } set { SetStatValue("Health",     value); } }
		public int MaxHealth  { get { return GetStatValue("MaxHealth");  } set { SetStatValue("MaxHealth",  value); } }
		public int Damage     { get { return GetStatValue("Damage");     } set { SetStatValue("Damage",     value); } }
		public int Actions    { get { return GetStatValue("Actions");    } set { SetStatValue("Actions",    value); } }
		public int MaxActions { get { return GetStatValue("MaxActions"); } set { SetStatValue("MaxActions", value); } }

		public CardState() {}

		public CardState(CardType type) {
			Type  = type;
			Stats = new Dictionary<string, int>();
		}

		public CardState WithPrice(int value) {
			Stats.Add("Price", value);
			return this;
		}

		public CardState WithHealth(int value) {
			Stats.Add("Health",    value);
			Stats.Add("MaxHealth", value);
			return this;
		}

		public CardState WithDamage(int value) {
			Stats.Add("Damage", value);
			return this;
		}

		public CardState WithActions(int start, int max) {
			Stats.Add("Actions",    start);
			Stats.Add("MaxActions", max);
			return this;
		}

		public CardState Clone() {
			var state = new CardState(Type);
			state.Stats = new Dictionary<string, int>(Stats);
			return state;
		}
	}
}
