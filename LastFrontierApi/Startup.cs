using LastFrontierApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LastFrontierApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LfContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SqlConnectionString")));
            services.AddMvc();
            /*services.Configure<MvcOptions>(options =>
                {
                    options.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));
                });*/

            services.AddCors(o => o.AddPolicy("AllowSpecificOrigin", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowSpecificOrigin");
            app.UseMvc();
        }
    }
}
