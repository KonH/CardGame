using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ServerSharedLibrary;
using ServerSharedLibrary.Models;

namespace Admin.Pages {
	public class UsersModel : PageModel {
		public IEnumerable<User> Users { get; private set; }

		public async Task<IActionResult> OnGetAsync() {
			var client = await Common.CreateClient();
			var responseBody = await client.GetStringAsync("api/user");
			Users = JsonConvert.DeserializeObject<IEnumerable<User>>(responseBody);
			return Page();
		}
	}
}
