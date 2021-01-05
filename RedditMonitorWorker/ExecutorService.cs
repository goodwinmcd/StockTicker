using System;
using Autofac;
using Common.RabbitMQ;
using RedditMonitorWorker.Logic;
using RedditMonitorWorker.ServiceConfiguration;
using Topshelf;

namespace RedditMonitorWorker
{
    public class ExecutorService
    {

        private static IRedditConsumer _worker;
        private static IContainer _container;

        private static void BootstrapService()
        {
            _container = Configure();
            _worker = _container.Resolve<IRedditConsumer>();
        }

        public bool Start(HostControl hostControl)
        {
            BootstrapService();
            try
            {
                _worker.Consume();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Stop(HostControl hostControl)
        {
            _container?.Dispose();
            return true;
        }

        private static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            //Registration of Dependencies
            RegisterDependencies(builder);

            var container = builder.Build();

            return container;
        }

        private static void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceConfigurations>().As<IServiceConfigurations, IRabbitConfigurations>().SingleInstance();
            builder.RegisterType<RabbitConsumer>().As<IRabbitConsumer>().SingleInstance();
            builder.RegisterType<StockTickerManager>().As<IStockTickerManager>().SingleInstance();
            builder.RegisterType<RedditConsumer>().As<IRedditConsumer>().SingleInstance();
            builder.RegisterType<RabbitPublisher>().As<IRabbitPublisher>().SingleInstance();
        }
    }
}