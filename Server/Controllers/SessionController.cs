using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharedLibrary.Models;
using Server.Repositories;
using Microsoft.Extensions.Configuration;

namespace Server.Controllers {
	[Authorize]
	[Produces("application/json")]
	[Route("api/session")]
	public class SessionController : Controller {
		ILogger _logger;
		bool _withAutoConnect;
		SessionRepository _sessions;
		IUserRepository _users;

		public SessionController(ILoggerFactory loggingFactory, IConfiguration config, SessionRepository sessions, IUserRepository users) {
			_logger = loggingFactory.CreateLogger<SessionController>();
			_withAutoConnect = bool.Parse(config["Simple-Bot"]);
			_sessions = sessions;
			_users = users;
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
				TryUseAutoConnect(session);
				return Json(session);
			}
			_logger.LogWarning("Can't create session");
			return BadRequest("Can't create session");
		}

		void TryUseAutoConnect(Session session) {
			if ( _withAutoConnect ) {
				var anotherUser = _users.All.First(u => u.Name != session.Owner).Name;
				session.Users.Add(anotherUser);
			}
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(string id) {
			var session = _sessions.Find(id);
			if ( session != null ) {
				var isUserSession = (User.Identity.Name == session.Owner);
				if ( isUserSession && _sessions.TryDelete(id) ) {
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
	}
}