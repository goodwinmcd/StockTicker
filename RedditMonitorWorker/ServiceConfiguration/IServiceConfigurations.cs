namespace RedditMonitorWorker.ServiceConfiguration
{
    public interface IServiceConfigurations
    {
        public string QueueHost { get; }
        public string QueueExchange { get; }
        public string Queue { get; }
        public string QueueUserName { get; }
        public string QueuePassword { get; }
        public int QueuePort { get; }
        public string ApiUrl { get; }
    }
}