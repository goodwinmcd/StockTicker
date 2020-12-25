using System;
using System.Collections.Generic;

namespace Common.Models
{
    public class RedditMessage
    {
        public RedditMessageType Source { get; set; }
        public String SubReddit { get; set; }
        public String RedditId { get; set; }
        public DateTime TimePosted { get; set; }
        public String Message { get; set; }
        public IEnumerable<string> Tickers {get; set; }
    }

    public enum RedditMessageType
    {
        Post,
        Comment
    }
}