using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.NeuralData;
using Encog.Util.Arrayutil;
using FileHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainPredictor2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Importing raw data");
            DownloadResult[] data = GetWeatherData();

            Console.WriteLine("Processing raw data");
            List<WeatherDataEntry> processedData = ProcessData(data);

            Console.WriteLine("Normalizing data");
            NormalizeData(processedData);

            Console.WriteLine("Creating training set");            
            INeuralDataSet trainingSet = CreateTrainingSet(processedData.Where(r => r.EntryDate < new DateTime(2017, 4, 1)).ToList());


            Console.WriteLine("Creating neural network");
            BasicNetwork network = CreateNetwork();


            Console.WriteLine("Training network");
            TrainNetwork(network, trainingSet);

            Console.WriteLine("Network trained. Press any key to see results.");
            Console.ReadKey();
            Console.WriteLine("Neural Network Results:");

            int totalGuesses = 0;
            int totalRight = 0;

            foreach(WeatherDataEntry entry in processedData.Where(r => r.EntryDate >= new DateTime(2017, 4, 1)))
            {
                double[] output = new double[1];
                network.Compute(new double[]
                {
                    entry.NormPreviousMinTemp,
                    entry.NormPreviousMaxTemp,
                    entry.NormPreviousAvgTemp,
                    entry.NormPreviousMorningHumidity,
                    entry.NormPreviousMorningPressure,
                    entry.NormPreviousMorningPressureDifference,
                    entry.NormMorningTemp,
                    entry.NormMorningPressure
                }, output);
                Console.WriteLine($"{entry.EntryDate} {entry.Rain}, Predicted: {output[0].ToString()}");

                if((output[0] > 0.5 ? 1 : 0) == (int)entry.Rain)
                {
                    totalRight++;
                }
                totalGuesses++;
            }

            Console.WriteLine($"{totalGuesses.ToString()} guessed. {totalRight.ToString()} right.");

            /*foreach(BasicMLDataPair pair in trainingSet)
            {
                IMLData output = network.Compute(pair.Input);
                Console.WriteLine(pair.Input[0] + "," + pair.Input[1] + ", actual=" + output[0] + ",ideal=" + pair.Ideal[0]);


            }*/

            Console.WriteLine("Press Any Key to Continue");
            Console.ReadKey();
        }

        static DownloadResult[] GetWeatherData()
        {
            var engine = new FileHelperEngine<DownloadResult>();

            var records = engine.ReadFile(@"C:\Users\Matthew\Downloads\960971.csv");

            DataTable data = FileHelpers.CsvEngine.CsvToDataTable(@"C:\Users\Matthew\Downloads\960971.csv", ',');

            return records;
        }

        static List<WeatherDataEntry> ProcessData(DownloadResult[] data)
        {
            DateTime minDate = data.AsEnumerable().Skip(1).First().DATE;
            DateTime maxDate = data.AsEnumerable().Last().DATE;

            List<WeatherDataEntry> results = new List<WeatherDataEntry>();
            int numberOfDays = (maxDate - minDate).Days;

            DateTime currentDate = minDate.Date.AddDays(1);
            DateTime previousDate = minDate.Date.AddDays(0);

            IEnumerable<DownloadResult> currentDayRows = data.AsEnumerable().Where(r => r.DATE >= currentDate.Date && r.DATE < currentDate.Date.AddDays(1)).ToList();
            IEnumerable<DownloadResult> previousDayRows = data.AsEnumerable().Where(r => r.DATE >= previousDate.Date && r.DATE < previousDate.Date.AddDays(1)).ToList();
            IEnumerable<DownloadResult> twoDaysAgoRows;

            for (int dayIncrement = 2; dayIncrement <= numberOfDays; dayIncrement++)
            {
                Console.WriteLine($"Processing day {dayIncrement} of {numberOfDays}");
                currentDate = minDate.Date.AddDays(dayIncrement);

                twoDaysAgoRows = previousDayRows;
                previousDayRows = currentDayRows;
                currentDayRows = data.AsEnumerable().Where(r => r.DATE >= currentDate.Date && r.DATE < currentDate.Date.AddDays(1)).ToList();
                
                


                //string precipResult = GetValue(currentDayRows, "DAILYPrecip");
                string precipResult = currentDayRows.First(r => !string.IsNullOrWhiteSpace(r.DAILYPrecip)).DAILYPrecip;

                results.Add(new WeatherDataEntry()
                {
                    //PreviousMorningHumidity = double.Parse(GetMorningValue(previousDayRows, "HOURLYRelativeHumidity")),
                    PreviousMorningHumidity = double.Parse(previousDayRows.Where(r => r.DATE.Hour >= 8).OrderBy(r => r.DATE).Where(r => !string.IsNullOrWhiteSpace(r.HOURLYRelativeHumidity)).First().HOURLYRelativeHumidity),

                    //PreviousAvgTemp = double.Parse(GetValue(previousDayRows, "DAILYAverageDryBulbTemp")),
                    PreviousAvgTemp = double.Parse(previousDayRows.First(r => !string.IsNullOrWhiteSpace(r.DAILYAverageDryBulbTemp)).DAILYAverageDryBulbTemp),

                    //PreviousMinTemp = double.Parse(GetValue(previousDayRows, "DAILYMinimumDryBulbTemp")),
                    PreviousMinTemp = double.Parse(previousDayRows.First(r => !string.IsNullOrWhiteSpace(r.DAILYMinimumDryBulbTemp)).DAILYMinimumDryBulbTemp),

                    //PreviousMaxTemp = double.Parse(GetValue(previousDayRows, "DAILYMaximumDryBulbTemp")),
                    PreviousMaxTemp = double.Parse(previousDayRows.First(r => !string.IsNullOrWhiteSpace(r.DAILYMaximumDryBulbTemp)).DAILYMaximumDryBulbTemp),

                    //PreviousMorningPressure = double.Parse(GetMorningValue(previousDayRows, "HOURLYSeaLevelPressure")),
                    PreviousMorningPressure = double.Parse(previousDayRows.Where(r => r.DATE.Hour >= 8).OrderBy(r => r.DATE).Where(r => !string.IsNullOrWhiteSpace(r.HOURLYSeaLevelPressure)).First().HOURLYSeaLevelPressure),

                    //PreviousMorningPressureDifference = double.Parse(GetMorningValue(previousDayRows, "HOURLYSeaLevelPressure")) - double.Parse(GetMorningValue(twoDaysAgoRows, "HOURLYSeaLevelPressure")),
                    PreviousMorningPressureDifference = double.Parse(previousDayRows.Where(r => r.DATE.Hour >= 8).OrderBy(r => r.DATE).Where(r => !string.IsNullOrWhiteSpace(r.HOURLYSeaLevelPressure)).First().HOURLYSeaLevelPressure),

                    EntryDate = currentDate.Date,
                    
                    //MorningPressure = double.Parse(GetMorningValue(currentDayRows, "HOURLYSeaLevelPressure")),
                    MorningPressure = double.Parse(currentDayRows.Where(r => r.DATE.Hour >= 8).OrderBy(r => r.DATE).Where(r => !string.IsNullOrWhiteSpace(r.HOURLYSeaLevelPressure)).First().HOURLYSeaLevelPressure),

                    //MorningTemp = double.Parse(GetMorningValue(currentDayRows, "HOURLYDRYBULBTEMPF")),
                    MorningTemp = double.Parse(currentDayRows.Where(r => r.DATE.Hour >= 8).OrderBy(r => r.DATE).Where(r => !string.IsNullOrWhiteSpace(r.HOURLYDRYBULBTEMPF)).First().HOURLYDRYBULBTEMPF),

                    Rain = double.Parse(precipResult == "T" ? "0.01" : precipResult) > 0 ? 1.0 : 0.0
                });
            }

            return results;
        }

        static void NormalizeData(List<WeatherDataEntry> data)
        {
            var PreviousMinTempNorm = new NormalizedField(NormalizationAction.Normalize, "PreviousMinTemp", data.Max(r => r.PreviousMinTemp), data.Min(r => r.PreviousMinTemp), 1.0, -1.0);
            var PreviousMaxTempNorm = new NormalizedField(NormalizationAction.Normalize, "PreviousMaxTemp", data.Max(r => r.PreviousMaxTemp), data.Min(r => r.PreviousMaxTemp), 1.0, -1.0);
            var PreviousAvgTempNorm = new NormalizedField(NormalizationAction.Normalize, "PreviousAvgTemp", data.Max(r => r.PreviousAvgTemp), data.Min(r => r.PreviousAvgTemp), 1.0, -1.0);


            var PreviousMorningHumidityNorm = new NormalizedField(NormalizationAction.Normalize, "PreviousMorningHumidity", data.Max(r => r.PreviousMorningHumidity), data.Min(r => r.PreviousMorningHumidity), 1.0, -1.0);
            var PreviousMorningPressureNorm = new NormalizedField(NormalizationAction.Normalize, "PreviousMorningPressure", data.Max(r => r.PreviousMorningPressure), data.Min(r => r.PreviousMorningPressure), 1.0, -1.0);
            var PreviousMorningPressureDifferenceNorm = new NormalizedField(NormalizationAction.Normalize, "PreviousMorningPressureDifference", data.Max(r => r.PreviousMorningPressureDifference), data.Min(r => r.PreviousMorningPressureDifference), 1.0, -1.0);
            var MorningTempNorm = new NormalizedField(NormalizationAction.Normalize, "MorningTemp", data.Max(r => r.MorningTemp), data.Min(r => r.MorningTemp), 1.0, -1.0);
            var MorningPressureNorm = new NormalizedField(NormalizationAction.Normalize, "MorningPressure", data.Max(r => r.MorningPressure), data.Min(r => r.MorningPressure), 1.0, -1.0);

            foreach(WeatherDataEntry entry in data)
            {
                entry.NormPreviousMinTemp = PreviousMinTempNorm.Normalize(entry.PreviousMinTemp);
                entry.NormPreviousMaxTemp = PreviousMaxTempNorm.Normalize(entry.PreviousMaxTemp);
                entry.NormPreviousAvgTemp = PreviousAvgTempNorm.Normalize(entry.PreviousAvgTemp);
                entry.NormPreviousMorningHumidity = PreviousMorningHumidityNorm.Normalize(entry.PreviousMorningHumidity);
                entry.NormPreviousMorningPressure = PreviousMorningPressureNorm.Normalize(entry.PreviousMorningPressure);
                entry.NormPreviousMorningPressureDifference = PreviousMorningPressureDifferenceNorm.Normalize(entry.PreviousMorningPressureDifference);
                entry.NormMorningTemp = MorningTempNorm.Normalize(entry.MorningTemp);
                entry.NormMorningPressure = MorningPressureNorm.Normalize(entry.MorningPressure);
            }
        }

        static BasicNetwork CreateNetwork()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 8));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 40));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 40));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 40));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));

            network.Structure.FinalizeStructure();
            network.Reset();

            return network;
        }

        static INeuralDataSet CreateTrainingSet(List<WeatherDataEntry> processedData)
        {
            double[][] WEATHER_INPUT = new double[processedData.Count][];
            double[][] WEATHER_IDEAL = new double[processedData.Count][];

            for (int i = 0; i < processedData.Count; i++)
            {

                WEATHER_INPUT[i] = new double[8]
                {
                    processedData[i].NormPreviousMinTemp,
                    processedData[i].NormPreviousMaxTemp,
                    processedData[i].NormPreviousAvgTemp,
                    processedData[i].NormPreviousMorningHumidity,
                    processedData[i].NormPreviousMorningPressure,
                    processedData[i].NormPreviousMorningPressureDifference,
                    processedData[i].NormMorningTemp,
                    processedData[i].NormMorningPressure
                };

                WEATHER_IDEAL[i] = new double[1] { processedData[i].Rain };
            }

            INeuralDataSet trainingSet = new BasicNeuralDataSet(WEATHER_INPUT, WEATHER_IDEAL);

            return trainingSet;
        }

        static void TrainNetwork(BasicNetwork network, INeuralDataSet trainingSet)
        {

            ITrain train = new ResilientPropagation(network, trainingSet);

            int epoch = 1;
            do
            {
                train.Iteration();
                Console.WriteLine("Epoch #" + epoch + " Error: " + train.Error);
                epoch++;
            } while (epoch < 50000 && train.Error > 0.001);
        }

        //static string GetValue(IEnumerable<DownloadResult> rows, string ColumnName)
        //{
        //    var subRows = rows.Where(r => r.Field<string>(ColumnName) != "");

        //    string res = rows.Where(r => r.Field<string>(ColumnName) != "" && r.Field<string>(ColumnName) != null).First().Field<string>(ColumnName);
        //    return res;
        //    //return string.IsNullOrWhiteSpace(res) ? "0" : rows.Where(r => r.Field<string>(ColumnName) != null).First().Field<string>(ColumnName);
        //}

        //static string GetMorningValue(IEnumerable<DataRow> rows, string ColumnName)
        //{
        //    return GetValue(rows.Where(r => DateTime.Parse(r.Field<string>("DATE")).Hour >= 8).OrderBy(r => DateTime.Parse(r.Field<string>("DATE"))), ColumnName);
        //}
    }



    class WeatherDataEntry
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

    [DelimitedRecord(","), IgnoreFirst(1)]
    public class DownloadResult
    {
        [FieldValueDiscarded]
        public string STATION;

        [FieldValueDiscarded]
        public string STATION_NAME;

        [FieldValueDiscarded]
        public string ELEVATION;

        [FieldValueDiscarded]
        public string LATITUDE;

        [FieldValueDiscarded]
        public string LONGITUDE;

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm")]
        public DateTime DATE;

        [FieldValueDiscarded]
        public string REPORTTPYE;

        [FieldValueDiscarded]
        public string HOURLYSKYCONDITIONS;

        [FieldValueDiscarded]
        public string HOURLYVISIBILITY;

        [FieldValueDiscarded]
        public string HOURLYPRSENTWEATHERTYPE;

        public string HOURLYDRYBULBTEMPF;

        public string HOURLYDRYBULBTEMPC;

        public string HOURLYWETBULBTEMPF;

        public string HOURLYWETBULBTEMPC;

        public string HOURLYDewPointTempF;

        public string HOURLYDewPointTempC;

        public string HOURLYRelativeHumidity;

        [FieldValueDiscarded]
        public string HOURLYWindSpeed;

        [FieldValueDiscarded]
        public string HOURLYWindDirection;

        [FieldValueDiscarded]
        public string HOURLYWindGustSpeed;

        public string HOURLYStationPressure;
        
        public string HOURLYPressureTendency;
        
        public string HOURLYPressureChange;


        public string HOURLYSeaLevelPressure;

        public string HOURLYPrecip;

        [FieldValueDiscarded]
        public string HOURLYAltimeterSetting;

        public string DAILYMaximumDryBulbTemp;

        public string DAILYMinimumDryBulbTemp;


        public string DAILYAverageDryBulbTemp;


        public string DAILYDeptFromNormalAverageTemp;

        public string DAILYAverageRelativeHumidity;

        public string DAILYAverageDewPointTemp;

        public string DAILYAverageWetBulbTemp;

        [FieldValueDiscarded]
        public string DAILYHeatingDegreeDays;

        [FieldValueDiscarded]
        public string DAILYCoolingDegreeDays;

        [FieldValueDiscarded]
        public string DAILYSunrise;

        [FieldValueDiscarded]
        public string DAILYSunset;

        [FieldValueDiscarded]
        public string DAILYWeather;
        
        public string DAILYPrecip;

        [FieldValueDiscarded]
        public string DAILYSnowfall;

        [FieldValueDiscarded]
        public string DAILYSnowDepth;

        [FieldValueDiscarded]
        public string DAILYAverageStationPressure;
        
        public double? DAILYAverageSeaLevelPressure;

        [FieldValueDiscarded]
        public string DAILYAverageWindSpeed;

        [FieldValueDiscarded]
        public string DAILYPeakWindSpeed;

        [FieldValueDiscarded]
        public string PeakWindDirection;

        [FieldValueDiscarded]
        public string DAILYSustainedWindSpeed;

        [FieldValueDiscarded]
        public string DAILYSustainedWindDirection;

        [FieldValueDiscarded]
        public string MonthlyMaximumTemp;

        [FieldValueDiscarded]
        public string MonthlyMinimumTemp;

        [FieldValueDiscarded]
        public string MonthlyMeanTemp;

        [FieldValueDiscarded]
        public string MonthlyAverageRH;

        [FieldValueDiscarded]
        public string MonthlyDewpointTemp;

        [FieldValueDiscarded]
        public string MonthlyWetBulbTemp;

        [FieldValueDiscarded]
        public string MonthlyAvgHeatingDegreeDays;

        [FieldValueDiscarded]
        public string MonthlyAvgCoolingDegreeDays;

        [FieldValueDiscarded]
        public string MonthlyStationPressure;

        [FieldValueDiscarded]
        public string MonthlySeaLevelPressure;

        [FieldValueDiscarded]
        public string MonthlyAverageWindSpeed;

        [FieldValueDiscarded]
        public string MonthlyTotalSnowfall;

        [FieldValueDiscarded]
        public string MonthlyDeptFromNormalMaximumTemp;

        [FieldValueDiscarded]
        public string MonthlyDeptFromNormalMinimumTemp;

        [FieldValueDiscarded]
        public string MonthlyDeptFromNormalAverageTemp;

        [FieldValueDiscarded]
        public string MonthlyDeptFromNormalPrecip;

        [FieldValueDiscarded]
        public string MonthlyTotalLiquidPrecip;

        [FieldValueDiscarded]
        public string MonthlyGreatestPrecip;

        [FieldValueDiscarded]
        public string MonthlyGreatestPrecipDate;

        [FieldValueDiscarded]
        public string MonthlyGreatestSnowfall;

        [FieldValueDiscarded]
        public string MonthlyGreatestSnowfallDate;

        [FieldValueDiscarded]
        public string MonthlyGreatestSnowDepth;

        [FieldValueDiscarded]
        public string MonthlyGreatestSnowDepthDate;

        [FieldValueDiscarded]
        public string MonthlyDaysWithGT90Temp;

        [FieldValueDiscarded]
        public string MonthlyDaysWithLT32Temp;

        [FieldValueDiscarded]
        public string MonthlyDaysWithGT32Temp;

        [FieldValueDiscarded]
        public string MonthlyDaysWithLT0Temp;

        [FieldValueDiscarded]
        public string MonthlyDaysWithGT001Precip;

        [FieldValueDiscarded]
        public string MonthlyDaysWithGT010Precip;

        [FieldValueDiscarded]
        public string MonthlyDaysWithGT1Snow;

        [FieldValueDiscarded]
        public string MonthlyMaxSeaLevelPressureValue;

        [FieldValueDiscarded]
        public string MonthlyMaxSeaLevelPressureDate;

        [FieldValueDiscarded]
        public string MonthlyMaxSeaLevelPressureTime;

        [FieldValueDiscarded]
        public string MonthlyMinSeaLevelPressureValue;

        [FieldValueDiscarded]
        public string MonthlyMinSeaLevelPressureDate;

        [FieldValueDiscarded]
        public string MonthlyMinSeaLevelPressureTime;

        [FieldValueDiscarded]
        public string MonthlyTotalHeatingDegreeDays;

        [FieldValueDiscarded]
        public string MonthlyTotalCoolingDegreeDays;

        [FieldValueDiscarded]
        public string MonthlyDeptFromNormalHeatingDD;

        [FieldValueDiscarded]
        public string MonthlyDeptFromNormalCoolingDD;

        [FieldValueDiscarded]
        public string MonthlyTotalSeasonToDateHeatingDD;

        [FieldValueDiscarded]
        public string MonthlyTotalSeasonToDateCoolingDD;
    }
}
