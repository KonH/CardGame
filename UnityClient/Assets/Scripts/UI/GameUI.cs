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
using DG.Tweening;

public class GameUI : MonoBehaviour {
	public UserView Player;
	public UserView Enemy;
	public Button TurnButton;
	public CardView CardPrefab;

	Dictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

	// TODO: States
	bool _inBuySelection;
	bool _inAttackSelection;
	int  _selectedHandIndex;
	int  _selectedTableIndex;

	void Awake() {
		TurnButton.onClick.AddListener(OnTurnClick);

		AddHandler<TurnAction>(OnTurnAction);
		AddHandler<BuyCreatureAction>(OnBuyCreatureAction);
	}

	void AddHandler<T>(Action<T> handler) where T : class, IGameAction {
		_handlers.Add(typeof(T), a => handler(a as T));
	}

	void Start() {
		Player.Hand.Init(OnPlayerHandCardClick);
		Player.Table.Init(OnPlayerTableCardClick);
		Enemy.Table.Init(OnEnemyTableCardClick);
		Enemy.User.Init(OnEnemyUserCardClick);

		Events.Subscribe<Game_Init>(this, OnGameInit);
		Events.Subscribe<Game_Reload>(this, OnGameReload);
		Events.Subscribe<Game_End>(this, OnGameEnd);
		Events.Subscribe<Common_Error>(this, OnGameError);
		Events.Subscribe<Game_NewAction>(this, OnGameNewAction);

		Game.Start();
	}

	void OnDestroy() {
		Events.Unsubscribe<Game_Init>(OnGameInit);
		Events.Unsubscribe<Game_Reload>(OnGameReload);
		Events.Unsubscribe<Game_End>(OnGameEnd);
		Events.Unsubscribe<Common_Error>(OnGameError);
		Events.Unsubscribe<Game_NewAction>(OnGameNewAction);
	}

	void Update() {
		Game.Update();
	}

	void OnGameInit(Game_Init e) {
		FullStateUpdate(e.State);
	}

	void OnGameError(Common_Error e) {
		NoticeWindow.ShowWithOkButton("Server Error", e.Text, Game.ApplyEnd);
	}

	void OnGameEnd(Game_End e) {
		NoticeWindow.ShowWithOkButton("Game Ended", string.Format("Winner is '{0}'", e.Winner), Game.ApplyEnd);
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
			DrawUserState(user, active, true);
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

	bool GetHandCardInteractable(bool activeUser, CardState card) {
		return activeUser && Game.CanBought(card);
	}

	void DrawUserState(UserState userState, bool activeUser, bool full) {
		var view = SelectView(userState);
		var nameText = userState.Name;
		if ( activeUser ) {
			nameText = string.Format("<b>{0}</b>", nameText);
		}
		view.NameText.text = nameText;
		view.HPText.text = string.Format("HP: {0}/{1}", userState.Health, userState.MaxHealth);
		view.PowerText.text = string.Format("PW: {0}/{1}", userState.Power, userState.MaxPower);

		if ( !full ) {
			return;
		}
		UpdateUserHolder(false, view.User);
		UpdateGlobalCount(userState.GlobalSet.Count, view.Global);
		UpdateCards(userState.HandSet, view.Hand, (card) => GetHandCardInteractable(activeUser, card));
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

	CardView CreateCardView(CardState card, Func<CardState, bool> interactable) {
		var cardView = ObjectPool.Spawn(CardPrefab);
		var interactableState = (interactable != null) && interactable(card);
		if ( card == null ) {
			cardView.InitPlaceholder(interactableState);
		} else {
			cardView.Init(true, interactableState, card.Type.ToString(), card.Price, card.Damage, card.Health, card.MaxHealth);
		}
		return cardView;
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
			var cardView = CreateCardView(card, interactable);
			set.Add(cardView);
		}
	}

	void OnTurnClick() {
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

	void OnGameNewAction(Game_NewAction e) {
		var type = e.Action.GetType();
		foreach ( var handler in _handlers ) {
			if ( handler.Key == type ) {
				handler.Value(e.Action);
				return;
			}
		}
		Log.WarningFormat("No handler for action: '{0}'", LogTags.Game, e.Action.GetType());
		OnCommonAction();
	}

	void OnCommonAction() {
		FullStateUpdate(Game.State);
		Game.EndCurrentAction();
	}

	void PerformCommonCallback() {
		Game.EndCurrentAction();
		var isUserTurn = User.Name == Game.State.TurnOwner;
		DrawUserState(Game.GetUserState(), isUserTurn, false);
		DrawUserState(Game.GetEnemyState(), false, false);
	}

	void OnTurnAction(TurnAction action) {
		Action callback = PerformCommonCallback;
		var state = GetUserState(action);
		var view = GetUserView(action);
		if ( CheckNewCards(state, view.Hand, callback) ) {
			return;
		}
		callback();
	}

	UserView GetUserView(BaseGameAction action) {
		return GetUserView(action.User);
	}

	UserView GetUserView(string userName) {
		if ( userName == User.Name ) {
			return Player;
		}
		return Enemy;
	}

	UserState GetUserState(BaseGameAction action) {
		return GetUserState(action.User);
	}

	UserState GetUserState(string userName) {
		if ( userName == User.Name ) {
			return Game.GetUserState();
		}
		return Game.GetEnemyState();
	}

	void OnBuyCreatureAction(BuyCreatureAction action) {
		Action callback = PerformCommonCallback;
		var userView = GetUserView(action);
		var hand = userView.Hand;
		var table = userView.Table;
		var cardToSpawn = hand.Cards[action.HandIndex];
		var oldPos = cardToSpawn.Root.position;
		cardToSpawn.Root.SetParent(GameObject.FindObjectOfType<Canvas>().transform, true);
		hand.Remove(cardToSpawn, false);
		var cardToReplace = table.Cards[action.TableIndex];
		var newPos = cardToReplace.Root.transform.position;
		table.Remove(cardToReplace, true);
		table.Insert(cardToSpawn, action.TableIndex);
		var seq = DOTween.Sequence();
		seq.AppendInterval(0.01f);
		seq.AppendCallback(() => cardToSpawn.Root.SetParent(cardToSpawn.transform, true));
		seq.Append(cardToSpawn.Root.DOLocalMove(Vector3.zero, 0.33f, true));
		cardToSpawn.SetEffect(seq, callback);
	}

	bool CheckNewCards(UserState userState, CardSet handSet, Action callback) {
		var userHand = userState.HandSet;
		if ( userHand.Count > handSet.Cards.Count ) {
			var isActiveUser = userState.Name == User.Name;
			var cardToSpawn = userHand[userHand.Count - 1];
			var cardView = CreateCardView(cardToSpawn, card => GetHandCardInteractable(isActiveUser, card));
			handSet.Add(cardView);
			var trans = cardView.transform;
			trans.localScale = Vector3.zero;
			cardView.SetEffect(trans.DOScale(Vector3.one, 0.33f), callback);
			return true;
		}
		return false;
	}
}
