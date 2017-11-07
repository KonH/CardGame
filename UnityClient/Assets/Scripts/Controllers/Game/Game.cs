using UDBase.Controllers;
using SharedLibrary.Actions;
using SharedLibrary.Models.Game;

public class Game : ControllerHelper<IGame> {
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
}