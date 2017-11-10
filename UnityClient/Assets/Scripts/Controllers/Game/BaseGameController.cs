using System.Collections.Generic;
using UDBase.Controllers.LogSystem;
using UDBase.Controllers.UserSystem;
using UDBase.Controllers.EventSystem;
using UDBase.Controllers.SceneSystem;
using SharedLibrary.Common;
using SharedLibrary.Models;
using SharedLibrary.Actions;
using SharedLibrary.Models.Game;

public abstract class BaseGameController : IGame {
	public abstract GameState State { get; }

	public IGameAction CurrentAction { get; private set; }

	protected Queue<IGameAction> Actions { get; private set; }


	public virtual void Init() {
		Actions = new Queue<IGameAction>();
	}

	public virtual void PostInit() {}

	public virtual void Reset() {}

	public abstract void Start();

	protected void OnGameInit() {
		Events.Fire(new Game_Init(State));
	}

	protected void ApplyIncomingAction(IGameAction action) {
		if ( (action != null) && State.TryApply(action) ) {
			OnNewActionApplied(action);
		}
	}

	protected void OnNewActionApplied(IGameAction action) {
		Log.MessageFormat("OnNewActionApplied: '{0}'", LogTags.Game, action);
		Actions.Enqueue(action);
	}

	void FireNewAction(IGameAction action) {
		Log.MessageFormat("FireNewAction: '{0}'", LogTags.Game, action);
		CurrentAction = action;
		Events.Fire(new Game_NewAction(action));
		if ( State.IsEnded ) {
			Events.Fire(new Game_End(State.Winner));
			OnGameEnd();
		}
	}

	public void EndCurrentAction() {
		Log.MessageFormat("EndCurrentAction: '{0}'", LogTags.Game, CurrentAction);
		CurrentAction = null;
		CheckForNewAction();
	}

	void CheckForNewAction() {
		Log.Message("CheckForNewAction", LogTags.Game);
		if ( Actions.Count > 0 ) {
			var action = Actions.Dequeue();
			FireNewAction(action);
		}
	}

	protected virtual void OnGameEnd() {}

	public virtual void Update() {
		if ( (CurrentAction == null) && (Actions.Count > 0) ) {
			CheckForNewAction();
		}
	}

	public void NextTurn() {
		ApplyAction(new TurnAction());
	}

	public abstract void ApplyAction<T>(T action) where T : IGameAction, new();

	public bool CanBought(CardState card) {
		if ( card.Type == CardType.Hidden ) {
			return false;
		}
		var userState = GetUserState();
		if ( (State.TurnOwner == userState.Name) && (card.Price <= userState.Power) ) {
			if ( card.Type == CardType.Creature ) {
				foreach ( var tableCard in userState.TableSet ) {
					if ( tableCard == null ) {
						return true;
					}
				}
			}
		}
		return false;
	}

	public UserState GetUserState() {
		return State.Users.Find(u => u.Name == User.Name);
	}

	public UserState GetEnemyState() {
		return State.Users.Find(u => u.Name != User.Name);
	}

	public void ApplyEnd() {
		Scene.LoadSceneByName("Main");
	}
}

