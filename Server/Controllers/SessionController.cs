using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Repositories;
using System;

namespace Server.Controllers {
	[Authorize]
	[Produces("application/json")]
	[Route("api/session")]
	public class SessionController : Controller {
		SessionRepository _sessions;

		public SessionController(SessionRepository sessions) {
			_sessions = sessions;
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
				return Json(session);
			}
			return BadRequest("Can't create session");
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(string id) {
			var session = _sessions.Find(id);
			if ( session != null ) {
				if ( (User.Identity.Name == session.Owner) && _sessions.TryDelete(id) ) {
					return Ok();
				}
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
				}
			}
			return BadRequest("Can't connect to session");
		}
	}
}