using System.Collections.Generic;
using System.Collections.Concurrent;
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

		public bool TryAdd(User user) {
			if ( (user == null) || (string.IsNullOrEmpty(user.Login)) ) {
				return false;
			}
			if ( _users.ContainsKey(user.Login) ) {
				return false;
			}
			return _users.TryAdd(user.Login, user);
		}

		public bool TryDelete(string login) {
			if ( string.IsNullOrEmpty(login) ) {
				return false;
			}
			return _users.TryRemove(login, out User _);
		}

		public bool TryUpdate(User user) {
			if ( (user == null) || (string.IsNullOrEmpty(user.Login)) ) {
				return false;
			}
			if ( _users.TryGetValue(user.Login, out User oldUser) ) {
				return _users.TryUpdate(user.Login, user, oldUser);
			}
			return false;
		}
	}
}
