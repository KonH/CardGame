using System.Linq;
using Server.Repositories;
using Xunit;

namespace Server.Tests {
	public class UserTest {
		IUserRepository _users = new InMemoryUserRepository();

		[Fact]
		public void HasUsers() {
			Assert.True(_users.All.Count() > 0);
		}
	}
}
