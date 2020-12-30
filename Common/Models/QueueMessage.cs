using System;

namespace Common.Models
{
    public class QueueMessage
    {
        public Guid TraceId { get; set; }
        public FoundMessage MessageContent { get; set; }
        public int RetryCount { get; set; } = 2;
    }
}