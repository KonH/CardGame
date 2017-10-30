using FullSerializer;
using System;
using UDBase.Controllers;
using UDBase.Controllers.EventSystem;
using UDBase.Controllers.LogSystem;
using UDBase.Controllers.UserSystem;
using UDBase.Utils;
using UDBase.Utils.Json;

public interface IAuthController : IController {
	string Login { get; }
	string Password { get; }
	string AuthHeader { get; }
	void TryLogin(string login, string password, Action<bool, string> onComplete);
}

public class Auth : ControllerHelper<IAuthController> {
	public static string Login {
		get {
			if ( Instance != null ) {
				return Instance.Login;
			}
			return null;
		}
	}

	public static string Password {
		get {
			if ( Instance != null ) {
				return Instance.Password;
			}
			return null;
		}
	}

	public static string AuthHeader {
		get {
			if ( Instance != null ) {
				return Instance.AuthHeader;
			}
			return null;
		}
	}

	public static void TryLogin(string login, string password, Action<bool, string> onComplete) {
		if ( Instance != null ) {
			Instance.TryLogin(login, password, onComplete);
		}
	}
}

class AuthController : IAuthController {
	class Jwtoken {
		[fsProperty("token")]
		public string Token = "";
		[fsProperty("userName")]
		public string UserName = "";
	}

	public string Login {
		get {
			return User.FindExternalId("login");
		}
		private set {
			User.AddExternalId("login", value);

		}
	}

	public string Password {
		get {
			return User.FindExternalId("password");
		}
		private set {
			User.AddExternalId("password", value);
		}
	}

	public string AuthHeader {
		get {
			return User.FindExternalId("token");
		}
		private set {
			User.AddExternalId("token", value);
			OnAuthHeaderUpdate();
		}
	}

	const string _authUrl = "{0}/api/auth/token?login={1}&password={2}";

	WebClient _client = new WebClient();

	public void Init() {}

	public void PostInit() {
		UnityHelper.AddPersistantStartCallback(TryReloadToken);
	}

	public void Reset() {}

	void OnAuthHeaderUpdate() {
		Events.Fire(new Auth_UpdateHeader(AuthHeader));
	}

	void TryReloadToken() {
		if ( !string.IsNullOrEmpty(AuthHeader) ) {
			OnAuthHeaderUpdate();
		}
	}

	public void TryLogin(string login, string password, Action<bool, string> onComplete) {
		Login = login;
		Password = password;
		Log.MessageFormat("TryLogin: login: '{0}', password: '{1}'", LogTags.Auth, login, password);
		if ( !_client.InProgress ) {
			var request = CardUrl.Prepare(_authUrl, login, password);
			_client.SendGetRequest(request, onComplete: resp => OnLoginComplete(resp, onComplete));
		}
	}

	void OnLoginComplete(NetUtils.Response resp, Action<bool, string> callback) {
		var success = !resp.HasError;
		Log.MessageFormat("OnLoginComplete: success: {0}", LogTags.Auth, success);
		if ( success ) {
			var token = ExtractToken(resp.Text);
			if ( token != null ) {
				User.Name = token.UserName;
				AuthHeader = "Bearer " + token.Token;
			} else {
				success = false;
			}
		}
		var errorText = "";
		if ( !success ) {
			errorText = !string.IsNullOrEmpty(resp.Text) ? resp.Text : resp.Code.ToString();
		}
		callback?.Invoke(success, errorText);
	}

	Jwtoken ExtractToken(string json) {
		return JsonUtils.Deserialize<Jwtoken>(json);
	}
}


