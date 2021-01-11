namespace StockTickerUi.Models
{
    public class StockTickerWithCount : StockTicker
    {
        public int CountOfOccurences { get; set; }
        public double DailyChangeInVolume { get; set; }
    }
}