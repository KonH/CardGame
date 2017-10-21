using Server.Models;
using System.Collections.Generic;

namespace Server.Repositories {
	public interface IUserRepository {
		User Find(string login, string password);
		IEnumerable<User> All { get; }
	}
}
