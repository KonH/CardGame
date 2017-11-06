using System.Linq;
using System.Collections.Generic;
using SharedLibrary.Models.Game;
using Xunit;

namespace SharedLibrary_Tests {
    public class VisibilityTest {
		bool SetIs(List<CardState> set, CardType type) {
			return set.TrueForAll(c => c.Type == type);
		}

		bool SetIsNot(List<CardState> set, CardType type) {
			return set.TrueForAll(c => c.Type != type);
		}

		[Fact]
		public void GlobalSetIsInvisible() {
			var state               = Common.GameState;
			var filteredState       = state.Filter("1");
			var user                = state.Users.First(u => u.Name == "1");
			var filteredUser        = filteredState.Users.First(u => u.Name == "1");
			var anotherUser         = state.Users.First(u => u.Name != "1");
			var filteredAnotherUser = filteredState.Users.First(u => u.Name != "1");
			Assert.Equal(user.GlobalSet.Count, filteredUser.GlobalSet.Count);
			Assert.Equal(anotherUser.GlobalSet.Count, filteredAnotherUser.GlobalSet.Count);
			Assert.True(SetIs(filteredUser.GlobalSet, CardType.Hidden));
			Assert.True(SetIs(filteredAnotherUser.GlobalSet, CardType.Hidden));
		}

		[Fact]
		public void HandSetIsInvisibleForOthers() {
			var state               = Common.GameState;
			var filteredState       = state.Filter("1");
			var user                = state.Users.First(u => u.Name == "1");
			var filteredUser        = filteredState.Users.First(u => u.Name == "1");
			var anotherUser         = state.Users.First(u => u.Name != "1");
			var filteredAnotherUser = filteredState.Users.First(u => u.Name != "1");
			Assert.Equal(user.HandSet.Count, filteredUser.HandSet.Count);
			Assert.Equal(anotherUser.HandSet.Count, filteredAnotherUser.HandSet.Count);
			Assert.True(SetIsNot(filteredUser.HandSet, CardType.Hidden));
			Assert.True(SetIs(filteredAnotherUser.HandSet, CardType.Hidden));
		}
	}
}
