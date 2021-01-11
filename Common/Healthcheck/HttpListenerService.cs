using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace RedditMonitor.Logic.Healthcheck
{
    public class HttpListenerService : BackgroundService
    {
        private IHealthCheckCustom _healthCheck;
        private HttpListener _listener;

        public HttpListenerService()
        {
            _healthCheck = new HealthCheckCustom();
            _listener = new HttpListener
            {
                Prefixes = { "http://localhost:5001/" }
            };
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _listener.Start();
            while (true)
            {
                var context = await _listener.GetContextAsync();
                var response = context.Response;
                var test = await _healthCheck.CheckHealthAsync(new HealthCheckContext());
                if (test.Status == HealthStatus.Healthy)
                    response.StatusCode = 200;
                else
                    response.StatusCode = 500;
                response.Close();
            }
        }
    }
}