using System.Collections.Generic;
using Server.Models;

namespace Server.Repositories {
	public interface IUserRepository {
		IEnumerable<User> All { get; }
		User Find(string login, string password);
	}
}
