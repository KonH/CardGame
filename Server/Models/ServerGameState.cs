using System.Collections.Generic;
using SharedLibrary.AI;
using SharedLibrary.Models;
using SharedLibrary.Actions;

namespace Server.Models {
	public class ServerGameState {
		public string            Session     { get; private set; }
		public GameState         SharedState { get; private set; }
		public GameAI            BotUser     { get; private set; }
		public List<IGameAction> Actions     { get; private set; }

		public ServerGameState(string session, GameState sharedState, GameAI botUser) {
			Session     = session;
			SharedState = sharedState;
			BotUser     = botUser;
			Actions     = new List<IGameAction>();
		}
	}
}
