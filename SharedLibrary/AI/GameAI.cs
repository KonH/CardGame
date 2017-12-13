using System.Linq;
using System.Collections.Generic;
using SharedLibrary.Models;
using SharedLibrary.Actions;
using SharedLibrary.Models.Game;

namespace SharedLibrary.AI {
	public class GameAI {
		class TempState {
			public IGameAction Action  { get; }
			public GameState   State   { get; }
			public int         Measure { get; }

			public TempState(GameState state, IGameAction action, string botName) {
				State = state.Clone();
				Action = action;
				Action.Apply(State);
				Measure = MeasureState(State, botName);
			}
		}

		string _userName;

		public GameAI(string userName) {
			_userName = userName;
		}

		public IGameAction GetAction(GameState state) {
			if ( state.TurnOwner == _userName ) {
				return SelectAction(state, FindAvailableActions(state));
			}
			return null;
		}

		static UserState FindUserState(GameState state, string userName) {
			return state.Users.Where(u => u.Name == userName).First();
		}

		static UserState FindEnemyState(GameState state, string userName) {
			return state.Users.Where(u => u.Name != userName).First();
		}

		static int FindUserPower(UserState userState) {
			return userState.Health - userState.Power;
		}

		static int FindTablePower(UserState userState) {
			return
				userState.TableSet.
				Where(c => c != null).
				Sum(c => c.Health + c.Damage);
		}

		static int MeasureState(GameState state, string userName) {
			var userState = FindUserState(state, userName);
			var userPower = FindUserPower(userState);
			var userTablePower = FindTablePower(userState);

			var enemyState = FindEnemyState(state, userName);
			var enemyPower = FindUserPower(enemyState);
			var enemyTablePower = FindTablePower(enemyState);

			var measure =
				(userPower + userTablePower) -
				(enemyPower + enemyTablePower);
			return measure;
		}

		List<IGameAction> FindAvailableActions(GameState state) {
			var actions = new List<IGameAction>();
			var userState = FindUserState(state, _userName);
			var userHandIndexes = Enumerable.Range(0, userState.HandSet.Count);
			var userTableIndexes = Enumerable.Range(0, userState.TableSet.Count);
			foreach ( var handIndex in userHandIndexes ) {
				foreach ( var tableIndex in userTableIndexes ) {
					var buyAction = new BuyCreatureAction(_userName, handIndex, tableIndex);
					if ( buyAction.CanApply(state) ) {
						actions.Add(buyAction);
					}
				}
			}
			var enemyState = FindEnemyState(state, _userName);
			var enemyTableIndexes = Enumerable.Range(0, enemyState.TableSet.Count);
			foreach ( var tableIndex in userTableIndexes ) {
				var attackPlayer = new AttackPlayerAction(_userName, tableIndex);
				if ( attackPlayer.CanApply(state) ) {
					actions.Add(attackPlayer);
				}
				foreach ( var victimIndex in enemyTableIndexes ) {
					var attackCreature = new AttackCreatureAction(_userName, tableIndex, victimIndex);
					if ( attackCreature.CanApply(state) ) {
						actions.Add(attackCreature);
					}
				}
			}
			actions.Add(new TurnAction(_userName));
			return actions;
		}

		IGameAction SelectAction(GameState state, List<IGameAction> availableActions) {
			return availableActions.
				Select(action => new TempState(state, action, _userName)).
				OrderByDescending(ts => ts.Measure).
				First().Action;
		}
	}
}
