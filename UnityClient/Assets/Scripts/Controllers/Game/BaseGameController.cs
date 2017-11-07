using UDBase.Controllers.UserSystem;
using UDBase.Controllers.EventSystem;
using SharedLibrary.Common;
using SharedLibrary.Models;
using SharedLibrary.Actions;
using SharedLibrary.Models.Game;
using UDBase.Controllers.SceneSystem;

public abstract class BaseGameController : IGame {

	protected abstract GameState State { get; }

	public virtual void Init() {}

	public virtual void PostInit() {}

	public virtual void Reset() {}

	public abstract void Start();

	protected void OnGameInit() {
		Events.Fire(new Game_Init(State));
	}

	protected void ApplyIncomingAction(IGameAction action) {
		if ( (action != null) && State.TryApply(action) ) {
			// TODO: Implement concrete action handlers
			OnNewActionApplyed();
		}
	}

	protected void OnNewActionApplyed() {
		Events.Fire(new Game_Reload(State));
		if ( State.IsEnded ) {
			Events.Fire(new Game_End(State.Winner));
			OnGameEnd();
		}
	}

	protected virtual void OnGameEnd() {}

	public virtual void Update() {}

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

