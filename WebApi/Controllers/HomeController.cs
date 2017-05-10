using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Predict()
        {
            return View(new WeatherEntryViewModel());
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Predict([FromBody] WeatherEntryViewModel model)
        {
            
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "morningPressure", model.MorningPressure.ToString() },
                { "morningTemp", model.MorningTemp.ToString() },
                { "previousAvgTemp", model.PreviousAvgTemp.ToString() },
                { "previousMaxTemp", model.PreviousMaxTemp.ToString() },
                { "previousMinTemp", model.PreviousMinTemp.ToString() },
                { "previousMorningHumidity", model.PreviousMorningHumidity.ToString() },
                { "previousMorningPressure", model.PreviousMorningPressure.ToString() },
                { "PreviousMorningPressureDifference", model.PreviousMorningPressureDifference.ToString() }
            };

            return Redirect($"/predict?{string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
        }
    }
}
