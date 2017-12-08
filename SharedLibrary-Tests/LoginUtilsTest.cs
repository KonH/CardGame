using System.Linq;
using System.Collections.Generic;
using SharedLibrary.Models.Game;
using Xunit;
using SharedLibrary.Utils;

namespace SharedLibrary_Tests {
    public class LoginUtilsTest {
		
		[Fact]
		public void HashingIsApplied() {
			var input = "123";
			var output = LoginUtils.MakeHash(input);
			Assert.NotEqual(input, output);
		}
	}
}
