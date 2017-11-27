using System.Collections.Generic;
using Server.Models;
using ServerSharedLibrary.Models;

namespace Server.Repositories {
	public interface IUserRepository {
		IEnumerable<User> All { get; }
		User Find(string login, string password);
	}
}
