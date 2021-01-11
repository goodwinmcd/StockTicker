using System;
using System.Threading.Tasks;
using System.Threading;
using Common.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedditMonitor.Configurations;
using RedditMonitor.Logic;
using RedditMonitor.Logic.Twitter;
using Tweetinvi;
using Tweetinvi.Models;
using Microsoft.Extensions.Hosting;
using Amazon.Extensions.NETCore.Setup;

namespace RedditMonitor
{
    public class Worker : BackgroundService
    {

        public IConfiguration Configuration { get; }
        private static IServiceProvider _serviceProvider;
        private static AWSOptions _awsOptions = new AWSOptions
            {
                Profile = "socialmediadata",
                Region = Amazon.RegionEndpoint.USWest2
            };

        public Worker(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            RegisterServices();
            var redditMonitoring = _serviceProvider.GetService<IRedditMonitoring>();
            var twitterMonitoring = _serviceProvider.GetService<ITwitterMonitoring>();
            var redditTask = redditMonitoring.MonitorPostsAsync();
            var twitterTask = twitterMonitoring.MonitorTweetsAsync();
            await Task.WhenAll(twitterTask, redditTask);
            DisposeServices();
        }

        private void RegisterServices()
        {
            IServiceCollection services = new ServiceCollection();
            var configurations = new ServiceConfigurations(Configuration);
            services.AddSingleton<IRabbitConfigurations>(configurations);
            services.AddSingleton<IServiceConfigurations>(configurations);
            services.AddSingleton<IRedditMonitoring, RedditMonitoring>();
            services.AddSingleton<ITwitterMonitoring, TwitterMonitoring>();
            services.AddSingleton<IRabbitPublisher, RabbitPublisher>();
            services.AddSingleton<ITwitterClient>(new TwitterClient(BuildTwitterCreds(configurations)));
            services.AddHealthChecks();
            _serviceProvider = services.BuildServiceProvider(true);
        }

        private TwitterCredentials BuildTwitterCreds(IServiceConfigurations serviceConfigurations)
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

        private void DisposeServices()
        {
            if(_serviceProvider == null)
                return;
            if (_serviceProvider is IDisposable)
                ((IDisposable)_serviceProvider).Dispose();
        }
    }
}