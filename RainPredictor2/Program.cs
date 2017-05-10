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

namespace RainPredictorNeuralNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            List<WeatherDataEntry> processedData;
            if (System.IO.File.Exists("./data.json"))
            {
                processedData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WeatherDataEntry>>(System.IO.File.ReadAllText("./data.json"));
            }
            else
            {



                /*Console.WriteLine("Importing raw data");
                DownloadResult[] data = GetWeatherData();

                Console.WriteLine("Processing raw data");
                processedData = ProcessData(data);

                Console.WriteLine("Normalizing data");
                NormalizeData(processedData);
                */
                //System.IO.File.WriteAllText("./data.json", Newtonsoft.Json.JsonConvert.SerializeObject(processedData));
            }


            /*Console.WriteLine("Creating training set");            
            INeuralDataSet trainingSet = CreateTrainingSet(processedData.Where(r => r.EntryDate < new DateTime(2017, 1, 1)).ToList());

            RainNeuralProcessor processor = new RainNeuralProcessor();

            Console.WriteLine("Creating neural network");
            List<Tuple<int, int, double>> Results = new List<Tuple<int, int, double>>();

            for(int numberOfLayers = 0; numberOfLayers <= 3; numberOfLayers++)
            {
                for(int numberOfNeurons = 1; numberOfNeurons <= 20; numberOfNeurons++)
                {
                    List<int> layers = new List<int>();
                    for(int index = 0; index < numberOfLayers; index++)
                    {
                        layers.Add(numberOfNeurons);
                    }

                    processor.CreateNetwork(layers.ToArray());
                    processor.TrainNetwork(trainingSet, 5000, 0.18);
                    Tuple<int, int> accuracy = processor.CheckTrainedModel(processedData.Where(r => r.EntryDate >= new DateTime(2017, 1, 1)));
                    Results.Add(new Tuple<int, int, double>(numberOfLayers, numberOfNeurons, (double)accuracy.Item1 / accuracy.Item2));

                    Console.WriteLine($"Finished {numberOfLayers}, {numberOfNeurons}: {(double)accuracy.Item1 / accuracy.Item2}");
                }
            }

            foreach(Tuple<int, int, double> result in Results.OrderBy(t => t.Item3))
            {
                Console.WriteLine($"{result.Item1}, {result.Item2}: {result.Item3}");
            }

            
            Console.WriteLine("Press Any Key to Continue");
            Console.ReadKey();*/
        }



    }





}
