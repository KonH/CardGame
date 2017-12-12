using SharedLibrary.Utils;

namespace ServerSharedLibrary.Models {
	public class User {
		public string Login        { get; set; }
		public string PasswordHash { get; set; }
		public string Name         { get; set; }
		public string Role         { get; set; }

		public User() { }

		private User(string login, string passwordHash, string name, string role) {
			Login        = login;
			PasswordHash = passwordHash;
			Name         = name;
			Role         = role;
		}

		public static User WithPassword(string login, string password, string name, string role) {
			return new User(login, LoginUtils.MakePasswordHash(login, password), name, role);
		}

		public static User WithHash(string login, string hash, string name, string role) {
			return new User(login, hash, name, role);
		}
	}
}
