using System.Linq;
using System.Collections.Generic;
using UDBase.Utils;
using UDBase.Utils.Json;
using UDBase.Controllers;
using UDBase.Controllers.LogSystem;
using UDBase.Controllers.UserSystem;
using UDBase.Controllers.EventSystem;
using SharedLibrary.Models;

public interface ISessionController : IController {
	string CurrentSessionId { get; }
	void TryConnect(string sessionId);
	void TryRefresh();
	void TryCreate();
	void TryClose();
}

public class Sessions : ControllerHelper<ISessionController> {
	public static string CurrentSessionId {
		get {
			if ( Instance != null ) {
				return Instance.CurrentSessionId;
			}
			return null;
		}
	}
	public static void TryConnect(string sessionId) {
		if ( Instance != null ) {
			Instance.TryConnect(sessionId);
		}
	}

	public static void TryRefresh() {
		if ( Instance != null ) {
			Instance.TryRefresh();
		}
	}

	public static void TryCreate() {
		if ( Instance != null ) {
			Instance.TryCreate();
		}
	}

	public static void TryClose() {
		if ( Instance != null ) {
			Instance.TryClose();
		}
	}
}

public class SessionController : ISessionController {
	const string _refreshUrl = "{0}/api/session";
	const string _createUrl  = "{0}/api/session";
	const string _closeUrl   = "{0}/api/session/{1}";
	const string _connectUrl = "{0}/api/session/connect/{1}";

	public string CurrentSessionId { get; private set; }

	int _retryCount = 0;

	BearerWebClient _client        = new BearerWebClient();
	List<Session>   _sessions      = new List<Session>();
	int             _curRetryCount = 0;
	bool            _ready         = true;

	public SessionController(int retryCount) {
		_retryCount = retryCount;
	}

	public void Init() {
		_client.Init();
	}

	public void PostInit() {}

	public void Reset() {
		_client.Reset();
	}

	public void TryConnect(string sessionId) {
		if ( _ready && !_client.InProgress ) {
			_client.SendPostRequest(CardUrl.Prepare(_connectUrl, sessionId), "", onComplete: (resp) => OnConnectComplete(sessionId, resp));
		}
	}

	void OnConnectComplete(string id, NetUtils.Response resp) {
		Log.MessageFormat("OnConnectComplete: {0}, '{1}'", LogTags.Session, resp.Code, resp.Text);
		if ( !resp.HasError ) {
			CurrentSessionId = id;
			Events.Fire(new Session_ConnectComplete(id));
		} else {
			Events.Fire(new Common_Error(resp.GetNonEmptyText()));
		}
	}

	public void TryRefresh() {
		if ( _ready && !_client.InProgress ) {
			Log.Message("TryRefresh.", LogTags.Session);
			_client.SendGetRequest(CardUrl.Prepare(_refreshUrl), onComplete: OnRefreshSessionsComplete);
		}
	}

	void OnRefreshSessionsComplete(NetUtils.Response resp) {
		if ( !resp.HasError ) {
			SetupSessions(resp.Text);
			UpdateUserSession(_sessions);
			_curRetryCount = 0;
		} else {
			_curRetryCount++;
			if ( _curRetryCount >= _retryCount ) {
				_ready = false;
				Events.Fire(new Common_Error(resp.GetNonEmptyText()));
			}
		}
		Events.Fire(new Session_Update(_sessions));
	}

	void UpdateUserSession(List<Session> sessions) {
		var userSession = sessions.Where(s => s.Owner == User.Name).FirstOrDefault();
		if ( userSession != null ) {
			CurrentSessionId = userSession.Id;
			if ( !userSession.Awaiting ) {
				Events.Fire(new Session_ConnectComplete(userSession.Id));
			}
		} else {
			CurrentSessionId = null;
		}
	}

	void SetupSessions(string json) {
		_sessions.Clear();
		_sessions = JsonUtils.Deserialize<List<Session>>(json);
	}

	public void TryCreate() {
		if ( _ready && !_client.InProgress ) {
			Log.Message("TryCreate.", LogTags.Session);
			_client.SendPostRequest(CardUrl.Prepare(_createUrl), "");
		}
	}

	public void TryClose() {
		if ( _ready && !_client.InProgress && !string.IsNullOrEmpty(CurrentSessionId) ) {
			Log.Message("TryClose", LogTags.Session);
			_client.SendDeleteRequest(CardUrl.Prepare(_closeUrl, CurrentSessionId));
		}
	}
}
