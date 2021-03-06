﻿using System.Collections.Generic;
using SharedLibrary.Common;
using SharedLibrary.Models;
using SharedLibrary.Models.Game;

namespace SharedLibrary_Tests {
    static class Common {
		public static ICollection<UserState> Users {
			get {
				return new List<UserState>().With("1", 1, 1).With("2", 1, 1);
			}
		}
		public static GameState GameState {
			get {
				return new GameState(Users, "1");
			}
		} 
	}
}
