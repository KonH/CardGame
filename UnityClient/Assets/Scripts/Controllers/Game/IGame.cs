using UDBase.Controllers;
using SharedLibrary.Models;
using SharedLibrary.Actions;
using SharedLibrary.Models.Game;

public interface IGame : IController {
	GameState State { get; }
	IGameAction CurrentAction { get; }
	void Start();
	void Update();
	void ApplyAction<T>(T action) where T : IGameAction, new();
	void NextTurn();
	bool CanBought(CardState card);
	UserState GetUserState();
	UserState GetEnemyState();
	void ApplyEnd();
	void EndCurrentAction();
}

