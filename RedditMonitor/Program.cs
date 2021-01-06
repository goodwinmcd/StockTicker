using System;
using System.Configuration;
using System.Threading.Tasks;
using Common.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using RedditMonitor.Logic;
using RedditMonitor.Logic.Twitter;
using RedditMonitor.Configurations;
using Tweetinvi;
using Tweetinvi.Models;
using Microsoft.Extensions.Configuration;

namespace RedditMonitor
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            RegisterServices();
            var redditMonitoring = _serviceProvider.GetService<IRedditMonitoring>();
            var twitterMonitoring = _serviceProvider.GetService<ITwitterMonitoring>();
            var redditTask = redditMonitoring.MonitorPostsAsync();
            var twitterTask = twitterMonitoring.MonitorTweetsAsync();
            Task.WaitAll(twitterTask, redditTask);
            DisposeServices();
        }


        private static void RegisterServices()
        {
            var services = new ServiceCollection();
            var appConfigurations = RegisterConfigurations();
            var configurations = new ServiceConfigurations(appConfigurations);
            services.AddSingleton<IRabbitConfigurations>(configurations);
            services.AddSingleton<IServiceConfigurations>(configurations);
            services.AddSingleton<IRedditMonitoring, RedditMonitoring>();
            services.AddSingleton<ITwitterMonitoring, TwitterMonitoring>();
            services.AddSingleton<IRabbitPublisher, RabbitPublisher>();
            services.AddSingleton<ITwitterClient>(new TwitterClient(BuildTwitterCreds(configurations)));
            _serviceProvider = services.BuildServiceProvider(true);
        }

        private static IConfiguration RegisterConfigurations()
            => new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

        private static TwitterCredentials BuildTwitterCreds(IServiceConfigurations serviceConfigurations)
        {
            return new TwitterCredentials(
                serviceConfigurations.TwitterApiKey,
                serviceConfigurations.TwitterApiSecret,
                serviceConfigurations.TwitterAccessToken,
                serviceConfigurations.TwitterAccessSecret)
            {
                BearerToken = serviceConfigurations.TwitterBearerToken // bearer token is optional in some cases
            };
        }

        private static void DisposeServices()
        {
            if(_serviceProvider == null)
                return;
            if (_serviceProvider is IDisposable)
                ((IDisposable)_serviceProvider).Dispose();
        }
    }
}
