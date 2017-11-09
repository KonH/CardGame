using UnityEngine;
using UDBase.Utils;
using UDBase.Utils.Json;
using SharedLibrary.Utils;
using SharedLibrary.Common;
using SharedLibrary.Models;
using SharedLibrary.Actions;

public class NetworkGameController : BaseGameController {
	const string _getStateUrl   = "{0}/api/game/state?session={1}";
	const string _getActionUrl  = "{0}/api/game/action?session={1}&version={2}";
	const string _postActionUrl = "{0}/api/game/action?session={1}&version={2}&type={3}";

	GameState _state;
	protected override GameState State {
		get {
			return _state;
		}
	}

	bool IsBusy {
		get {
			return _postClient.InProgress;
		}
	}

	bool IsNeedToUpdate {
		get {
			return !_updateClient.InProgress && !IsBusy && (Time.realtimeSinceStartup > _lastUpdateTime + _updateInterval);
		}
	}

	BearerWebClient _updateClient   = new BearerWebClient();
	BearerWebClient _postClient     = new BearerWebClient();
	float           _updateInterval = 0.0f;

	bool  _ready;
	float _lastUpdateTime;


	public NetworkGameController(float updateInterval) {
		_updateInterval = updateInterval;
	}

	public override void Init() {
		base.Init();
		_updateClient.Init();
		_postClient  .Init();
	}

	public override void Reset() {
		_updateClient.Reset();
		_postClient  .Reset();
	}

	public override void Start() {
		_updateClient.SendGetRequest(CardUrl.Prepare(_getStateUrl, Sessions.CurrentSessionId), onComplete: OnStartComplete);
	}

	public override void Update() {
		if ( _ready && IsNeedToUpdate ) {
			var url = CardUrl.Prepare(_getActionUrl, Sessions.CurrentSessionId, State.Version.ToString());
			_updateClient.SendGetRequest(url, onComplete: OnGetActionComplete);
		}
	}

	void OnStartComplete(NetUtils.Response resp) {
		if ( !resp.HasError ) {
			_state = JsonUtils.Deserialize<GameState>(resp.Text);
			OnGameInit();
			_ready = true;
		}
	}

	string GetTypeHeader(NetUtils.Response resp) {
		return resp.GetHeader(Interaction.ActionTypeHeader);
	}

	void OnGetActionComplete(NetUtils.Response resp) {
		_lastUpdateTime = Time.realtimeSinceStartup;
		if ( !resp.HasError ) {
			ParseNewAction(GetTypeHeader(resp), resp.Text);
		}
	}

	void ParseNewAction(string headerValue, string textResponse) {
		if ( !string.IsNullOrEmpty(textResponse) ) {
			var type = ReflectionUtils.GetActionType(headerValue);
			if ( type != null ) {
				var action = JsonUtils.Deserialize(textResponse, type) as IGameAction;
				ApplyIncomingAction(action);
			}
		}
	}

	protected override void OnGameEnd() {
		_ready = false;
	}

	public override void ApplyAction<T>(T action) {
		if ( IsBusy ) {
			return;
		}
		var version    = State.Version.ToString();
		var actionName = action.GetType().FullName;
		var json       = JsonUtils.Serialize(action);
		_postClient.SendJsonPostRequest(CardUrl.Prepare(_postActionUrl, Sessions.CurrentSessionId, version, actionName), json);
	}
}

