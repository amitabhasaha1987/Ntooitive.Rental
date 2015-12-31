using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Models.ViewModel
{
    public class ManageNewHomePropertyViewModel
    {
        [BsonIgnore]
        public string UniqueId { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsSpotlight { get; set; }
        public string AgentDescription { get; set; }
        [BsonIgnore]
        public string Type { get; set; }
    }
}
