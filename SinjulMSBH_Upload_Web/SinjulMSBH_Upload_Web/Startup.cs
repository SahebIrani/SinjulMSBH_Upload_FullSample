using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SinjulMSBH_Upload_Web
{
	public class Startup
	{
		// This is not the best idea to use a static variable (especially when you have more than one active uploading session at the same time)
		public static int Progress { get; set; }

		public Startup ( IConfiguration configuration )
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices ( IServiceCollection services )
		{
			services.AddMvc( );
			services.AddAntiforgery( o => o.HeaderName = "XSRF-TOKEN" );
			services.Configure<FormOptions>( x =>
			{
				x.ValueLengthLimit = int.MaxValue;
				x.MultipartBodyLengthLimit = int.MaxValue;
			} );
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure ( IApplicationBuilder app , IHostingEnvironment env )
		{
			if ( env.IsDevelopment( ) )
			{
				app.UseBrowserLink( );
				app.UseDeveloperExceptionPage( );
			}
			else
			{
				app.UseExceptionHandler( "/Home/Error" );
			}

			app.UseStaticFiles( );

			app.UseMvc( routes =>
			 {
				 routes.MapRoute(
			    name: "default" ,
			    template: "{controller=Home}/{action=Index}/{id?}" );
			 } );
		}
	}
}