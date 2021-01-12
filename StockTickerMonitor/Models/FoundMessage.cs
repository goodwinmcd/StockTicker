using System;
using System.Collections.Generic;

namespace StockTickerMonitor.Models
{
    public class FoundMessage
    {
        public String Source { get; set; }
        public String SubReddit { get; set; }
        public String ExternalId { get; set; }
        public DateTime TimePosted { get; set; }
        public String Message { get; set; }
    }

    public enum MessageSource
    {
        Twitter,
        Reddit
    }
}