using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServerSharedLibrary;
using ServerSharedLibrary.Models;

namespace Admin.Components {
	public class UsersViewComponent : ViewComponent {
		public async Task<IViewComponentResult> InvokeAsync() {
			var client = new HttpClient();
			client.BaseAddress = new Uri("http://localhost:8080/");
			await Auth.AuthorizeClient(client, "1", "1");
			var responseBody = await client.GetStringAsync("api/user");
			var users = JsonConvert.DeserializeObject<IEnumerable<User>>(responseBody);
			return View("UsersView", users);
		}
	}
}
