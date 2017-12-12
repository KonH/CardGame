using UDBase.Controllers;
using SharedLibrary.Models;
using SharedLibrary.Actions;
using SharedLibrary.Models.Game;

public class Game : ControllerHelper<IGame> {
	public static GameState State {
		get {
			if ( Instance != null ) {
				return Instance.State;
			}
			return null;
		}
	}

	public static IGameAction CurrentAction {
		get {
			if ( Instance != null ) {
				return Instance.CurrentAction;
			}
			return null;
		}
	}

	public static void Start() {
		if ( Instance != null ) {
			Instance.Start();
		}
	}

	public static void Update() {
		if ( Instance != null ) {
			Instance.Update();
		}
	}

	public static void ApplyAction<T>(T action) where T : IGameAction, new() {
		if ( Instance != null ) {
			Instance.ApplyAction(action);
		}
	}

	public static void NextTurn() {
		if ( Instance != null ) {
			Instance.NextTurn();
		}
	}

	public static bool CanBought(CardState card) {
		if ( Instance != null ) {
			return Instance.CanBought(card);
		}
		return false;
	}

	public static UserState GetUserState() {
		if ( Instance != null ) {
			return Instance.GetUserState();
		}
		return null;
	}

	public static UserState GetEnemyState() {
		if ( Instance != null ) {
			return Instance.GetEnemyState();
		}
		return null;
	}

	public static void ApplyEnd() {
		if ( Instance != null ) {
			Instance.ApplyEnd();
		}
	}

	public static void EndCurrentAction() {
		if ( Instance != null ) {
			Instance.EndCurrentAction();
		}
	}
}