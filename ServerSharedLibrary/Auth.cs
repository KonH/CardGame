using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ServerSharedLibrary {
	public class Auth {
		public static async Task<string> GetAuthToken(HttpClient client, string login, string password) {
			var loginResponse = await client.GetAsync(string.Format(Routing.AuthTokenPathWithArgs, login, password));
			loginResponse.EnsureSuccessStatusCode();
			var responseText = await loginResponse.Content.ReadAsStringAsync();
			var tokenObj = JObject.Parse(responseText);
			var token = tokenObj.GetValue("token").ToString();
			return token;
		}

		public static void AddToken(HttpClient client, string token) {
			client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
		}

		public static async Task AuthorizeClient(HttpClient client, string login, string password) {
			var token = await GetAuthToken(client, login, password);
			AddToken(client, token);
		}
	}
}
