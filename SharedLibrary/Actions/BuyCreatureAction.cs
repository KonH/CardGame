using SharedLibrary.Models;

namespace SharedLibrary.Actions {
	public class BuyCreatureAction : BaseGameAction, IExpandCardAction {
		public string ExpandUser  { get; private set; }
		public bool   ExpandHand  { get { return false; } }
		public bool   ExpandTable { get { return true ; } }

		public int HandIndex  { get; set; }
		public int TableIndex { get; set; }

		public BuyCreatureAction() {}

		public BuyCreatureAction(string user, int handIndex, int tableIndex) {
			User       = user;
			HandIndex  = handIndex;
			TableIndex = tableIndex;
		}

		public override bool CanApply(GameState state) {
			if ( base.CanApply(state) ) {
				var user = state.Users.Find(u => u.Name == User);
				if ( user != null ) {
					if ( (HandIndex >= 0) && (HandIndex < user.HandSet.Count) ) {
						var card = user.HandSet[HandIndex];
						if ( card.Price <= user.Power ) {
							if ( (TableIndex >= 0) && (TableIndex < user.TableSet.Count) ) {
								var tablePos = user.TableSet[TableIndex];
								if ( tablePos == null ) {
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		public override void Apply(GameState state) {
			var user = state.Users.Find(u => u.Name == User);
			var card = user.HandSet[HandIndex];
			user.Power -= card.Price;
			user.HandSet.Remove(card);
			user.TableSet[TableIndex] = card;
		}
	}
}
