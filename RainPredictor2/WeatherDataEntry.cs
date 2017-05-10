using Encog.Util.Arrayutil;
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
        public double NormPreviousMinTemp
        {
            get
            {
                NormalizedField n = new NormalizedField(NormalizationAction.Normalize, "Field", 136, -128.6, 1.0, -1.0);
                return n.Normalize(PreviousMinTemp);
            }
        }

        public double PreviousMaxTemp { get; set; }
        public double NormPreviousMaxTemp
        {
            get
            {
                NormalizedField n = new NormalizedField(NormalizationAction.Normalize, "Field", 136, -128.6, 1.0, -1.0);
                return n.Normalize(PreviousMaxTemp);
            }
        }

        public double PreviousAvgTemp { get; set; }
        public double NormPreviousAvgTemp
        {
            get
            {
                NormalizedField n = new NormalizedField(NormalizationAction.Normalize, "Field", 136, -128.6, 1.0, -1.0);
                return n.Normalize(PreviousAvgTemp);
            }
        }

        public double PreviousMorningHumidity { get; set; }
        public double NormPreviousMorningHumidity
        {
            get
            {
                NormalizedField n = new NormalizedField(NormalizationAction.Normalize, "Field", 100, 0, 1.0, 0);
                return n.Normalize(PreviousMorningHumidity);
            }
        }

        public double PreviousMorningPressure { get; set; }
        public double NormPreviousMorningPressure
        {
            get
            {
                NormalizedField n = new NormalizedField(NormalizationAction.Normalize, "Field", 32.06, 25.69, 1.0, -1.0);
                return n.Normalize(PreviousMorningPressure);
            }
        }

        public double PreviousMorningPressureDifference { get; set; }
        public double NormPreviousMorningPressureDifference
        {
            get
            {
                NormalizedField n = new NormalizedField(NormalizationAction.Normalize, "Field", 6.37, -6.37, 1.0, -1.0);
                return n.Normalize(PreviousMorningPressureDifference);
            }
        }

        public double MorningTemp { get; set; }
        public double NormMorningTemp
        {
            get
            {
                NormalizedField n = new NormalizedField(NormalizationAction.Normalize, "Field", 136, -128.6, 1.0, -1.0);
                return n.Normalize(MorningTemp);
            }
        }

        public double MorningPressure { get; set; }
        public double NormMorningPressure
        {
            get
            {
                NormalizedField n = new NormalizedField(NormalizationAction.Normalize, "Field", 32.06, 25.69, 1.0, -1.0);
                return n.Normalize(MorningPressure);
            }
        }

        public double Rain { get; set; }

    }

}
