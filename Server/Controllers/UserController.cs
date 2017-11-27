using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Server.Repositories;
using ServerSharedLibrary.Models;

namespace Server.Controllers {
	[Authorize(Roles = "admin")]
	[Produces("application/json")]
	[Route("api/user")]
	public class UserController : Controller {
		ILogger         _logger;
		IUserRepository _users;

		public UserController(ILogger<UserController> logger, IUserRepository users) {
			_logger = logger;
			_users  = users;
		}

		[HttpGet]
		public IActionResult Index() {
			return Json(_users.All);
		}

		[HttpPost]
		public IActionResult Add([FromBody] User user) {
			if ( _users.TryAdd(user) ) {
				return Ok();
			}
			return BadRequest();
		}

		[HttpPut]
		public IActionResult Update([FromBody] User user) {
			if ( _users.TryUpdate(user) ) {
				return Ok();
			}
			return BadRequest();
		}

		[HttpDelete]
		public IActionResult Delete(string login) {
			if ( _users.TryDelete(login) ) {
				return Ok();
			}
			return BadRequest();
		}
	}
}