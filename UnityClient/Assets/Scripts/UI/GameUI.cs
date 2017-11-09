using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UDBase.Controllers.UserSystem;
using UDBase.Controllers.EventSystem;
using SharedLibrary.Models;
using SharedLibrary.Actions;
using SharedLibrary.Models.Game;
using UDBase.Controllers.LogSystem;

public class GameUI : MonoBehaviour {
	public UserView Player;
	public UserView Enemy;
	public Button   TurnButton;
	public CardView CardPrefab;

	// TODO: States
	bool _inBuySelection;
	bool _inAttackSelection;
	int  _selectedHandIndex;
	int  _selectedTableIndex;

	void Awake() {
		TurnButton.onClick.AddListener(OnTurn);
	}

	void Start() {
		Player.Hand.Init (OnPlayerHandCardClick);
		Player.Table.Init(OnPlayerTableCardClick);
		Enemy.Table.Init (OnEnemyTableCardClick);
		Enemy.User.Init  (OnEnemyUserCardClick);

		Events.Subscribe<Game_Init>  (this, OnGameInit);
		Events.Subscribe<Game_Reload>(this, OnGameReload);
		Events.Subscribe<Game_End>   (this, OnGameEnd);
		Game.Start();
	}

	void OnDestroy() {
		Events.Unsubscribe<Game_Init>  (OnGameInit);
		Events.Unsubscribe<Game_Reload>(OnGameReload);
		Events.Unsubscribe<Game_End>   (OnGameEnd);
	}

	void Update() {
		// Temp
		if ( Game.CurrentAction != null ) {
			Game.EndCurrentAction();
		}
		Game.Update();
	}

	void OnGameInit(Game_Init e) {
		FullStateUpdate(e.State);
	}

	void OnGameEnd(Game_End e) {
		Log.MessageFormat("GameEnded: Winner: '{0}'", LogTags.UI, e.Winner);
		Game.ApplyEnd();
	}

	void OnGameReload(Game_Reload e) {
		FullStateUpdate(e.State);
	}

	void FullStateUpdate(GameState state) {
		_inBuySelection = false;
		_inAttackSelection = false;
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

		UpdateUserHolder(false, view.User);
		UpdateGlobalCount(userState.GlobalSet.Count, view.Global);
		UpdateCards(userState.HandSet, view.Hand, (card) => activeUser && Game.CanBought(card));
		UpdateTableSet(userState, view.Table, activeUser);
	}

	void UpdateTableSet(UserState userState, CardSet set, bool activeUser) {
		UpdateCards(userState.TableSet, set, (card) => (card != null) && activeUser && card.Actions > 0);
	}

	CardView GetFirstOrCreateView(CardSet set) {
		CardView view;
		if ( set.Cards.Count == 0 ) {
			view = ObjectPool.Spawn(CardPrefab);
			set.Add(view);
		} else {
			view = set.Cards[0];
		}
		return view;
	}

	void UpdateUserHolder(bool interactable, CardSet userSet) {
		var userCardView = GetFirstOrCreateView(userSet);
		userCardView.InitPlaceholder(interactable);
	}

	void UpdateGlobalCount(int count, CardSet global) {
		var globalCardView = GetFirstOrCreateView(global);
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

	void ResetBuySelection(UserState userState) {
		UpdateTableSet(userState, Player.Table, true);
	}

	void OnPlayerHandCardClick(int index) {
		var userState = Game.GetUserState();
		if ( _inBuySelection ) {
			_inBuySelection = false;
			ResetBuySelection(userState);
		} else {
			_inBuySelection = true;
			_selectedHandIndex = index;
			UpdateCards(userState.TableSet, Player.Table, (card) => card == null);
		}
	}

	void OnPlayerTableCardClick(int index) {
		var userState = Game.GetUserState();
		if ( _inBuySelection && (userState.TableSet[index] == null) ) {
			TryMakeBuyAction(userState, _selectedHandIndex, index);
		} else {
			TryStartAttackAction(userState, index);
		}
	}

	void TryStartAttackAction(UserState userState, int tableIndex) {
		if ( _inAttackSelection ) {
			_inAttackSelection = false;
			ResetAttackSelection();
		} else {
			_inAttackSelection = true;
			_selectedTableIndex = tableIndex;
			var enemyTable = Game.GetEnemyState().TableSet;
			UpdateCards(enemyTable, Enemy.Table, c => (c != null));
			UpdateUserHolder(true, Enemy.User);
		}
	}

	void TryMakeBuyAction(UserState userState, int handIndex, int tableIndex) {
		_inBuySelection = false;
		ResetBuySelection(userState);
		var action = new BuyCreatureAction("", handIndex, tableIndex);
		Game.ApplyAction(action);
	}

	void ResetAttackSelection() {
		var enemyTable = Game.GetEnemyState().TableSet;
		UpdateCards(enemyTable, Enemy.Table, _ => false);
		UpdateUserHolder(false, Enemy.User);
	}

	void OnEnemyTableCardClick(int index) {
		_inAttackSelection = false;
		ResetAttackSelection();
		var action = new AttackCreatureAction("", _selectedTableIndex, index);
		Game.ApplyAction(action);
	}

	void OnEnemyUserCardClick(int index) {
		_inAttackSelection = false;
		ResetAttackSelection();
		var action = new AttackPlayerAction("", _selectedTableIndex);
		Game.ApplyAction(action);
	}
}
