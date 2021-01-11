using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StockTickerApi.DataAccess;
using StockTickerApi.Logic;
using StockTickerApi.Configurations;

namespace StockTickerApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            InitilizeDb();
            services.AddControllers();
            services.AddSingleton<IServiceConfigurations>(new ServiceConfigurations(Configuration));
            services.AddScoped<IStockTickerRepo, StockTickerRepo>();
            services.AddScoped<IStockTickerService, StockTickerService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IMessageRepo, MessageRepo>();
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
