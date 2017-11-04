using SharedLibrary.Models;
using SharedLibrary.Models.Game;
using System.Collections.Generic;
using UDBase.Controllers.EventSystem;
using UDBase.Controllers.UserSystem;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
	public UserView Player;
	public UserView Enemy;
	public Button TurnButton;
	public Button AttackButton;
	public CardView CardPrefab;

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
			var active = user.Name == state.TurnOwner;
			DrawUserState(user, active);
			if ( user.Name == User.Name ) {
				UpdateControlState(active);
			}
		}
	}

	UserView SelectView(UserState userState) {
		if ( userState.Name == User.Name ) {
			return Player;
		}
		return Enemy;
	}

	void UpdateControlState(bool active) {
		AttackButton.interactable = active;
		TurnButton.interactable = active;
	}

	void DrawUserState(UserState userState, bool activeUser) {
		var view = SelectView(userState);
		var nameText = userState.Name;
		if ( activeUser ) {
			nameText = string.Format("<b>{0}</b>", nameText);
		}
		view.NameText.text = nameText;
		view.HPText.text = string.Format("{0}/{1}", userState.Health, userState.MaxHealth);

		UpdateGlobalCount(userState.GlobalSet.Count, view.Global);
		UpdateCards(userState.HandSet, view.Hand);
		UpdateCards(userState.TableSet, view.Table);
	}

	void UpdateGlobalCount(int count, CardSet global) {
		CardView globalCardView;
		if ( global.Cards.Count == 0 ) {
			globalCardView = ObjectPool.Spawn(CardPrefab);
			global.Add(globalCardView);
		} else {
			globalCardView = global.Cards[0];
		}
		globalCardView.Init(count.ToString(), 0, 0, 0, 0);
	}

	void UpdateCards(List<CardState> cards, CardSet set) {
		set.Clear();
		foreach ( var card in cards ) {
			var cardView = ObjectPool.Spawn(CardPrefab);
			cardView.Init(card.Type.ToString(), 0, 0, 0, 0);
			set.Add(cardView);
		}
	}

	void OnTurn() {
		Game.NextTurn();
	}

	void OnAttack() {
		Game.ApplyTestAttack();
	}
}
