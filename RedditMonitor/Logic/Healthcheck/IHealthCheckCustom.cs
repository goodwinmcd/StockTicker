using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RedditMonitor.Logic.Healthcheck
{
    public interface IHealthCheckCustom : IHealthCheck
    {
        void SetHealthToFailing();
        void SetHealthToHealthy();
    }
}