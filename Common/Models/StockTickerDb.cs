using System;

namespace Common.Models
{
    public class StockTickerDb
    {
        public String NasdaqSymbol { get; set; }
        public String Exchange { get; set; }
        public String SecurityName { get; set; }
    }
}