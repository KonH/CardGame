using SharedLibrary.Models;
using UDBase.Controllers;
using UDBase.Controllers.EventSystem;
using UDBase.Utils;
using UDBase.Utils.Json;

public interface IGame : IController {
	void Start();
}

public class Game : ControllerHelper<IGame> {
	public static void Start() {
		if ( Instance != null ) {
			Instance.Start();
		}
	}
}

public class GameController : IGame {
	const string _getStateUrl = "{0}/api/game";

	BearerWebClient _client = new BearerWebClient();
	GameState _state;

	public void Init() {
		_client.Init();
	}

	public void PostInit() {}

	public void Reset() {
		_client.Reset();
	}

	public void Start() {
		_client.SendGetRequest(CardUrl.Prepare(_getStateUrl), onComplete: OnStartComplete);
	}

	void OnStartComplete(NetUtils.Response resp) {
		if ( !resp.HasError ) {
			_state = JsonUtils.Serialize<GameState>(resp.Text);
			Events.Fire(new Game_Init(_state));
		}
	}
}

