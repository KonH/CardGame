using Server.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Server.Repositories {
	public class InMemoryUserRepository : IUserRepository {
		ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User> {
			["1"] = new User("1", "1", "user1", "user"),
			["2"] = new User("2", "2", "user2", "user")
		};

		public IEnumerable<User> All {
			get {
				return _users.Values;
			}
		}

		public User Find(string login, string password) {
			if ( _users.TryGetValue(login, out User user) ) {
				if ( user.Password == password ) {
					return user;
				}
			}
			return null;
		}
	}
}
