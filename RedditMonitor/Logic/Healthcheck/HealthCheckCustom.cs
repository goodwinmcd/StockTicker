using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RedditMonitor.Logic.Healthcheck
{

    public class HealthCheckCustom : IHealthCheckCustom
    {
        private HealthCheckResult _healthStatus;

        public HealthCheckCustom()
        {
            SetHealthToHealthy();
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() => _healthStatus);
        }

        public void SetHealthToFailing()
        {
            _healthStatus = HealthCheckResult.Degraded();
        }

        public void SetHealthToHealthy()
        {
            _healthStatus = HealthCheckResult.Healthy();
        }
    }
}