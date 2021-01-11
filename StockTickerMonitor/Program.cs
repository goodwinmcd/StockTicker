﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Amazon.Extensions.NETCore.Setup;
using RedditMonitor.Logic.Healthcheck;

namespace StockTickerMonitor
{
    class Program
    {
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
    }
}
