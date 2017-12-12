using UnityEngine;
using UnityEngine.UI;
using UDBase.Utils;
using UDBase.Controllers.SceneSystem;

public class LoginUI : MonoBehaviour {
	public InputField LoginField;
	public InputField PasswordField;
	public Button     LoginButton;

	void Start () {
		LoginButton.onClick.AddListener(TryLogin);
		SetupCredentials();
	}

	void SetupCredentials() {
		var login          = TextUtils.EnsureString(Auth.Login);
		LoginField.text    = login;
		var password       = TextUtils.EnsureString(Auth.Password);
		PasswordField.text = password;
	}

	void TryLogin() {
		var login    = LoginField.text;
		var password = PasswordField.text;
		Auth.TryLogin(login, password, OnAuthComplete);
	}

	void OnAuthComplete(bool success, string errorText) {
		if ( success ) {
			Scene.LoadSceneByName("Main");
		} else {
			NoticeWindow.ShowWithOkButton("Server Error", errorText);
		}
	}
}
