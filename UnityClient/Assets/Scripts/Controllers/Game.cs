using UnityEngine;
using UDBase.Utils;
using UDBase.Utils.Json;
using UDBase.Controllers;
using UDBase.Controllers.EventSystem;
using SharedLibrary.Utils;
using SharedLibrary.Common;
using SharedLibrary.Models;
using SharedLibrary.Actions;

public interface IGame : IController {
	void Start();
	void Update();
	void ApplyAction<T>(T action) where T : IGameAction, new();
	void ApplyTestAttack();
	void NextTurn();
}

public class Game : ControllerHelper<IGame> {
	public static void Start() {
		if ( Instance != null ) {
			Instance.Start();
		}
	}

	public static void Update() {
		if ( Instance != null ) {
			Instance.Update();
		}
	}

	public static void ApplyAction<T>(T action) where T : IGameAction, new() {
		if ( Instance != null ) {
			Instance.ApplyAction(action);
		}
	}

	public static void ApplyTestAttack() {
		if ( Instance != null ) {
			Instance.ApplyTestAttack();
		}
	}

	public static void NextTurn() {
		if ( Instance != null ) {
			Instance.NextTurn();
		}
	}
}

public class GameController : IGame {
	const string _getStateUrl   = "{0}/api/game/state?session={1}";
	const string _getActionUrl  = "{0}/api/game/action?session={1}&version={2}";
	const string _postActionUrl = "{0}/api/game/action?session={1}&version={2}&type={3}";

	BearerWebClient _updateClient = new BearerWebClient();
	BearerWebClient _postClient = new BearerWebClient();
	GameState       _state;
	float           _updateInterval;

	bool  _ready;
	float _lastUpdateTime;

	bool IsBusy {
		get {
			return _postClient.InProgress;
		}
	}

	public GameController(float updateInterval) {
		_updateInterval = updateInterval;
	}

	public void Init() {
		_updateClient.Init();
		_postClient  .Init();
	}

	public void PostInit() {}

	public void Reset() {
		_updateClient.Reset();
		_postClient  .Reset();
	}

	public void Start() {
		_updateClient.SendGetRequest(CardUrl.Prepare(_getStateUrl, Sessions.CurrentSessionId), onComplete: OnStartComplete);
	}

	bool IsNeedToUpdate() {
		return !_updateClient.InProgress && !IsBusy && (Time.realtimeSinceStartup > _lastUpdateTime + _updateInterval);
	}

	public void Update() {
		if ( _ready && IsNeedToUpdate() ) {
			var url = CardUrl.Prepare(_getActionUrl, Sessions.CurrentSessionId, _state.Version.ToString());
			_updateClient.SendGetRequest(url, onComplete: OnGetActionComplete);
		}
	}

	void OnStartComplete(NetUtils.Response resp) {
		if ( !resp.HasError ) {
			_state = JsonUtils.Deserialize<GameState>(resp.Text);
			Events.Fire(new Game_Init(_state));
			_ready = true;
		}
	}

	string GetTypeHeader(NetUtils.Response resp) {
		return resp.GetHeader(Interaction.ActionTypeHeader);
	}

	void OnGetActionComplete(NetUtils.Response resp) {
		_lastUpdateTime = Time.realtimeSinceStartup;
		if ( !resp.HasError ) {
			ProcessNewAction(GetTypeHeader(resp), resp.Text);
		}
	}

	void ProcessNewAction(string headerValue, string textResponse) {
		if ( !string.IsNullOrEmpty(textResponse) ) {
			var type = ReflectionUtils.GetActionType(headerValue);
			if ( type != null ) {
				var action = JsonUtils.Deserialize(textResponse, type) as IGameAction;
				if ( (action != null) && _state.TryApply(action) ) {
					// TODO: Implement concrete action handlers
					Events.Fire(new Game_Reload(_state));
				}
			}
		}
	}

	public void ApplyTestAttack() {
		var action = new DamageAction(_state.Users.Find(u => u.Name != UDBase.Controllers.UserSystem.User.Name).Name, 1);
		ApplyAction(action);
	}

	public void NextTurn() {
		ApplyAction(new TurnAction());
	}

	public void ApplyAction<T>(T action) where T:IGameAction, new() {
		if ( IsBusy ) {
			return;
		}
		var version    = _state.Version.ToString();
		var actionName = action.GetType().FullName;
		var json       = JsonUtils.Serialize(action);
		_postClient.SendJsonPostRequest(CardUrl.Prepare(_postActionUrl, Sessions.CurrentSessionId, version, actionName), json);
	}
}

