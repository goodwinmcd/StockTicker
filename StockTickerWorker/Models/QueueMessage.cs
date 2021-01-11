using System;

namespace StockTickerWorker.Models
{
    public class QueueMessage
    {
        public Guid TraceId { get; set; }
        public FoundMessage MessageContent { get; set; }
        public int RetryCount { get; set; } = 2;
    }
}