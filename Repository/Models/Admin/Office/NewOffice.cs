using Repositories.Models.ListHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Models.Admin.Office
{
   public class NewOffice : Repositories.Models.ListHub.Office
    {
        //public string OfficeKey { get; set; }
        //public string OfficeId { get; set; }
        //public OfficeCode OfficeCode { get; set; }
        //public string Name { get; set; }
        //public string CorporateName { get; set; }
        //public string BrokerId { get; set; }
        //public string MainOfficeId { get; set; }
        //public string PhoneNumber { get; set; }
        //public Address Address { get; set; }
        //public string OfficeEmail { get; set; }
        //public string Website { get; set; }
        public List<State> StateList { get; set; }
        public List<Cities> CityList { get; set; }
        public List<StreetAddress> StreetList { get; set; }
        public List<ZipCode> ZipCodeList { get; set; }
    }
}
