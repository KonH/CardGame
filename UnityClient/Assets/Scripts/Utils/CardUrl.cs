static class CardUrl {
	public static string Prepare(string url) {
		return string.Format(url, Configs.BaseUrl);
	}

	public static string Prepare(string url, params string[] parts) {
		var newParts = new string[parts.Length + 1];
		newParts[0] = Configs.BaseUrl;
		for ( var i = 0; i < parts.Length; i++ ) {
			newParts[i + 1] = parts[i];
		}
		return string.Format(url, newParts);
	}
}
