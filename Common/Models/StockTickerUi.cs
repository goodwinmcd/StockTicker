using System.Collections.Generic;

namespace Common.Models
{
    public class StockTickerUi
    {
        public IEnumerable<StockTickerWithCount> Tickers { get; set; }
        public int Paging { get; set; }
    }
}