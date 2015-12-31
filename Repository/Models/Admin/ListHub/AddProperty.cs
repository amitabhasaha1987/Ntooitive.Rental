using Repositories.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Models.Admin.ListHub
{
    public class AddProperty
    {
        public string ProviderName { get; set; }
        public string BuilderName { get; set; }

        public string CommunityName { get; set; }

        public string PropertyType { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string WebsiteAddress { get; set; }

        public decimal Price { get; set; }

        public int BedRoom { get; set; }

        public int FullBathrooms { get; set; }
        public int HalfBathrooms { get; set; }
        public int ThreeQuarterBathrooms { get; set; }
        public int OneQuarterBathrooms { get; set; }
        public int PartialBathrooms { get; set; }
        public decimal SQFT { get; set; }

        public int LotSize { get; set; }

        public string PropertyDetails { get; set; }

        public string SocialMediapostingoptions { get; set; }

        public string EmailFriend { get; set; }

        public string VirtualTours { get; set; }

        public string VideoTours { get; set; }

        public string CreatedBy { get; set; }
        public string UniqueNo { get; set; }

        public string City { get; set; }
        public string StateOrProvince { get; set; }
        public string PostalCode { get; set; }
        public ManagePropertyViewModel ManagePropertyViewModel { get; set; }
    }
}
