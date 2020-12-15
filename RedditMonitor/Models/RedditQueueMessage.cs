using System;

namespace RedditMonitor.Models
{
    public class RedditQueueMessage
    {
        public RedditMessageType Source { get; set; }
        public String RedditId { get; set; }
        public DateTime TimePosted { get; set; }
        public String Message { get; set; }
    }

    public enum RedditMessageType
    {
        Post,
        Comment
    }
}