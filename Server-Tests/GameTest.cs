using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Server.Helpers;
using Server.Repositories;
using SharedLibrary.Actions;
using SharedLibrary.Common;
using SharedLibrary.Models.Game;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace Server.Tests {
	public class GameTest {
		TestServer _server;
		HttpClient _client1;
		HttpClient _client2;

		public GameTest() {
			_server = new TestServer(new WebHostBuilder()
				.UseStartup<Startup>());
			_client1 = _server.CreateClient();
			_client2 = _server.CreateClient();
		}

		[Fact]
		public void NewCardIsVisible() {
			var games = Common.GetService<GameRepository>(_server);
			var session = "test";
			var serverState = new GameBuilder(session, new string[] { "1", "2" }, 1).WithTurnOwner("1").Build();
			Assert.True(games.TryAdd(serverState));
			var state1 = serverState.SharedState.Filter("1");
			var state2 = serverState.SharedState.Filter("2");
			Assert.True(games.TryApplyAction(serverState, new TurnAction("1")));
			Assert.True(games.TryApplyAction(serverState, new TurnAction("2")));
			Assert.True(games.TryApplyAction(serverState, new TurnAction("1")));
			foreach ( var action in serverState.Actions ) {
				state1.TryApply(games.FilterAction(action, "1"));
				state2.TryApply(games.FilterAction(action, "2"));
			}

			Assert.True(state1.Users.First(u => u.Name == "1").HandSet.TrueForAll(c => c.Type != CardType.Hidden));
			Assert.True(state1.Users.First(u => u.Name == "2").HandSet.TrueForAll(c => c.Type == CardType.Hidden));

			Assert.True(state2.Users.First(u => u.Name == "2").HandSet.TrueForAll(c => c.Type != CardType.Hidden));
			Assert.True(state2.Users.First(u => u.Name == "1").HandSet.TrueForAll(c => c.Type == CardType.Hidden));
		} 
	}
}
