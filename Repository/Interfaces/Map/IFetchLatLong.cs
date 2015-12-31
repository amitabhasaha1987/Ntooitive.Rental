using Repositories.Models.Map;

namespace Repositories.Interfaces.Map
{
   public interface IFetchLatLong
    {
        Coordinates GetLatitudeAndLongitude(string address);

        bool InsertLatlong(string address, Coordinates coordinates);//double[] coordinates);

       double[] GetLatlong(string address);
    }
}
