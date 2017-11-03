using System;
using System.Linq;
using System.Collections.Generic;
using Server.Models;
using SharedLibrary.Models;
using SharedLibrary.Models.Game;
using SharedLibrary.AI;

namespace Server.Helpers {
	public class GameBuilder {
		string _session;
		IEnumerable<UserState> _users;
		string _turnOwner;
		GameAI _bot;

		public GameBuilder(string session, IEnumerable<string> users, int defaultHealth) {
			_session = session;
			_users = users.Select(name => new UserState(name, defaultHealth)).ToList();
		}

		public GameBuilder WithTurnOwner(string userName) {
			if ( !_users.Any(u => u.Name == userName) ) {
				throw new InvalidOperationException("TurnOwner is not presented in Users");
			}
			_turnOwner = userName;
			return this;
		}

		public GameBuilder WithBot(string userName) {
			if ( !_users.Any(u => u.Name == userName) ) {
				throw new InvalidOperationException("Bot is not presented in Users");
			}
			_bot = new GameAI(userName);
			return this;
		}

		public ServerGameState Build() {
			var gameState = new GameState(_users, _turnOwner);
			var serverGameState = new ServerGameState(_session, gameState, _bot);
			return serverGameState;
		}
	}
}
