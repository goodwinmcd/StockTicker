using System;
using System.Configuration;
using System.Threading.Tasks;
using Common.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using RedditMonitor.Logic;
using RedditMonitor.Logic.Twitter;
using Tweetinvi;
using Tweetinvi.Models;

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
            services.AddSingleton<IRedditMonitoring, RedditMonitoring>();
            services.AddSingleton<ITwitterMonitoring, TwitterMonitoring>();
            services.AddSingleton<IRabbitPublisher, RabbitPublisher>();
            services.AddSingleton<ITwitterClient>(new TwitterClient(BuildTwitterCreds()));
            _serviceProvider = services.BuildServiceProvider(true);
        }

        private static TwitterCredentials BuildTwitterCreds()
        {
            var _twitterApiKey = ConfigurationManager.AppSettings["twitter_api_key"];
            var _twitterApiSecret = ConfigurationManager.AppSettings["twitter_api_secret"];
            var _twitterAccessToken = ConfigurationManager.AppSettings["twitter_access_token"];
            var _twitterAccessSecret = ConfigurationManager.AppSettings["twitter_access_secret"];
            var _twitterBearerToken = ConfigurationManager.AppSettings["twitter_bearer_token"];
            return new TwitterCredentials(_twitterApiKey, _twitterApiSecret, _twitterAccessToken, _twitterAccessSecret)
            {
                BearerToken = _twitterBearerToken // bearer token is optional in some cases
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
