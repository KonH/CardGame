using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using ServerSharedLibrary;
using ServerSharedLibrary.Models;
using System.Text;

namespace Admin.Pages {
	public class EditUser : PageModel {
		public User UserToEdit { get; private set; }

		public async Task<IActionResult> OnGetAsync(string login) {
			var client = await Common.CreateClient();
			var responseBody = await client.GetStringAsync("api/user");
			var users = JsonConvert.DeserializeObject<IEnumerable<User>>(responseBody);
			foreach ( var user in users ) {
				if ( user.Login == login ) {
					UserToEdit = user;
				}
			}
			if ( UserToEdit == null ) {
				UserToEdit = ServerSharedLibrary.Models.User.WithPassword("", "", "", "");
			}
			return Page();
		}

		public async Task<IActionResult> OnPostAsync(
			string delete, string oldLogin, string newLogin, string name, string password, string role)
		{
			var client = await Common.CreateClient();
			HttpResponseMessage result;
			if ( !string.IsNullOrEmpty(delete) ) {
				result = await client.DeleteAsync($"api/user?login={oldLogin}");
				return RedirectToPage("Users");
			}
			var login = !string.IsNullOrEmpty(newLogin) ? newLogin : oldLogin;
			var user = ServerSharedLibrary.Models.User.WithPassword(login, password, name, role);
			var jsonContent = JsonConvert.SerializeObject(user);
			var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
			var url = "api/user";
			if ( string.IsNullOrEmpty(oldLogin) ) {
				result = await client.PostAsync(url, content);
			} else {
				result = await client.PutAsync(url, content);
			}
			return RedirectToPage("Users");
		}
	}
}
