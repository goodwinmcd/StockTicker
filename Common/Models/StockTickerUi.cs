using System.Collections.Generic;

namespace Common.Models
{
    public class StockTickerUi
    {
        public IEnumerable<StockTickerWithCount> Tickers { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
    }
}