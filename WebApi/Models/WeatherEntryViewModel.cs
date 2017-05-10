using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class WeatherEntryViewModel
    {
        public double PreviousMinTemp { get; set; }
        public double PreviousMaxTemp { get; set; }
        public double PreviousAvgTemp { get; set; }
        public double PreviousMorningHumidity { get; set; }
        public double PreviousMorningPressure { get; set; }
        public double PreviousMorningPressureDifference { get; set; }
        public double MorningTemp { get; set; }
        public double MorningPressure { get; set; }
    }
}