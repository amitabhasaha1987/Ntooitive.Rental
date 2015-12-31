using System.Configuration;
using System.Linq;
using Repositories.Interfaces.Map;
using Repositories.Models.Map;
using RestSharp;

namespace Core.Implementation.Map
{
    public class GetLatLongFromGoogle //: IFetchLatLong
    {
        public LatitudeLongitude GetLatitudeAndLongitude(string address)
        {
            var apiKey = ConfigurationManager.AppSettings["GoogleMap:APIKey"];
            var client = new RestClient("https://maps.google.com/");

            var request = new RestRequest("maps/api/geocode/json?address=" + address + "&key=" + apiKey, Method.GET)
            {
                RequestFormat = DataFormat.Json
            };
            var response = client.Execute<AddressDetails>(request);

            if (response.Data.status == "OK")
            {
                var location = response.Data.results.FirstOrDefault();
                if (location != null)
                    return location.geometry.location;
                else
                {
                    return null;
                }
            }
            else if (response.Data.status == "OVER_QUERY_LIMIT")
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        public  bool InsertLatlong(string address, double[] coordinates)
        {
            throw new System.NotImplementedException();
        }

        public  double[] GetLatlong(string address)
        {
            throw new System.NotImplementedException();
        }
    }
}
