using UnityEngine;
using UnityEngine.UI;
using UDBase.Utils;
using FullSerializer;
using System;
using UDBase.Controllers.SceneSystem;

public class LoginSample : MonoBehaviour {

	class Jwtoken {
		[fsProperty("token")]
		public string Token = "";
		[fsProperty("userName")]
		public string UserName = "";
	}

	public InputField LoginField;
	public InputField PasswordField;
	public Button LoginButton;
	public Button GetButton;
	public Text ResultText;

	string _authUrl = "http://localhost:8080/api/auth/token?login={0}&password={1}";
	string _dataUrl = "http://localhost:8080/api/values";
	WebClient _client = new WebClient();

	void Start () {
		LoginButton.onClick.AddListener(TryLogin);
		GetButton.onClick.AddListener(TryGetData);
	}

	void TryLogin() {
		var login = LoginField.text;
		var password = PasswordField.text;
		if ( !_client.InProgress ) {
			var request = string.Format(_authUrl, login, password);
			_client.SendGetRequest(request, onComplete: OnAuthComplete);
		}
	}

	void OnAuthComplete(NetUtils.Response resp) {
		ResultText.text = resp.Text;
		Debug.Log(resp.Text);
		if ( !resp.HasError ) {
			var token = ExtractToken(resp.Text);
			if ( token != null ) {
				ResultText.text = token.Token + ";" + token.UserName;
				var header = "Bearer " + token.Token;
				Debug.Log(header);
				TempStorage.AuthHeader = header;
				TempStorage.UserName = token.UserName;
				_client.ApplyAuthHeader(header);
				Debug.Log("Before load");
				Scene.LoadSceneByName("Main");
			}
		}
	}

	Jwtoken ExtractToken(string json) {
		var serializer = new fsSerializer();
		Jwtoken token = null;
		try {
			var data = fsJsonParser.Parse(json);
			serializer.TryDeserialize(data, ref token);
		} catch ( Exception e ) {
			ResultText.text = e.Message;
		}
		return token;
	}

	void TryGetData() {
		if ( !_client.InProgress && _client.HasAuthorization ) {
			_client.SendGetRequest(_dataUrl, onComplete: OnGetData);
		}
	}

	void OnGetData(NetUtils.Response resp) {
		ResultText.text = resp.Text;
	}
}
