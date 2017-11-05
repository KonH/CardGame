using System.Linq;
using System.Collections.Generic;
using SharedLibrary.Common;

namespace SharedLibrary.Models {
	public class Session {
		public string       Id;
		public List<string> Users = new List<string>();

		public string Owner    => Users.FirstOrDefault();
		public bool   Awaiting => Users.Count < SessionRules.MaxUsersInSession;

		public Session(string id, string creator) {
			Id = id;
			Users.Add(creator);
		}
	}
}
