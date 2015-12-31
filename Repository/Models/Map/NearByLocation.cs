using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Models.Map
{   
    public class Metadata
    {
        public string uri { get; set; }
    }

    public class NearByLocation
    {
        public Metadata __metadata { get; set; }
        public string EntityID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string AddressLine { get; set; }
        public string Locality { get; set; }
        public string AdminDistrict2 { get; set; }
        public string AdminDistrict { get; set; }
        public string PostalCode { get; set; }
        public string CountryRegion { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Phone { get; set; }
        public string EntityTypeID { get; set; }

        public string __Distance { get; set; }
    }

    public class D
    {
        public string __copyright { get; set; }

         public List<NearByLocation> results { get; set; }
    }

    public class SpatialDataCollection
    {
        public D d { get; set; }
    }


}
