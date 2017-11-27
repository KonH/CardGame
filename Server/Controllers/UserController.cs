using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Server.Repositories;

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
	}
}