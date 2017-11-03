using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;
using SharedLibrary.Actions;
using Server.Models;

namespace Server.Repositories {
	public class GameRepository {
		ILogger _logger;
		ConcurrentDictionary<string, ServerGameState> _games = new ConcurrentDictionary<string, ServerGameState>();

		public GameRepository(ILogger<GameRepository> logger) {
			_logger = logger;
		}

		public bool TryAdd(ServerGameState game) {
			if ( (game == null) || (string.IsNullOrEmpty(game.Session)) ) {
				return false;
			}
			if ( _games.ContainsKey(game.Session) ) {
				return false;
			}
			try {
				_games.TryAdd(game.Session, game);
				TryAddBotAction(game);
				return true;
			} catch {
				return false;
			}
		}

		public ServerGameState Find(string session) {
			if ( session != null ) {
				if ( _games.TryGetValue(session, out ServerGameState value) ) {
					return value;
				}
			}
			return null;
		}

		public void TryAddBotAction(ServerGameState state) {
			if ( (state != null) && (state.BotUser != null) ) {
				var gameState = state.SharedState;
				var botAction = state.BotUser.GetAction(gameState);
				if ( botAction != null ) {
					if ( gameState.TryApply(botAction) ) {
						AddAction(state, botAction);
					}
				}
			}
		}

		public void AddAction(ServerGameState state, IGameAction action) {
			if ( (state != null) && (action != null) ) {
				state.Actions.Add(action);
				_logger.LogDebug(
					"[{0}] User: '{1}', action: {2} (state v.{3})",
					state.Session, action.User, action.ToString(), state.SharedState.Version);
				TryAddBotAction(state);
			}
		}
	}
}
