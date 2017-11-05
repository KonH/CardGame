using System;
using System.Linq;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;
using SharedLibrary.Actions;
using Server.Models;

namespace Server.Repositories {
	public class GameRepository {
		ILogger                                       _logger;
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
				if ( _games.TryAdd(game.Session, game) ) {
					_logger.LogDebug(
						"Game for session '{0}' is created (first turn to: '{1}')", 
						game.Session, game.SharedState.TurnOwner);
					TryAddBotAction(game);
					return true;
				}
			} catch (Exception e) {
				_logger.LogWarning("TryAdd: {0}", e);
			}
			return false;
		}

		public ServerGameState Find(string session) {
			if ( session != null ) {
				if ( _games.TryGetValue(session, out ServerGameState value) ) {
					return value;
				}
			}
			return null;
		}

		public bool TryApplyAction(ServerGameState state, IGameAction action) {
			if ( state != null ) {
				var gameState = state.SharedState;
				if ( gameState.TryApply(action) ) {
					AddAction(state, action);
					return true;
				}
			}
			return false;
		}

		void AddAction(ServerGameState state, IGameAction action) {
			state.Actions.Add(action);
			_logger.LogDebug(
				"[{0}] User: '{1}', action: {2} (state v.{3})",
				state.Session, action.User, action.ToString(), state.SharedState.Version);
			TryAddShowCardAction(state, action);
			TryAddBotAction(state);
		}

		void TryAddShowCardAction(ServerGameState state, IGameAction action) {
			var expandAction = action as IExpandCardAction;
			if ( (expandAction != null) && (!string.IsNullOrEmpty(expandAction.ExpandUser)) ) {
				state.SharedState.Version++;
				var card = state.SharedState.Users.Find(u => u.Name == expandAction.ExpandUser).HandSet.Last();
				_logger.LogDebug(
					"[{0}] ExpandCardAction: user: '{1}' (hand: {2}), card: {3}",
					state.Session, expandAction.ExpandUser, expandAction.ExpandHand, card.Type);
				if ( expandAction.ExpandHand ) {
					state.Actions.Add(new ShowHandCardAction(expandAction.ExpandUser, card));
				}
				if ( expandAction.ExpandTable ) {
					state.Actions.Add(new ShowTableCardAction(expandAction.ExpandUser, card));
				}
			}
		}

		void TryAddBotAction(ServerGameState state) {
			if ( (state != null) && (state.BotUser != null) ) {
				var gameState = state.SharedState;
				var botAction = state.BotUser.GetAction(gameState);
				if ( botAction != null ) {
					TryApplyAction(state, botAction);
				}
			}
		}

		public IGameAction FilterAction(IGameAction action, string user) {
			var limitedAction = action as ILimitedAction;
			if ( limitedAction != null ) {
				if ( user != limitedAction.LimitedUser ) {
					return new EmptyAction();
				}
			}
			return action;
		}
	}
}
