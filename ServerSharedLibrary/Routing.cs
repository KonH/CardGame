namespace ServerSharedLibrary {
	public static class Routing {
		public const string AuthTokenPath         = "api/auth/token";
		public const string AuthTokenPathWithArgs = AuthTokenPath + "?login={0}&password={1}";
	}
}
