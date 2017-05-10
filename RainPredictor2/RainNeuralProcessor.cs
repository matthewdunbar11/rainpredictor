using Encog.Engine.Network.Activation;
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
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RainPredictorNeuralNetwork
{
    public class RainNeuralProcessor
    {
        BasicNetwork _network;

        NormalizedField NormPreviousMinTemp;
        NormalizedField NormPreviousMaxTemp;
        NormalizedField NormPreviousAvgTemp;
        NormalizedField NormPreviousMorningHumidity;
        NormalizedField NormPreviousMorningPressure;
        NormalizedField NormPreviousMorningPressureDifference;
        NormalizedField NormMorningTemp;
        NormalizedField NormMorningPressure;

        public RainNeuralProcessor()
        {

        }

        public RainNeuralProcessor(string fileLocation)
        {
            byte[] serialized = File.ReadAllBytes(fileLocation);


        }

        public void CreateNetwork(int[] layers)
        {
            _network = new BasicNetwork();
            //_network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 8));

            foreach(int layer in layers)
            {
                _network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, layer));
            }
            
            //_network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));

            _network.Structure.FinalizeStructure();
            _network.Reset();
        }

        public void SaveNetwork(string filePath)
        {
            var serializer = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                serializer.Serialize(ms, _network);
                ms.Position = 0;
                byte[] serialized = ms.ToArray();

                File.WriteAllBytes(filePath, serialized);
            }
        }


        public void TrainNetwork(INeuralDataSet trainingDataSet, int MaxIterations, double MaxError)
        {
            ITrain train = new ResilientPropagation(_network, trainingDataSet);

            int epoch = 1;
            do
            {
                train.Iteration();
                //Console.WriteLine("Epoch #" + epoch + " Error: " + train.Error);
                epoch++;
            } while (epoch < MaxIterations && train.Error > MaxError);
        }

        public Tuple<int, int> CheckTrainedModel(IEnumerable<WeatherDataEntry> checkData)
        {
            int totalGuesses = 0;
            int totalRight = 0;
            foreach (WeatherDataEntry entry in checkData)
            {
                double[] output = new double[1];
                _network.Compute(new double[]
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
                
                //Console.WriteLine($"{entry.EntryDate} {entry.Rain}, Predicted: {output[0].ToString()}");

                if ((output[0] > 0.5 ? 1 : 0) == (int)entry.Rain)
                {
                    totalRight++;
                }
                totalGuesses++;
            }

            return new Tuple<int, int>(totalRight, totalGuesses);
        }

        public bool WillItRain(WeatherDataEntry entry)
        {
            double[] output = new double[1];
            _network.Compute(new double[]
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

            return output[0] > 0.5;
        }

        public void TrainFromFile(string FileLocation, int MaxIterations, double MaxError)
        {
            List<WeatherDataEntry> processedData;
            if (System.IO.File.Exists("./data.json"))
            {
                processedData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WeatherDataEntry>>(System.IO.File.ReadAllText("./data.json"));
            }
            else
            {

                Console.WriteLine("Importing raw data");
                DownloadResult[] data = GetWeatherData(FileLocation);

                Console.WriteLine("Processing raw data");
                processedData = ProcessData(data);

                Console.WriteLine("Normalizing data");
                NormalizeData(processedData);

                //System.IO.File.WriteAllText("./data.json", Newtonsoft.Json.JsonConvert.SerializeObject(processedData));
            }


            Console.WriteLine("Creating training set");
            INeuralDataSet trainingSet = CreateTrainingSet(processedData.Where(r => r.EntryDate < new DateTime(2017, 1, 1)).ToList());

            this.TrainNetwork(trainingSet, MaxIterations, MaxError);
        }




        private DownloadResult[] GetWeatherData(string FileLocation)
        {
            var engine = new FileHelperEngine<DownloadResult>();

            var records = engine.ReadFile(FileLocation);

            //DataTable data = FileHelpers.CsvEngine.CsvToDataTable(@"C:\Users\Matthew\Downloads\960971.csv", ',');

            return records;
        }

        private List<WeatherDataEntry> ProcessData(DownloadResult[] data)
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

        private void NormalizeData(List<WeatherDataEntry> data)
        {
            if (NormPreviousMinTemp == null)
            {
                NormPreviousMinTemp = new NormalizedField(NormalizationAction.Normalize, "PreviousMinTemp", data.Max(r => r.PreviousMinTemp), data.Min(r => r.PreviousMinTemp), 1.0, -1.0);
                NormPreviousMaxTemp = new NormalizedField(NormalizationAction.Normalize, "PreviousMaxTemp", data.Max(r => r.PreviousMaxTemp), data.Min(r => r.PreviousMaxTemp), 1.0, -1.0);
                NormPreviousAvgTemp = new NormalizedField(NormalizationAction.Normalize, "PreviousAvgTemp", data.Max(r => r.PreviousAvgTemp), data.Min(r => r.PreviousAvgTemp), 1.0, -1.0);


                NormPreviousMorningHumidity = new NormalizedField(NormalizationAction.Normalize, "PreviousMorningHumidity", data.Max(r => r.PreviousMorningHumidity), data.Min(r => r.PreviousMorningHumidity), 1.0, -1.0);
                NormPreviousMorningPressure = new NormalizedField(NormalizationAction.Normalize, "PreviousMorningPressure", data.Max(r => r.PreviousMorningPressure), data.Min(r => r.PreviousMorningPressure), 1.0, -1.0);
                NormPreviousMorningPressureDifference = new NormalizedField(NormalizationAction.Normalize, "PreviousMorningPressureDifference", data.Max(r => r.PreviousMorningPressureDifference), data.Min(r => r.PreviousMorningPressureDifference), 1.0, -1.0);
                NormMorningTemp = new NormalizedField(NormalizationAction.Normalize, "MorningTemp", data.Max(r => r.MorningTemp), data.Min(r => r.MorningTemp), 1.0, -1.0);
                NormMorningPressure = new NormalizedField(NormalizationAction.Normalize, "MorningPressure", data.Max(r => r.MorningPressure), data.Min(r => r.MorningPressure), 1.0, -1.0);
            }

            foreach (WeatherDataEntry entry in data)
            {
                entry.NormPreviousMinTemp = NormPreviousMinTemp.Normalize(entry.PreviousMinTemp);
                entry.NormPreviousMaxTemp = NormPreviousMaxTemp.Normalize(entry.PreviousMaxTemp);
                entry.NormPreviousAvgTemp = NormPreviousAvgTemp.Normalize(entry.PreviousAvgTemp);
                entry.NormPreviousMorningHumidity = NormPreviousMorningHumidity.Normalize(entry.PreviousMorningHumidity);
                entry.NormPreviousMorningPressure = NormPreviousMorningPressure.Normalize(entry.PreviousMorningPressure);
                entry.NormPreviousMorningPressureDifference = NormPreviousMorningPressureDifference.Normalize(entry.PreviousMorningPressureDifference);
                entry.NormMorningTemp = NormMorningTemp.Normalize(entry.MorningTemp);
                entry.NormMorningPressure = NormMorningPressure.Normalize(entry.MorningPressure);
            }
        }


        private INeuralDataSet CreateTrainingSet(List<WeatherDataEntry> processedData)
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



    }
}
