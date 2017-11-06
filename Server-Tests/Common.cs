using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.TestHost;
using Server.Models;
using Server.Repositories;
using Newtonsoft.Json.Linq;

namespace Server.Tests {
	public static class Common {
		public const string AuthTokenPath         = "api/auth/token";
		public const string AuthTokenPathWithArgs = "api/auth/token?login={0}&password={1}";

		public static async Task<string> GetAuthToken(HttpClient client, User user) {
			var login         = user.Login;
			var password      = user.Password;
			var loginResponse = await client.GetAsync(string.Format(AuthTokenPathWithArgs, login, password));
			loginResponse.EnsureSuccessStatusCode();
			var responseText = await loginResponse.Content.ReadAsStringAsync();
			var tokenObj = JObject.Parse(responseText);
			var token = tokenObj.GetValue("token").ToString();
			return token;
		}

		public static void AddToken(HttpClient client, string token) {
			client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
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
			var token = await GetAuthToken(client, user);
			AddToken(client, token);
		} 
	}
}
