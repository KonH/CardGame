using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UDBase.Controllers.UserSystem;
using UDBase.Controllers.EventSystem;
using SharedLibrary.Models;
using SharedLibrary.Actions;
using SharedLibrary.Models.Game;

public class GameUI : MonoBehaviour {
	public UserView Player;
	public UserView Enemy;
	public Button   TurnButton;
	public CardView CardPrefab;

	int _selectedHandIndex;

	void Awake() {
		TurnButton.onClick.AddListener(OnTurn);
	}

	void Start() {
		Player.Hand.Init(OnPlayerHandCardClick);
		Player.Table.Init(OnPlayerTableCardClick);

		Events.Subscribe<Game_Init>  (this, OnGameInit);
		Events.Subscribe<Game_Reload>(this, OnGameReload);
		Game.Start();
	}

	void OnDestroy() {
		Events.Unsubscribe<Game_Init>  (OnGameInit);
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
		TurnButton.interactable = active;
	}

	void DrawUserState(UserState userState, bool activeUser) {
		var view     = SelectView(userState);
		var nameText = userState.Name;
		if ( activeUser ) {
			nameText = string.Format("<b>{0}</b>", nameText);
		}
		view.NameText.text  = nameText;
		view.HPText.text    = string.Format("HP: {0}/{1}", userState.Health, userState.MaxHealth);
		view.PowerText.text = string.Format("PW: {0}/{1}", userState.Power, userState.MaxPower);

		UpdateGlobalCount(userState.GlobalSet.Count, view.Global);
		UpdateCards(userState.HandSet, view.Hand, (card) => activeUser && Game.CanBought(card));
		UpdateCards(userState.TableSet, view.Table, (card) => false);
	}

	void UpdateGlobalCount(int count, CardSet global) {
		CardView globalCardView;
		if ( global.Cards.Count == 0 ) {
			globalCardView = ObjectPool.Spawn(CardPrefab);
			global.Add(globalCardView);
		} else {
			globalCardView = global.Cards[0];
		}
		globalCardView.Init(true, false, count.ToString(), 0, 0, 0, 0);
	}

	void UpdateCards(List<CardState> cards, CardSet set, Func<CardState, bool> interactable) {
		set.Clear();
		for ( var i = 0; i < cards.Count; i++ ) {
			var card = cards[i];
			var cardView = ObjectPool.Spawn(CardPrefab);
			var interactableState = (interactable != null) && interactable(card);
			if ( card == null ) {
				cardView.InitPlaceholder(interactableState);
			} else {
				cardView.Init(true, interactableState, card.Type.ToString(), card.Price, card.Damage, card.Health, card.MaxHealth);
			}
			set.Add(cardView);
		}
	}

	void OnTurn() {
		Game.NextTurn();
	}

	void OnPlayerHandCardClick(int index) {
		_selectedHandIndex = index;
		var userState = Game.GetCurrentUserState();
		UpdateCards(userState.TableSet, Player.Table, (card) => card == null);
	}

	void OnPlayerTableCardClick(int index) {
		var userState = Game.GetCurrentUserState();
		UpdateCards(userState.TableSet, Player.Table, (card) => false);
		var action = new BuyCreatureAction("", _selectedHandIndex, index);
		Game.ApplyAction(action);
	}
}
