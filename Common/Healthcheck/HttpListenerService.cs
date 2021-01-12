using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Common.Healthcheck
{
    public class HttpListenerService : BackgroundService
    {
        private IHealthCheckCustom _healthCheck;
        private HttpListener _listener;
        private string _port;

        public HttpListenerService(IConfiguration configs)
        {
            _healthCheck = new HealthCheckCustom();
            _port = configs["listenerPort"] ?? "5000";
            _listener = new HttpListener
            {
                Prefixes = { $"http://localhost:{_port}/" }
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