using System.Collections.Generic;
using ServerSharedLibrary.Models;

namespace Server.Repositories {
	public interface IUserRepository {
		IEnumerable<User> All { get; }
		User Find(string login, string password);
		bool TryDelete(string login);
		bool TryAdd(User user);
		bool TryUpdate(User user);
	}
}
