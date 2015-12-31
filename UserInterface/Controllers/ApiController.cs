using System.Web.Mvc;
using RestSharp;
using UserInterface.Models;

namespace UserInterface.Controllers
{
    public class ApiController : Controller
    {
        // GET: Api
        public ActionResult GetNearbySchools(string state, string zip, string address, string city, string schoolType, string levelCode)
        {
            var client = new RestClient("http://api.greatschools.org/");
            var apiKey = "eliwqg0t8cnrqwvsyh0tcaxx";
            var radius = 5;
            var limit = 3;
            string url;
            if (levelCode != "private")
            {
                url = "/schools/nearby?key=" + apiKey + "&address=" + address +
                      "&city=" + city + "&state=" + state + "&zip=" + zip + "&schoolType=" + schoolType + "&levelCode=" +
                      levelCode
                      + "&radius=" + radius + "&limit=" + limit;
            }
            else
            {
                url = "/schools/nearby?key=" + apiKey + "&address=" + address +
                      "&city=" + city + "&state=" + state + "&zip=" + zip + "&schoolType=" + schoolType 
                      + "&radius=" + radius + "&limit=" + limit;
            }
            var request = new RestRequest(url, Method.GET)
            {
                RequestFormat = DataFormat.Json
            };
            var response = client.Execute<NearBySchool>(request);
            if (response.Data != null && response.Data.schools != null && response.Data.schools.school != null)
            {
                return Json(response.Data.schools.school, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);

            }


        }
    }
}