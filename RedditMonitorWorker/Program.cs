using System;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedditMonitor.Logic.Healthcheck;

namespace RedditMonitorWorker
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        private static AWSOptions _awsOptions = new AWSOptions
            {
                Profile = "socialmediadata",
                Region = Amazon.RegionEndpoint.USWest2
            };

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "production")
                .ConfigureAppConfiguration((hostingContext, builder) => {
                    builder
                        .AddSystemsManager(awsConfigs => {

                            awsConfigs.AwsOptions = _awsOptions;
                            awsConfigs.Path = $"/{hostingContext.HostingEnvironment.EnvironmentName}/StockTicker";
                        }).AddSystemsManager(awsConfigs => {
                            awsConfigs.AwsOptions = _awsOptions;
                            awsConfigs.Path = $"/StockTicker";
                    });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHealthChecks().AddCheck<HealthCheckCustom>("custom_hc");
                    services.AddHostedService<HttpListenerService>();
                    services.AddHostedService<Worker>();
                });

        // static void Main(string[] args)
        // {
        //     ConfigureAndRunWinService();
        // }

        // private static void ConfigureAndRunWinService()
        // {
        //     var rc = HostFactory.Run(configurator =>
        //     {
        //         configurator.EnableServiceRecovery(recoveryConfig => recoveryConfig
        //             .RestartService(1)
        //             .RestartService(1)
        //             .RestartService(1));
        //         configurator.Service<ExecutorService>(serviceConfig =>
        //         {
        //             serviceConfig.ConstructUsing(context => new ExecutorService());
        //             serviceConfig.WhenStarted((service, control) => service.Start(control));
        //             serviceConfig.WhenStopped((service, control) => service.Stop(control));
        //         });
        //         configurator.RunAsLocalSystem();
        //     });
        // }
    }
}
