using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;

namespace RedditMonitor.Logic.RabbitMQ
{
    public class RabbitManager
    {
        private readonly DefaultObjectPool<IModel> _objectPool;

        public RabbitManager(IPooledObjectPolicy<IModel> objectPolicy)
        {
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy, 4);
        }
    }
}