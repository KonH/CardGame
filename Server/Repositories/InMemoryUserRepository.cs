using System.Collections.Generic;
using System.Collections.Concurrent;
using Server.Models;
using ServerSharedLibrary.Models;

namespace Server.Repositories {
	public class InMemoryUserRepository : IUserRepository {
		ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User> {
			["1"] = new User("1", "1", "user1", "admin"),
			["2"] = new User("2", "2", "user2", "user"),
			["3"] = new User("3", "3", "user3", "user"),
			["4"] = new User("4", "4", "user4", "user")
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
