using Topshelf;

namespace RedditMonitorWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureAndRunWinService();
        }

        private static void ConfigureAndRunWinService()
        {
            var rc = HostFactory.Run(configurator =>
            {
                configurator.EnableServiceRecovery(recoveryConfig => recoveryConfig
                    .RestartService(1)
                    .RestartService(1)
                    .RestartService(1));
                configurator.Service<ExecutorService>(serviceConfig =>
                {
                    serviceConfig.ConstructUsing(context => new ExecutorService());
                    serviceConfig.WhenStarted((service, control) => service.Start(control));
                    serviceConfig.WhenStopped((service, control) => service.Stop(control));
                });
                configurator.RunAsLocalSystem();
            });
        }
    }
}
