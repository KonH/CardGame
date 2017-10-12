using Server.Models;

namespace Server.Repositories {
	public interface IUserRepository {
		User Find(string login, string password);
	}
}
