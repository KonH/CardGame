using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Server.Controllers {
	[Produces("application/json")]
	[Route("api/auth")]
	public class AuthController : Controller {

		List<User> _users = new List<Models.User> {
			new User() { Login = "1", Password = "2", Role = "user" }
		};

		public object AuthOptions { get; private set; }

		[HttpGet("token")]
		public IActionResult GetToken([FromQuery] string login, [FromQuery] string password) {
			var identity = GetIdentity(login, password);
			if ( identity == null ) {
				return BadRequest("Invalid login or password");
			}

			var now = DateTime.UtcNow;
			var jwt = new JwtSecurityToken(
					issuer: AuthSettings.Issuer,
					audience: AuthSettings.Audience,
					notBefore: now,
					claims: identity.Claims,
					expires: now.Add(TimeSpan.FromMinutes(AuthSettings.Lifetime)),
					signingCredentials: new SigningCredentials(AuthSettings.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			var response = new {
				token = encodedJwt,
				userName = identity.Name
			};
			return Json(response);

		}

		private ClaimsIdentity GetIdentity(string username, string password) {
			var user = _users.FirstOrDefault(x => x.Login == username && x.Password == password);
			if ( user != null ) {
				var claims = new List<Claim>
				{
					new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
					new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
				};
				var claimsIdentity = 
					new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
				return claimsIdentity;
			}
			return null;
		}
	}
}