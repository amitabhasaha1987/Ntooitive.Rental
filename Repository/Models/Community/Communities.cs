using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Repositories.Models.Base;

namespace Repositories.Models.Community
{
    public class Communities : BaseEntity
    {
        public string CommunityId { get; set; }
        public string CommunityName { get; set; }
        public string Number { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string WebSite { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string LogoImage { get; set; }
        public string Description { get; set; }
        public string[] Zips { get; set; }
        public string Zip1 { get; set; }
    }
}
