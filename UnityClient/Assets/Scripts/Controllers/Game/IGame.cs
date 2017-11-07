using UnityEngine;
using UDBase.Utils;
using UDBase.Controllers;
using SharedLibrary.Actions;
using SharedLibrary.Models.Game;

public interface IGame : IController {
	void Start();
	void Update();
	void ApplyAction<T>(T action) where T : IGameAction, new();
	void NextTurn();
	bool CanBought(CardState card);
	UserState GetUserState();
	UserState GetEnemyState();
	void ApplyEnd();
}

