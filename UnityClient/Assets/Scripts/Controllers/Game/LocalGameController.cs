using UDBase.Controllers.UserSystem;
using SharedLibrary.AI;
using SharedLibrary.Common;
using SharedLibrary.Models;
using SharedLibrary.Models.Game;

public class LocalGameController : BaseGameController {
	const string _botName = "Enemy";

	GameState _fullState;
	GameAI    _bot;

	public override GameState State {
		get {
			return _fullState?.Filter(User.Name);
		}
	}

	public override void Init() {
		base.Init();
	} 

	public override void Start() {
		var users = new UserState[] {
			new UserState(User.Name, 10, 1),
			new UserState(_botName, 10, 1)
		};
		_fullState = new GameState(users, User.Name);
		_bot = new GameAI(_botName);
		OnGameInit();
	}

	bool CanApplyBotAction() {
		return 
			(Actions.Count == 0) && (CurrentAction == null) &&
			!State.IsEnded &&
			(State.TurnOwner == _botName);
	}

	public override void Update() {
		base.Update();
		if ( CanApplyBotAction() ) {
			var botAction = _bot.GetAction(_fullState.Filter(_botName));
			if ( (botAction != null) && _fullState.TryApply(botAction) ) {
				OnNewActionApplied(botAction);
			}
		}
	}

	public override void ApplyAction<T>(T action) {
		if ( action != null ) {
			action.User = User.Name;
			if ( _fullState.TryApply(action) ) {
				OnNewActionApplied(action);
			}
		}
	}
}

