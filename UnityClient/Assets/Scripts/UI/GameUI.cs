using SharedLibrary.Models;
using SharedLibrary.Models.Game;
using UDBase.Controllers.EventSystem;
using UDBase.Controllers.UserSystem;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
	public UserView Player;
	public UserView Enemy;
	public Button TurnButton;
	public Button AttackButton;

	void Awake() {
		TurnButton.onClick.AddListener(OnTurn);
		AttackButton.onClick.AddListener(OnAttack);
	}

	void Start() {
		Events.Subscribe<Game_Init>(this, OnGameInit);
		Events.Subscribe<Game_Reload>(this, OnGameReload);
		Game.Start();
	}

	void OnDestroy() {
		Events.Unsubscribe<Game_Init>(OnGameInit);
		Events.Unsubscribe<Game_Reload>(OnGameReload);
	}

	void Update() {
		Game.Update();
	}

	void OnGameInit(Game_Init e) {
		FullStateUpdate(e.State);
	}

	void OnGameReload(Game_Reload e) {
		FullStateUpdate(e.State);
	}

	void FullStateUpdate(GameState state) {
		var users = state.Users;
		foreach ( var user in users ) {
			DrawUserState(user);
		}
	}

	UserView SelectView(UserState userState) {
		if ( userState.Name == User.Name ) {
			return Player;
		}
		return Enemy;
	}

	void DrawUserState(UserState userState) {
		var view = SelectView(userState);
		view.NameText.text = userState.Name;
		view.HPText.text = string.Format("{0}/{1}", userState.Health, userState.MaxHealth);
	}

	void OnTurn() {

	}

	void OnAttack() {
		Game.ApplyTestAttack();
	}
}
