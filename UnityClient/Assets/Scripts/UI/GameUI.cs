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
		Game.Start();
	}

	void OnDestroy() {
		Events.Unsubscribe<Game_Init>(OnGameInit);
	}

	void OnGameInit(Game_Init e) {
		var users = e.State.Users;
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

	}
}
