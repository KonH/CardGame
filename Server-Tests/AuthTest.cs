using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Server.Tests {
	public class AuthTest {
		TestServer _server;
		HttpClient _client;

		public AuthTest() {
			_server = new TestServer(new WebHostBuilder()
				.UseStartup<Startup>());
			_client = _server.CreateClient();
		}

		[Fact]
		public async Task CantGetTokenForInvalidUser() {
			var response = await _client.GetAsync(Common.AuthTokenPath);
			Assert.False(response.IsSuccessStatusCode);
		}

		[Fact]
		public async Task CantGetTokenForNotFoundUser() {
			var randStr  = Guid.NewGuid().ToString();
			var response = await _client.GetAsync(string.Format(Common.AuthTokenPathWithArgs, randStr, randStr));
			Assert.False(response.IsSuccessStatusCode);
		}

		[Fact]
		public async Task CanGetTokenForFoundUser() {
			var user         = Common.FindUsers(_server).First();
			var login        = user.Login;
			var passwordHash = user.PasswordHash;
			var response = await _client.GetAsync(string.Format(Common.AuthTokenPathWithArgs, login, passwordHash));
			Assert.True(response.IsSuccessStatusCode);
		}

		[Fact]
		public async Task IsTokenValid() {
			var user  = Common.FindUsers(_server).First();
			var token = await Common.GetAuthToken(_client, user);
			Common.AddToken(_client, token);
			var authorizedResponse = await _client.GetAsync("api/session");
			Assert.True(authorizedResponse.IsSuccessStatusCode);
		}
	}
}
