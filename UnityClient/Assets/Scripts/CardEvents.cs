using System.Collections.Generic;
using SharedLibrary.Models;
using SharedLibrary.Actions;

public struct Common_Error {
	public string Text { get; private set; }

	public Common_Error(string text) {
		Text = text;
	}
}

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

public struct Game_Init {
	public GameState State { get; private set; }
	
	public Game_Init(GameState state) {
		State = state;
	}
}

public struct Game_Reload {
	public GameState State { get; private set; }

	public Game_Reload(GameState state) {
		State = state;
	}
}

public struct Game_End {
	public string Winner { get; private set; }

	public Game_End(string winner) {
		Winner = winner;
	}
}

public struct Game_NewAction {
	public IGameAction Action { get; private set; }

	public Game_NewAction(IGameAction action) {
		Action = action;
	}
}