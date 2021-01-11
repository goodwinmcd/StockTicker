using System.Collections.Generic;

namespace StockTickerUi.Models
{
    public class StockTickerView
    {
        public IEnumerable<StockTickerWithCount> Tickers { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public string Source { get; set; } = "All";
        public string SelectedDateRange { get; set; } = "One Day";
    }
}