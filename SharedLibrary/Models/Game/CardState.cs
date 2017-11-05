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

		// TODO: Find way to disable serialization with ability to use in Unity client
		public int Price {
			get {
				return GetStatValue("Price");
			}
		}

		public int Health {
			get {
				return GetStatValue("Health");
			}
		}

		public int MaxHealth {
			get {
				return GetStatValue("MaxHealth");
			}
		}

		public int Damage {
			get {
				return GetStatValue("Damage");
			}
		}

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
			Stats.Add("Health", value);
			Stats.Add("MaxHealth", value);
			return this;
		}

		public CardState WithDamage(int value) {
			Stats.Add("Damage", value);
			return this;
		}
	}
}
