using System.Collections.Generic;
using SharedLibrary.Common;
using SharedLibrary.Models;
using UDBase.Utils;
using UnityEngine;
using UnityEngine.UI;
using FullSerializer;
using System;
using System.Linq;
using UDBase.Controllers.SceneSystem;

public class SessionSample : MonoBehaviour {
	public SessionView View;
	public Text UserText;
	public Button CreateButton;

	WebClient _client;
	string _url = "http://localhost:8080/api/session";
	string _connectUrl = "http://localhost:8080/api/session/connect/{0}";
	List<Session> _sessions = new List<Session>();
	float _lastTime;
	float _updateInterval = 3;

	void Awake() {
		CreateButton.onClick.AddListener(TryCreate);
	}

	void Start() {
		InitClient();
		UpdateSessions();
		UserText.text = TempStorage.UserName;
	}

	void InitClient() {
		_client = new WebClient();
		_client.ApplyAuthHeader(TempStorage.AuthHeader);
	}

	void UpdateSessions() {
		if ( !View ) {
			return;
		}
		if ( _sessions.Count > 0 ) {
			View.gameObject.SetActive(true);
			var firstSession = _sessions.Where(s => s.Awaiting).FirstOrDefault();
			if ( firstSession != null ) {
				View.Init(firstSession.Owner, firstSession.Users.Count, SessionRules.MaxUsersInSession, this, firstSession.Id);
			}
		} else {
			View.gameObject.SetActive(false);
		}
		_client.SendGetRequest(_url, onComplete: OnUpdateSessionsComplete);
		UpdateUserSession();
	}

	void UpdateUserSession() {
		var userSession = _sessions.Where(s => s.Owner == TempStorage.UserName).FirstOrDefault();
		if ( userSession != null ) {
			if ( !userSession.Awaiting ) {
				GoToGame();
			}
		}
	}

	void OnUpdateSessionsComplete(NetUtils.Response resp) {
		_lastTime = Time.realtimeSinceStartup;
		if ( !resp.HasError ) {
			SetupSessions(resp.Text);
		}
		UpdateSessions();
	}

	void SetupSessions(string json) {
		var serializer = new fsSerializer();
		try {
			var data = fsJsonParser.Parse(json);
			serializer.TryDeserialize(data, ref _sessions);
		} catch ( Exception e ) {
			Debug.LogError(e.Message);
		}
	}

	public void TryCreate() {
		_client.SendPostRequest(_url, "");
	}

	public void TryConnect(string sessionId) {
		_client.SendPostRequest(string.Format(_connectUrl, sessionId), "", onComplete: OnConnectComplete);
	}

	void OnConnectComplete(NetUtils.Response resp) {
		Debug.Log(resp.Code);
		Debug.Log(resp.Text);
		if ( !resp.HasError ) {
			GoToGame();
		}
	}

	void GoToGame() {
		Scene.LoadSceneByName("Game");
	}

	void Update() {
		if ( !_client.InProgress ) {
			if ( _lastTime + _updateInterval > Time.realtimeSinceStartup ) {
				UpdateSessions();
			}
		}
	}
}
