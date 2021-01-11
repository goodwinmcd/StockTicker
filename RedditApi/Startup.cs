using System;
using System.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RedditApi.DataAccess;
using RedditApi.Logic;
using RedditMonitor.Configurations;

namespace RedditApi
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
            InitilizeDb();
            services.AddControllers();
            // var dbConnectionString = ConfigurationManager.ConnectionStrings["pgsql"];
            // services.AddTransient<IDbConnection>((sp) => new NpgsqlConnection(dbConnectionString.ToString()));
            services.AddSingleton<IServiceConfigurations>(new ServiceConfigurations(Configuration));
            services.AddScoped<IStockTickerRepo, StockTickerRepo>();
            services.AddScoped<IStockTickerService, StockTickerService>();
            services.AddScoped<IRedditMessageService, RedditMessageService>();
            services.AddScoped<IRedditMessageRepo, RedditMessageRepo>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RedditApi", Version = "v1" });
            });
        }

        private void InitilizeDb()
        {
            var initializeDb = new InitializeDb(
                Configuration.GetConnectionString("stockTickerConnectionString"),
                Configuration.GetConnectionString("adminConnectionString"));
            initializeDb.InitializeDbAndTablesAsync().Wait();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RedditApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
