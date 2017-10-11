using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Server {
	public class AuthSettings {
		public const string Issuer = "CardServer";
		public const string Audience = "CardClient";
		public const int Lifetime = 60;
		const string Key = "wof7YIoqBlKdpXLJ";

		public static SymmetricSecurityKey GetSymmetricSecurityKey() {
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
		}
	}
}
