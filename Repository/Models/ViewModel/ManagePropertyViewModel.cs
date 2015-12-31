using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using Repositories.Models.ListHub;

namespace Repositories.Models.ViewModel
{
    public class ManagePropertyViewModel
    {
        [BsonIgnore]
        public string UniqueId { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsSpotlight { get; set; }
        public string AgentDescription { get; set; }
        public bool IsDeleted { get; set; }
        public List<DateTimeRange> DateTimeRanges { get; set; }

        [BsonIgnore]
        public string Type { get; set; }

        public List<string> PhotosUrl { get; set; }
    }

    public class DateTimeRange
    {
        [BsonIgnore]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        private DateTime StartDateTime { get; set; }

        [BsonIgnore]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        private DateTime EndDateTime { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]

        public DateTime OpenHouseStartDateTime
        {
            get
            {
                return !string.IsNullOrEmpty(OpenHouseStartDateTimestr)
                    ? DateTime.ParseExact(OpenHouseStartDateTimestr, "MM/dd/yyyy h:mm tt", new CultureInfo("en-US"),
                        DateTimeStyles.None)
                    : StartDateTime;
            }
            set { StartDateTime = value; }
        }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime OpenHouseEndDateTime
        {
            get
            {

                return !string.IsNullOrEmpty(OpenHouseEndDateTimestr)
                 ? DateTime.ParseExact(OpenHouseEndDateTimestr, "MM/dd/yyyy h:mm tt", new CultureInfo("en-US"),
                     DateTimeStyles.None)
                 : EndDateTime;
            }
            set { EndDateTime = value; }
        }
        [BsonIgnore]
        public string OpenHouseStartDateTimestr { get; set; }
        [BsonIgnore]
        public string OpenHouseEndDateTimestr { get; set; }
    }
}
