using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Geek.IdentityServer4.ApiDemo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc(opt => opt.EnableEndpointRouting = false);
            services.AddControllers();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", opt =>
                 {
                     opt.MetadataAddress = "http://localhost:5000";
                     //  opt.Authority = "http://localhost:5000";
                     opt.RequireHttpsMetadata = false;

                     opt.Audience = "api1";
                 });
            //.AddIdentityServerAuthentication("Bearer1", opt =>
            // {
            //     opt.ApiName = "api1";

            //     opt.Authority = "http://localhost:5000";
            //     opt.RequireHttpsMetadata = false;
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
