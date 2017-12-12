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
	abstract class UIState {
		public virtual bool CanTurn { get; }
		public virtual bool IsPlayerHandActive(CardState card) => false;
		public virtual bool IsPlayerTableActive(CardState card) => false;
		public virtual bool IsEnemyTableActive(CardState card) => false;
		public virtual bool IsEnemyUserActive(CardState card) => false;
	}

	class EmptyState : UIState {

	}

	class PlayerTurnState : UIState {
		public override bool CanTurn => true;

		public override bool IsPlayerHandActive(CardState card) {
			return ( card != null ) && Game.CanBought(card);
		}

		public override bool IsPlayerTableActive(CardState card) {
			return ( card != null ) && ( card.Actions > 0 );
		}
	}

	class PlayerBuyState : UIState {
		public int HandIndex { get; private set; }

		public PlayerBuyState(int handIndex) {
			HandIndex = handIndex;
		}

		public override bool IsPlayerHandActive(CardState card) {
			return (card != null) && (Game.GetUserState().HandSet.IndexOf(card) == HandIndex);
		}

		public override bool IsPlayerTableActive(CardState card) {
			return (card == null);
		}
	}

	class PlayerSelectTargetState : UIState {
		public int TableIndex { get; private set; }

		public PlayerSelectTargetState(int tableIndex) {
			TableIndex = tableIndex;
		}

		public override bool IsEnemyUserActive(CardState card) {
			return true;
		}

		public override bool IsEnemyTableActive(CardState card) {
			return card != null;
		}
	}

	class EnemyTurnState : UIState {}

	class EndGameState : UIState {}

	public UserView Player;
	public UserView Enemy;
	public Button TurnButton;
	public CardView CardPrefab;

	Dictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

	UIState _state;
	UIState State {
		get {
			return _state;
		}
		set {
			_state = value;
			OnStateChanged(_state);
		}
	}

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

		State = new EmptyState();

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
		State = new EmptyState();
		var users = state.Users;
		foreach ( var user in users ) {
			var active = user.Name == state.TurnOwner;
			DrawUserState(user, active, true);
		}
		StartActualState();
		UpdateControls();
	}

	void StartActualState() {
		if ( User.Name == Game.State.TurnOwner ) {
			State = new PlayerTurnState();
		} else {
			State = new EnemyTurnState();
		}
	}

	UserView SelectView(UserState userState) {
		if ( userState.Name == User.Name ) {
			return Player;
		}
		return Enemy;
	}

	void UpdateControls() {
		TurnButton.interactable = State.CanTurn;
		var playerState = Game.GetUserState();
		var enemyState = Game.GetEnemyState();
		UpdateCardInteraction(playerState?.HandSet, Player.Hand, State.IsPlayerHandActive);
		UpdateCardInteraction(playerState?.TableSet, Player.Table, State.IsPlayerTableActive);
		UpdateCardInteraction(enemyState?.TableSet, Enemy.Table, State.IsEnemyTableActive);
		UpdateCardInteraction(Enemy.User, State.IsEnemyUserActive);
	}

	void UpdateCardInteraction(List<CardState> states, CardSet set, Func<CardState, bool> isInteractive) {
		for ( var i = 0; i < set.Cards.Count; i++ ) {
			var state = states?[i];
			var view = set.Cards[i];
			view.UpdateInteractable(isInteractive(state));
		}
	}

	void UpdateCardInteraction(CardSet set, Func<CardState, bool> isInteractive) {
		foreach ( var view in set.Cards ) {
			view.UpdateInteractable(isInteractive(null));
		}
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
		UpdateUserHolder(view.User);
		UpdateGlobalCount(userState.GlobalSet.Count, view.Global);
		UpdateCards(userState.HandSet, view.Hand);
		UpdateTableSet(userState, view.Table);
	}

	void UpdateTableSet(UserState userState, CardSet set) {
		UpdateCards(userState.TableSet, set);
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

	CardView CreateCardView(CardState card) {
		var cardView = ObjectPool.Spawn(CardPrefab);
		if ( card == null ) {
			cardView.InitPlaceholder();
		} else {
			cardView.Init(true, card.Type.ToString(), card.Price, card.Damage, card.Health, card.MaxHealth);
		}
		return cardView;
	}

	void UpdateUserHolder(CardSet userSet) {
		var userCardView = GetFirstOrCreateView(userSet);
		userCardView.InitPlaceholder();
	}

	void UpdateGlobalCount(int count, CardSet global) {
		var globalCardView = GetFirstOrCreateView(global);
		globalCardView.Init(true, count.ToString(), 0, 0, 0, 0);
	}

	void UpdateCards(List<CardState> cards, CardSet set) {
		set.Clear();
		for ( var i = 0; i < cards.Count; i++ ) {
			var card = cards[i];
			var cardView = CreateCardView(card);
			set.Add(cardView);
		}
	}

	void OnTurnClick() {
		Game.NextTurn();
	}

	void OnPlayerHandCardClick(int index) {
		if ( State is PlayerBuyState ) {
			State = new PlayerTurnState();
		} else {
			State = new PlayerBuyState(index);
		}
	}

	void OnPlayerTableCardClick(int index) {
		var userState = Game.GetUserState();
		var buyState = State as PlayerBuyState;
		if ( buyState != null ) {
			if ( userState.TableSet[index] == null ) {
				TryMakeBuyAction(buyState.HandIndex, index);
			}
		} else {
			var selectTargetState = State as PlayerSelectTargetState;
			if ( selectTargetState != null ) {
				State = new PlayerTurnState();
			} else {
				State = new PlayerSelectTargetState(index);
			}
		}
	}

	void TryMakeBuyAction(int handIndex, int tableIndex) {
		State = new PlayerTurnState();
		var action = new BuyCreatureAction("", handIndex, tableIndex);
		Game.ApplyAction(action);
	}

	void OnEnemyTableCardClick(int index) {
		var selectTargetState = State as PlayerSelectTargetState;
		if ( selectTargetState != null ) {
			State = new PlayerTurnState();
			var action = new AttackCreatureAction("", selectTargetState.TableIndex, index);
			Game.ApplyAction(action);
		}
	}

	void OnEnemyUserCardClick(int index) {
		var selectTargetState = State as PlayerSelectTargetState;
		if ( selectTargetState != null ) {
			State = new PlayerTurnState();
			var action = new AttackPlayerAction("", selectTargetState.TableIndex);
			Game.ApplyAction(action);
		}
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
		UpdateControls();
	}

	void OnTurnAction(TurnAction action) {
		Action callback = () => {
			PerformCommonCallback();
			StartActualState();
		};
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
			var cardToSpawn = userHand[userHand.Count - 1];
			var cardView = CreateCardView(cardToSpawn);
			handSet.Add(cardView);
			var trans = cardView.transform;
			trans.localScale = Vector3.zero;
			cardView.SetEffect(trans.DOScale(Vector3.one, 0.33f), callback);
			return true;
		}
		return false;
	}

	void OnStateChanged(UIState state) {
		Log.MessageFormat("New UI State: {0}", LogTags.UI, state.GetType());
		UpdateControls();
	}
}
