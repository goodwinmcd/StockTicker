using System;
using System.Collections.Generic;

namespace Common.Models
{
    public class StockTickerUi
    {
        public IEnumerable<StockTickerCountDb> Tickers { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public string Source { get; set; } = "All";
        public string SelectedDateRange { get; set; } = "One Day";
    }
}