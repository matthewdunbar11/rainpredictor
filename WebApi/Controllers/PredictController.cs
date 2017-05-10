using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class PredictController : ApiController
    {
        [Route("predict")]
        [HttpGet]
        public bool Predict([FromUri] double morningPressure, 
                            [FromUri] double morningTemp, 
                            [FromUri] double previousAvgTemp, 
                            [FromUri] double previousMaxTemp, 
                            [FromUri] double previousMorningPressureDifference, 
                            [FromUri] double previousMonringPressure,
                            [FromUri] double previousMorningHumidity,
                            [FromUri] double previousMinTemp)
        {
            RainPredictorNeuralNetwork.WeatherDataEntry entry = new RainPredictorNeuralNetwork.WeatherDataEntry()
            {
                MorningPressure = morningPressure,
                MorningTemp = morningTemp,
                PreviousAvgTemp = previousAvgTemp,
                PreviousMaxTemp = previousMaxTemp,
                PreviousMorningPressureDifference = previousMorningPressureDifference,
                PreviousMorningPressure = previousMonringPressure,
                PreviousMorningHumidity = previousMorningHumidity,
                PreviousMinTemp = previousMinTemp
            };

            RainPredictorNeuralNetwork.RainNeuralProcessor processor = new RainPredictorNeuralNetwork.RainNeuralProcessor(HttpContext.Current.Server.MapPath("network.neu"));

            return processor.WillItRain(entry);
        }

        [Route("train")]
        [HttpGet]
        public string Train()
        {


            //RainPredictorNeuralNetwork.RainNeuralProcessor processor = new RainPredictorNeuralNetwork.RainNeuralProcessor(@"network.nueral");
            RainPredictorNeuralNetwork.RainNeuralProcessor processor = new RainPredictorNeuralNetwork.RainNeuralProcessor();

            processor.CreateNetwork(new int[] { 8, 13, 1 });
            processor.TrainFromFile(@"C:\Users\Matthew\Downloads\960971.csv", 5000, 0.10);
            processor.SaveNetwork(HttpContext.Current.Server.MapPath("network.neu"));
            return "trained";
        }
    }
}