using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Models;
using Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Server.Tests {
	public class AuthTest {
		const string _tokenPath = "api/auth/token";
		const string _tokenPathWithArgs = "api/auth/token?login={0}&password={1}";

		TestServer _server;
		HttpClient _client;

		public AuthTest() {
			_server = new TestServer(new WebHostBuilder()
				.UseStartup<Startup>());
			_client = _server.CreateClient();
		}

		[Fact]
		public async Task CantGetTokenForInvalidUser() {
			var response = await _client.GetAsync(_tokenPath);
			Assert.False(response.IsSuccessStatusCode);
		}

		[Fact]
		public async Task CantGetTokenForNotFoundUser() {
			var randStr = Guid.NewGuid().ToString();
			var response = await _client.GetAsync(string.Format(_tokenPathWithArgs, randStr, randStr));
			Assert.False(response.IsSuccessStatusCode);
		}

		IEnumerable<User> FindUsers() {
			var users =_server.Host.Services.GetService(typeof(IUserRepository)) as IUserRepository;
			return users.All;
		}

		[Fact]
		public async Task CanGetTokenForFoundUser() {
			var user = FindUsers().First();
			var login = user.Login;
			var password = user.Password;
			var response = await _client.GetAsync(string.Format(_tokenPathWithArgs, login, password));
			Assert.True(response.IsSuccessStatusCode);
		}

		async Task<string> GetAuthToken(User user) {
			var login = user.Login;
			var password = user.Password;
			var loginResponse = await _client.GetAsync(string.Format(_tokenPathWithArgs, login, password));
			loginResponse.EnsureSuccessStatusCode();
			var responseText = await loginResponse.Content.ReadAsStringAsync();
			var tokenObj = JObject.Parse(responseText);
			var token = tokenObj.GetValue("token").ToString();
			return token;
		}

		void AddToken(string token) {
			_client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
		}

		[Fact]
		public async Task IsTokenValid() {
			var user = FindUsers().First();
			var token = await GetAuthToken(user);
			AddToken(token);
			var authorizedResponse = await _client.GetAsync("api/session");
			Assert.True(authorizedResponse.IsSuccessStatusCode);
		}
	}
}
