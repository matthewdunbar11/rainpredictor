using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

            RainPredictorNeuralNetwork.RainNeuralProcessor processor = new RainPredictorNeuralNetwork.RainNeuralProcessor();


            return true;
        }

        [Route("train")]
        [HttpGet]
        public string Train()
        {


            RainPredictorNeuralNetwork.RainNeuralProcessor processor = new RainPredictorNeuralNetwork.RainNeuralProcessor(@"network.nueral");

            processor.TrainFromFile(@"C:\Users\Matthew\Downloads\960971.csv");

            return "trained";
        }
    }
}