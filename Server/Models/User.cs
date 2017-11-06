namespace Server.Models {
	public class User {
		public string Login    { get; set; }
		public string Password { get; set; }
		public string Name     { get; set; }
		public string Role     { get; set; }

		public User(string login, string password, string name, string role) {
			Login    = login;
			Name     = name;
			Password = password;
			Role     = role;
		}
	}
}
