using SharedLibrary.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Server.Repositories {
	public class SessionRepository {
		ConcurrentDictionary<string, Session> _sessions = new ConcurrentDictionary<string, Session>();

		public IEnumerable<Session> GetAll() {
			return _sessions.Values;
		}

		public bool TryAdd(Session session) {
			if ( (session == null) || (session.Id == null) ) {
				return false;
			}
			if ( _sessions.ContainsKey(session.Id) ) {
				return false;
			}
			try {
				_sessions.TryAdd(session.Id, session);
				return true;
			} catch {
				return false;
			}
		}

		public Session Find(string id) {
			if ( id != null ) {
				if ( _sessions.TryGetValue(id, out Session value) ) {
					return value;
				}
			}
			return null;
		}

		public bool TryDelete(string id) {
			if ( (id != null) && _sessions.ContainsKey(id) ) {
				return _sessions.TryRemove(id, out Session _);
			}
			return false;
		}
	}
}
