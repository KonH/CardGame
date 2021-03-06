﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UDBase.Controllers.SceneSystem;
using UDBase.Controllers.EventSystem;
using SharedLibrary.Common;
using SharedLibrary.Models;

public class SessionUI : MonoBehaviour {
	public SessionView View;
	public Button      CreateButton;
	public Button      CloseButton;

	float _lastTime;
	float _updateInterval = 3;

	void Awake() {
		_lastTime = -_updateInterval;
		CreateButton.onClick.AddListener(TryCreate);
		CloseButton.onClick.AddListener(TryClose);
	}

	void Start() {
		UpdateSessions(new List<Session>());
		Events.Subscribe<Common_Error>(this, OnError);
		Events.Subscribe<Session_Update>(this, OnSessionUpdate);
		Events.Subscribe<Session_ConnectComplete>(this, OnConnectComplete);
	}

	void OnDestroy() {
		Events.Unsubscribe<Common_Error>(OnError);
		Events.Unsubscribe<Session_Update>(OnSessionUpdate);
		Events.Unsubscribe<Session_ConnectComplete>(OnConnectComplete);
	}

	void OnError(Common_Error e) {
		NoticeWindow.ShowWithOkButton("Server Error", e.Text, () => Scene.LoadSceneByName("Login"));
	}

	void UpdateSessions(List<Session> sessions) {
		if ( !View ) {
			return;
		}
		if ( sessions.Count > 0 ) {
			var firstSession = sessions.Where(s => s.Awaiting).FirstOrDefault();
			if ( firstSession != null ) {
				View.gameObject.SetActive(true);
				View.Init(firstSession.Owner, firstSession.Users.Count, SessionRules.MaxUsersInSession, firstSession.Id);
			} else {
				View.gameObject.SetActive(false);
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

	public void TryClose() {
		Sessions.TryClose();
	}

	void Update() {
		if ( Time.realtimeSinceStartup > _lastTime + _updateInterval ) {
			Sessions.TryRefresh();
		}
	}
}
