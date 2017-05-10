using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainPredictorNeuralNetwork
{
    public class WeatherDataEntry
    {
        public DateTime EntryDate { get; set; }
        public double PreviousMinTemp { get; set; }
        public double NormPreviousMinTemp { get; set; }
        public double PreviousMaxTemp { get; set; }
        public double NormPreviousMaxTemp { get; set; }
        public double PreviousAvgTemp { get; set; }
        public double NormPreviousAvgTemp { get; set; }
        public double PreviousMorningHumidity { get; set; }
        public double NormPreviousMorningHumidity { get; set; }
        public double PreviousMorningPressure { get; set; }
        public double NormPreviousMorningPressure { get; set; }
        public double PreviousMorningPressureDifference { get; set; }
        public double NormPreviousMorningPressureDifference { get; set; }
        public double MorningTemp { get; set; }
        public double NormMorningTemp { get; set; }
        public double MorningPressure { get; set; }
        public double NormMorningPressure { get; set; }
        public double Rain { get; set; }

    }

}
