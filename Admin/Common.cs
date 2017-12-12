using System;
using System.Net.Http;
using System.Threading.Tasks;
using ServerSharedLibrary;
using SharedLibrary.Utils;

namespace Admin {
	public class Common {
		public static async Task<HttpClient> CreateClient() {
			var client = new HttpClient();
			client.BaseAddress = new Uri("http://localhost:8080/");
			await Auth.AuthorizeClient(client, "1", LoginUtils.MakePasswordHash("1", "1"));
			return client;
		}
	}
}
