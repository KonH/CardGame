using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Server.Controllers {
	[Produces("application/json")]
	[Route("api/auth")]
	public class AuthController : Controller {
		IUserRepository _users;

		public AuthController(IUserRepository users) {
			_users = users;
		}

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

		private ClaimsIdentity GetIdentity(string login, string password) {
			var user = _users.Find(login, password);
			if ( user != null ) {
				var claims = new List<Claim>
				{
					new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
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