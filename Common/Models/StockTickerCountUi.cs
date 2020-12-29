namespace Common.Models
{
    public class StockTickerCountUi
    {
        public string StockTickerId { get; set; }
        public int CountOfOccurences { get; set; }
        public string Exchange { get; set; }
        public string SecurityName { get; set; }
        public double VolumeIncrease { get; set; }
    }
}