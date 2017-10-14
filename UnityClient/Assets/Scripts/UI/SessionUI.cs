using System.Collections.Generic;
using SharedLibrary.Common;
using SharedLibrary.Models;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UDBase.Controllers.SceneSystem;
using UDBase.Controllers.EventSystem;

public class SessionUI : MonoBehaviour {
	public SessionView View;
	public Button CreateButton;

	float _lastTime;
	float _updateInterval = 3;

	void Awake() {
		_lastTime = -_updateInterval;
		CreateButton.onClick.AddListener(TryCreate);
	}

	void Start() {
		UpdateSessions(new List<Session>());
		Events.Subscribe<Session_Update>(this, OnSessionUpdate);
		Events.Subscribe<Session_ConnectComplete>(this, OnConnectComplete);
	}

	void OnDestroy() {
		Events.Unsubscribe<Session_Update>(OnSessionUpdate);
		Events.Unsubscribe<Session_ConnectComplete>(OnConnectComplete);
	}

	void UpdateSessions(List<Session> sessions) {
		if ( !View ) {
			return;
		}
		if ( sessions.Count > 0 ) {
			View.gameObject.SetActive(true);
			var firstSession = sessions.Where(s => s.Awaiting).FirstOrDefault();
			if ( firstSession != null ) {
				View.Init(firstSession.Owner, firstSession.Users.Count, SessionRules.MaxUsersInSession, firstSession.Id);
			}
		} else {
			View.gameObject.SetActive(false);
		}
	}

	void OnSessionUpdate(Session_Update e) {
		_lastTime = Time.realtimeSinceStartup;
		UpdateSessions(e.Sessions);
	}

	void OnConnectComplete(Session_ConnectComplete e) {
		Scene.LoadSceneByName("Game");
	}

	public void TryCreate() {
		Sessions.TryCreate();
	}

	void Update() {
		if ( Time.realtimeSinceStartup > _lastTime + _updateInterval ) {
			Sessions.TryRefresh();
		}
	}
}
