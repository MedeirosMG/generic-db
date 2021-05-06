using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MyDB.Application.CRUD.DatabaseService;
using MyDB.Application.CRUD.DatabaseService.Interfaces;
using MyDB.Infrastructure.Cache;
using MyDB.Infrastructure.Cache.Interfaces;
using MyDB.Infrastructure.Cache.Utilities;
using MyDB.Infrastructure.Tools;
using MyDB.Infrastructure.Tools.Interfaces;

namespace MyDB
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var teste = Configuration.GetValue<string>("redisConection");
        }

        public IConfiguration Configuration { get; }

        // Application Services
        private void setRepositoriesAndConnections(IServiceCollection services)
        {
            // Infrastructure services
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<IConnectionFactory, ConnectionFactory>();
            services.AddSingleton<IDistributedLock, DistributedLock>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IDatabaseService, DatabaseService>();

            // Controllers services
            services.AddAutoMapper(typeof(Startup));
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // CORS for local use application
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin();
                                      builder.AllowAnyMethod();
                                      builder.AllowAnyHeader();
                                  });
            });


            services.AddControllers();

            setRepositoriesAndConnections(services);

            // Swagger configuration
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "myDB", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Swagger configuration
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "myDB v1");
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(MyAllowSpecificOrigins); // For development use
            }

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
