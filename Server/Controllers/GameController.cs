using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SharedLibrary.Common;
using SharedLibrary.Actions;
using SharedLibrary.Utils;
using Server.Repositories;
using Server.Models;

namespace Server.Controllers {
	[Authorize]
	[Produces("application/json")]
	[Route("api/game")]
	public class GameController : Controller {
		ILogger _logger;
		GameRepository _games;

		public GameController(GameRepository games, ILogger<GameController> logger) {
			_games = games;
			_logger = logger;
		}

		[HttpGet("state")]
		public IActionResult GetGameState(string session) {
			// TODO: Filter user state
			var state = _games.Find(session);
			if ( state != null ) {
				return Json(state.SharedState);
			}
			return BadRequest("Can't find game");
		}

		IGameAction GetActionForVersion(ServerGameState state, int version) {
			if ( state.Actions.Count > version ) {
				return state.Actions[version];
			}
			return null;
		}

		[HttpGet("action")]
		public IActionResult GetAction(string session, int version) {
			var state = _games.Find(session);
			if ( state == null ) {
				return BadRequest("Can't find game");
			}
			var action = GetActionForVersion(state, version);
			if ( action != null ) {
				Response.HttpContext.Response.Headers.Add(Interaction.ActionTypeHeader, action.GetType().ToString());
				return Json(action);
			}
			return new EmptyResult();
		}

		[HttpPost("action")]
		public IActionResult PostAction(string session, int version, string type, [FromBody] JObject action) {
			var state = _games.Find(session);
			if ( state == null ) {
				return BadRequest("Can't find game");
			}
			var typeInstance = ReflectionUtils.GetActionType(type);
			if ( typeInstance != null ) {
				var actionInstance = action.ToObject(typeInstance) as IGameAction;
				if ( actionInstance != null ) {
					lock ( state ) {
						actionInstance.User = User.Identity.Name;
						if ( state.SharedState.TryApply(actionInstance) ) {
							_games.AddAction(state, actionInstance);
							return Ok();
						}
					}
				}
			}
			_logger.LogWarning(
				"[{0}] User: '{1}', action (type: '{2}'): {3}",
				session, User.Identity.Name, type, action.ToString());
			return BadRequest();
		}
	}
}