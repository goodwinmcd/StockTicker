using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Common.Healthcheck
{
    public interface IHealthCheckCustom : IHealthCheck
    {
        void SetHealthToFailing();
        void SetHealthToHealthy();
    }
}