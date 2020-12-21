using System;

namespace Common.Models
{
    public class StockTicker
    {
        public String NasdaqSymbol { get; set; }
        public String Exchange { get; set; }
        public String SecurityName { get; set; }
    }
}