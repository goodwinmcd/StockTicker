using System.Collections.Generic;

namespace Common.Models
{
    public class StockTickerUi
    {
        public IEnumerable<StockTickerCountDb> Tickers { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
    }
}