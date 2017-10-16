using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models;
using SharedLibrary.Models.Game;

namespace Server.Controllers {
	[Authorize]
	[Produces("application/json")]
	[Route("api/game")]
	public class GameController : Controller {
		GameState _state = new GameState() {
			Users = new List<UserState> {
				new UserState() { Name = "user1", Health = 11, MaxHealth = 20},
				new UserState() { Name = "user2", Health = 12, MaxHealth = 20},
			}
		};

		[HttpGet]
		public IActionResult GetGameState() {
			return Json(_state);
		}
	}
}