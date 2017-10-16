using System.Collections.Generic;
using SharedLibrary.Models;
using UDBase.Controllers;
using UDBase.Utils;
using UDBase.Controllers.LogSystem;
using UDBase.Controllers.EventSystem;
using FullSerializer;
using System;
using System.Linq;
using UDBase.Controllers.UserSystem;

public interface ISessionController : IController {
	void TryConnect(string sessionId);
	void TryRefresh();
	void TryCreate();
}

public class Sessions : ControllerHelper<ISessionController> {
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
}

public class SessionController : ISessionController {
	const string _refreshUrl = "{0}/api/session";
	const string _createUrl = "{0}/api/session";
	const string _connectUrl = "{0}/api/session/connect/{1}";

	BearerWebClient _client = new BearerWebClient();
	List<Session> _sessions = new List<Session>();

	public void Init() {
		_client.Init();
		
	}

	public void PostInit() {}

	public void Reset() {
		_client.Reset();
	}

	public void TryConnect(string sessionId) {
		if ( !_client.InProgress ) {
			_client.SendPostRequest(CardUrl.Prepare(_connectUrl, sessionId), "", onComplete: (resp) => OnConnectComplete(sessionId, resp));
		}
	}

	void OnConnectComplete(string id, NetUtils.Response resp) {
		Log.MessageFormat("OnConnectComplete: {0}, '{1}'", LogTags.Session, resp.Code, resp.Text);
		if ( !resp.HasError ) {
			Events.Fire(new Session_ConnectComplete(id));
		}
	}

	public void TryRefresh() {
		if ( !_client.InProgress ) {
			Log.Message("TryRefresh.", LogTags.Session);
			_client.SendGetRequest(CardUrl.Prepare(_refreshUrl), onComplete: OnRefreshSessionsComplete);
		}
	}

	void OnRefreshSessionsComplete(NetUtils.Response resp) {
		if ( !resp.HasError ) {
			SetupSessions(resp.Text);
			UpdateUserSession(_sessions);
		}
		Events.Fire(new Session_Update(_sessions));
	}

	void UpdateUserSession(List<Session> sessions) {
		var userSession = sessions.Where(s => s.Owner == User.Name).FirstOrDefault();
		if ( userSession != null ) {
			if ( !userSession.Awaiting ) {
				Events.Fire(new Session_ConnectComplete(userSession.Id));
			}
		}
	}

	void SetupSessions(string json) {
		var serializer = new fsSerializer();
		try {
			var data = fsJsonParser.Parse(json);
			serializer.TryDeserialize(data, ref _sessions);
		} catch ( Exception e ) {
			Log.ErrorFormat("SetupSessions: exception: {0}", LogTags.Session, e);
		}
	}

	public void TryCreate() {
		if ( !_client.InProgress ) {
			Log.Message("TryCreate.", LogTags.Session);
			_client.SendPostRequest(CardUrl.Prepare(_createUrl), "");
		}
	}
}
