﻿using SharedLibrary.Utils;
using SharedLibrary.Common;
using SharedLibrary.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Server.Models;
using Server.Repositories;
using Newtonsoft.Json.Linq;

namespace Server.Controllers {
	[Authorize]
	[Produces("application/json")]
	[Route("api/game")]
	public class GameController : Controller {
		ILogger        _logger;
		GameRepository _games;

		public GameController(GameRepository games, ILogger<GameController> logger) {
			_games  = games;
			_logger = logger;
		}

		[HttpGet("state")]
		public IActionResult GetGameState(string session) {
			var state = _games.Find(session);
			if ( state != null ) {
				var userName = User.Identity.Name;
				return Json(state.SharedState.Filter(userName));
			}
			return BadRequest("Can't find game");
		}

		IGameAction GetActionForVersion(ServerGameState state, int version) {
			if ( state.Actions.Count > version ) {
				return _games.FilterAction(state.Actions[version], User.Identity.Name);
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
						if ( _games.TryApplyAction(state, actionInstance) ) {
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