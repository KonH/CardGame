using SharedLibrary.Actions;
using SharedLibrary.Models;

namespace SharedLibrary.AI {
	public class GameAI {
		string _userName;

		public GameAI(string userName) {
			_userName = userName;
		} 

		public IGameAction GetAction(GameState state) {
			if ( state.TurnOwner == _userName ) {
				return new TurnAction(_userName);
			}
			return null;
		}
	}
}
