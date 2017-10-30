using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SharedLibrary.Common;
using SharedLibrary.Models;
using SharedLibrary.Models.Game;
using SharedLibrary.Actions;
using SharedLibrary.Utils;

namespace Server.Controllers {
	[Authorize]
	[Produces("application/json")]
	[Route("api/game")]
	public class GameController : Controller {
		static GameState _state = new GameState("", new List<UserState> {
				new UserState("user1", 10),
				new UserState("user2", 10)
			}
		);
		static List<IGameAction> _actions = new List<IGameAction>();

		ILogger _logger;

		public GameController(ILoggerFactory loggerFactory) {
			_logger = loggerFactory.CreateLogger(typeof(GameController));
		}

		[HttpGet("state")]
		public IActionResult GetGameState() {
			// TODO: Filter user state
			return Json(_state);
		}

		IGameAction GetActionForVersion(int version) {
			if ( _actions.Count > version ) {
				return _actions[version];
			}
			return null;
		}

		[HttpGet("action")]
		public IActionResult GetAction(int version) {
			var action = GetActionForVersion(version);
			if ( action != null ) {
				Response.HttpContext.Response.Headers.Add(Interaction.ActionTypeHeader, action.GetType().ToString());
				return Json(action);
			}
			return new EmptyResult();
		}

		[HttpPost("action")]
		public IActionResult PostAction(int version, string type, [FromBody] JObject action) {
			var typeInstance = ReflectionUtils.GetActionType(type);
			if ( typeInstance != null ) {
				var actionInstance = action.ToObject(typeInstance) as IGameAction;
				if ( actionInstance != null ) {
					lock ( _state ) {
						if ( _state.TryApply(actionInstance) ) {
							_actions.Add(actionInstance);
							return Ok();
						}
					}
				}
			}
			return BadRequest();
		}
	}
}