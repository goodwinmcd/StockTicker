using System;
using System.Collections.Generic;

namespace RedditMonitor.Models
{
    public class FoundMessage
    {
        public String Source { get; set; }
        public String SubReddit { get; set; }
        public String RedditId { get; set; }
        public DateTime TimePosted { get; set; }
        public String Message { get; set; }
    }

    public enum MessageSource
    {
        Twitter,
        Reddit
    }
}