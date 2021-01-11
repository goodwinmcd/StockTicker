using System;

namespace RedditData.Models
{
    public class TimeFrameSelection
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public enum DateSelectionOptions
        {
            oneDay,
            twoDays,
            oneWeek
        }

        public TimeFrameSelection(string timeFrame)
        {
            SetTimeFrameDates(timeFrame);
        }

        private void SetTimeFrameDates(string timeFrame)
        {
            var successfulParse = Enum.TryParse<DateSelectionOptions>(timeFrame, out var dateSelection);
            if (successfulParse)
            {
                switch(dateSelection)
                {
                    case DateSelectionOptions.oneDay:
                        StartDate = DateTime.Now.AddDays(-1);
                        EndDate = DateTime.Now;
                        break;
                    case DateSelectionOptions.twoDays:
                        StartDate = DateTime.Now.AddDays(-2);
                        EndDate = DateTime.Now;
                        break;
                    case DateSelectionOptions.oneWeek:
                        StartDate = DateTime.Now.AddDays(-7);
                        EndDate = DateTime.Now;
                        break;
                }
            }
            else
            {
                StartDate = DateTime.Now.AddDays(-1);
                EndDate = DateTime.Now;
            }
        }
    }
}