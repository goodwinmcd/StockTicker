using System;
using System.Collections.Generic;

namespace StockTickerApi.Models
{
    public class FoundMessage
    {
        public String Source { get; set; }
        public String SubReddit { get; set; }
        public String ExternalId { get; set; }
        public DateTime TimePosted { get; set; }
        public String Message { get; set; }
        public IEnumerable<string> Tickers {get; set; }
        public int Sentiment { get; set; }
    }

    public enum MessageSource
    {
        Twitter,
        Reddit
    }
}