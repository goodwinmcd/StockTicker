namespace Common.Models
{
    public class StockTickerUi
    {
        public string StockTickerId { get; set; }
        public int CountOfOccurences { get; set; }
        public string Exchange { get; set; }
        public string SecurityName { get; set; }
        public double VolumeIncrease { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
    }
}