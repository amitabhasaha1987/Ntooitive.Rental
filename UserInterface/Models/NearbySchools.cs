/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */

using System.Collections.Generic;
using System.Xml.Serialization;

namespace UserInterface.Models
{
    public class School
    {
        public int gsId { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string gradeRange { get; set; }
        public int enrollment { get; set; }
        public int gsRating { get; set; }
        public int parentRating { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public int districtId { get; set; }
        public string district { get; set; }
        public string districtNCESId { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string website { get; set; }
        public string ncesId { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public string overviewLink { get; set; }
        public string ratingsLink { get; set; }
        public string reviewsLink { get; set; }
        public double distance { get; set; }
        public string schoolStatsLink { get; set; }
    }

    public class Schools
    {
        public List<School> school { get; set; }
    }

    public class NearBySchool
    {
        public Schools schools { get; set; }
    }
}
