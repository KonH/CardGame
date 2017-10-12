using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Server.Repositories;

namespace Server {
	public static class StartupExtensions {
		public static void AddJwtBearerAuthentication(this IServiceCollection services) {
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options => {
					options.RequireHttpsMetadata = false;
					options.TokenValidationParameters = new TokenValidationParameters {
						ValidateIssuer = true,
						ValidIssuer = AuthSettings.Issuer,
						ValidateAudience = true,
						ValidAudience = AuthSettings.Audience,
						ValidateLifetime = true,
						IssuerSigningKey = AuthSettings.GetSymmetricSecurityKey(),
						ValidateIssuerSigningKey = true
					};
				}
			);
		}

		public static void AddUserRepository(this IServiceCollection services) {
			services.AddSingleton<IUserRepository, InMemoryUserRepository>();
		}

		public static void AddSessionRepository(this IServiceCollection services) {
			services.AddSingleton<SessionRepository>();
		}
	}
}
