using SharedLibrary.Models;
using System.Collections.Generic;

public struct Auth_UpdateHeader {
	public string AuthHeader { get; private set; }

	public Auth_UpdateHeader(string header) {
		AuthHeader = header;
	}
}

public struct Session_ConnectComplete {
	public string Id { get; private set; }

	public Session_ConnectComplete(string id) {
		Id = id;
	}
}

public struct Session_Update {
	public List<Session> Sessions { get; private set; }

	public Session_Update(List<Session> sessions) {
		Sessions = sessions;
	}
}