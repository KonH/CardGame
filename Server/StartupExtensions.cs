using Server.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Server {
	public static class StartupExtensions {
		public static void AddFullCors(this IServiceCollection services) {
			services.AddCors(o => o.AddPolicy("FullCorsPolicy", builder => {
				builder
					.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader();
			}));
		}

		public static void UseFullCors(this IApplicationBuilder app) {
			app.UseCors("FullCorsPolicy");
		}

		public static void AddJwtBearerAuthentication(this IServiceCollection services) {
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options => {
					options.RequireHttpsMetadata = false;
					options.TokenValidationParameters = new TokenValidationParameters {
						ValidateIssuer           = true,
						ValidIssuer              = AuthSettings.Issuer,
						ValidateAudience         = true,
						ValidAudience            = AuthSettings.Audience,
						ValidateLifetime         = true,
						IssuerSigningKey         = AuthSettings.GetSymmetricSecurityKey(),
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

		public static void AddGameRepository(this IServiceCollection services) {
			services.AddSingleton<GameRepository>();
		}
	}
}
