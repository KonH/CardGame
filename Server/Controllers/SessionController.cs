using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Server.Repositories;
using Server.Helpers;
using SharedLibrary.Common;
using SharedLibrary.Models;

namespace Server.Controllers {
	[Authorize]
	[Produces("application/json")]
	[Route("api/session")]
	public class SessionController : Controller {
		ILogger _logger;
		bool _botPlay;
		SessionRepository _sessions;
		IUserRepository _users;
		GameRepository _games;

		public SessionController(
			ILogger<SessionController> logger,
			IConfiguration config,
			SessionRepository sessions,
			IUserRepository users,
			GameRepository games) 
		{
			_logger = logger;
			_botPlay = config.GetValue("Simple-Bot", false);
			_sessions = sessions;
			_users = users;
			_games = games;
		}

		[HttpGet]
		public IActionResult Index() {
			return Json(_sessions.GetAll());
		}

		[HttpPost]
		public IActionResult Create() {
			var id = Guid.NewGuid().ToString();
			var creator = User.Identity.Name;
			var session = new Session(id, creator);
			if ( _sessions.TryAdd(session) ) {
				_logger.LogDebug("New session: '{0}' (creator: '{1}')", id, creator);
				TryUseAutoConnect(session);
				return Json(session);
			}
			_logger.LogWarning("Can't create session");
			return BadRequest("Can't create session");
		}

		void TryUseAutoConnect(Session session) {
			if ( _botPlay ) {
				var anotherUser = _users.All.First(u => u.Name != session.Owner).Name;
				session.Users.Add(anotherUser);
				TryCreateGame(session, anotherUser);
			}
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(string id) {
			var session = _sessions.Find(id);
			if ( session != null ) {
				var isUserSession = (User.Identity.Name == session.Owner);
				if ( isUserSession && _sessions.TryDelete(id) ) {
					_logger.LogDebug("Delete session: '{0}' (user: '{1}')", id, session.Owner);
					return Ok();
				}
				if ( !isUserSession ) {
					_logger.LogWarning("Can't delete session: It is not user session");
				}
			} else {
				_logger.LogWarning("Can't delete session: Can't find session");
			}
			return BadRequest("Can't delete session");
		}

		[HttpPost("connect/{id}")]
		public IActionResult Connect(string id) {
			var session = _sessions.Find(id);
			if ( (session != null) && (session.Awaiting) ) {
				var userName = User.Identity.Name;
				if ( !session.Users.Contains(userName) ) {
					session.Users.Add(userName);
					_logger.LogDebug("Connect to session: '{0}' (user: '{1}')", id, userName);
					TryCreateGame(session);
					return Ok();
				} else {
					_logger.LogWarning($"Can't connect to session: Already connected");
				}
			} else {
				if ( session == null ) {
					_logger.LogWarning($"Can't connect to session: Can't find session");
				} else if ( !session.Awaiting ) {
					_logger.LogWarning($"Can't connect to session: Session isn't awaiting");
				}
			}
			return BadRequest("Can't connect to session");
		}

		void TryCreateGame(Session session, string botUserName = null) {
			if ( session.Awaiting ) {
				return;
			}
			var rand = new Random(DateTime.Now.Millisecond);
			var turnOwnerIndex = rand.Next(session.Users.Count);
			var turnOwner = session.Users[turnOwnerIndex];
			var gameBuilder = 
				new GameBuilder(session.Id, session.Users, GameRules.DefaultHealth).WithTurnOwner(turnOwner);
			if ( !string.IsNullOrEmpty(botUserName) ) {
				gameBuilder.WithBot(botUserName);
			}
			if ( _games.TryAdd(gameBuilder.Build()) ) {
				_logger.LogDebug("Game for session '{0}' is created (first turn to: '{1}')", session.Id, turnOwner);
			} else {
				_logger.LogError("Can't create game for session: '{0}'", session.Id);
			}
		}
	}
}