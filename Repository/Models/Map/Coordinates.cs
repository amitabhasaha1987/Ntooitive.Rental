using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Models.Base;

namespace Repositories.Models.Map
{
    public class Coordinates : BaseEntity
    {
        public string Address { get; set; }
        public double[] Coordinate { get; set; }
       // public LatitudeLongitude LatLong { get; set; }
        public List<NearByLocation> ATM { get; set; }
        public List<NearByLocation> TrainStation { get; set; }
        public List<NearByLocation> BusStation { get; set; }
        public List<NearByLocation> Airport { get; set; }
        public List<NearByLocation> GroceryStore { get; set; }
        public List<NearByLocation> PetrolStation { get; set; }
        public List<NearByLocation> Restaurant { get; set; }
        public List<NearByLocation> Bank { get; set; }
        public List<NearByLocation> Hotel { get; set; }
        public List<NearByLocation> Hospital { get; set; }
        public List<NearByLocation> School { get; set; }
        public List<NearByLocation> PoliceStation { get; set; }
        public List<NearByLocation> PostOffice { get; set; }
        public List<NearByLocation> DepartmentStore { get; set; }
        public List<NearByLocation> CoffeeShop { get; set; }
    }
}
