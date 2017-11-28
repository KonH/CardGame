using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.TestHost;
using Server.Models;
using Server.Repositories;
using ServerSharedLibrary;
using ServerSharedLibrary.Models;

namespace Server.Tests {
	public static class Common {
		public static string AuthTokenPath         => Routing.AuthTokenPath;
		public static string AuthTokenPathWithArgs => Routing.AuthTokenPathWithArgs;

		public static async Task<string> GetAuthToken(HttpClient client, User user) {
			var login        = user.Login;
			var passwordHash = user.PasswordHash;
			return await Auth.GetAuthToken(client, login, passwordHash);
		}

		public static void AddToken(HttpClient client, string token) {
			Auth.AddToken(client, token);
		}

		public static T GetService<T>(TestServer server) where T:class {
			return server.Host.Services.GetService(typeof(T)) as T;
		}

		public static IEnumerable<User> FindUsers(TestServer server) {
			var users = GetService<IUserRepository>(server);
			return users.All;
		}

		public static async Task AuthorizeClient(TestServer server, HttpClient client, string userName) {
			var user  = FindUsers(server).First(u => u.Name == userName);
			await Auth.AuthorizeClient(client, user.Login, user.PasswordHash);
		} 
	}
}
