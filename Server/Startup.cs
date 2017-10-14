using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace Server {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }
		
		public void ConfigureServices(IServiceCollection services) {
			services.AddJwtBearerAuthentication();
			services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
			services.AddUserRepository();
			services.AddSessionRepository();
		}
		
		public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
			if ( env.IsDevelopment() ) {
				app.UseDeveloperExceptionPage();
			}

			app.UseAuthentication();
			app.UseMvc();
		}
	}
}
