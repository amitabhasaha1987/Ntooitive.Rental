using System.Collections.Generic;
using Repositories.Models.ListHub;
using Repositories.Models.NewHome;
using System;

namespace Repositories.Models.Common
{
    public class PropertyDetailsDistrict
    {
    }

    public class PropertyDetailsSchool
    {
        public string Name { get; set; }
        public string SchoolCategory { get; set; }
        public PropertyDetailsDistrict District { get; set; }
    }

    public class PropertyDetails
    {
        public double ListPrice { get; set; }
        public GeoPoint GeoPoint { get; set; }
        public string ListingURL { get; set; }
        public string ProviderName { get; set; }
        public string ProviderURL { get; set; }
        public string ProviderCategory { get; set; }
        public string LeadRoutingEmail { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public string PropertyType { get; set; }
        public string PropertySubType { get; set; }
        public string ListingKey { get; set; }
        public string ListingCategory { get; set; }
        public string ListingStatus { get; set; }
        public List<string> Photos { get; set; }
        public string ListingDescription { get; set; }
        public string MlsNumber { get; set; }
        public double LotSize { get; set; }
        public string ListingDate { get; set; }
        public string ListingTitle { get; set; }
        public int FullBathrooms { get; set; }
        public int ThreeQuarterBathrooms { get; set; }
        public int HalfBathrooms { get; set; }
        public int OneQuarterBathrooms { get; set; }
        public int PartialBathrooms { get; set; }
        public string Offices { get; set; }
        public string Location { get; set; }
        public string FullStreetAddress { get; set; }
        public string City { get; set; }
        public string StateOrProvince { get; set; }
        public string PostalCode { get; set; }
        public List<PropertyDetailsSchool> Schools { get; set; }
        public string OfficeId { get; set; }
        public string OfficeCodeId { get; set; }
        public string OfficeName { get; set; }
        public string CorporateName { get; set; }
        public string BrokerId { get; set; }
        public string MainOfficeId { get; set; }
        public string OfficePhoneNumber { get; set; }
        public string OfficeFullStreetAddress { get; set; }
        public string OfficeCity { get; set; }
        public string OfficeState { get; set; }
        public string OfficePostalCode { get; set; }
        public string BrokerageName { get; set; }
        public string BrokeragePhone { get; set; }
        public string BrokerageEmail { get; set; }
        public string BrokerageWebsite { get; set; }
        public string BrokerageLogoURL { get; set; }
        public string BrokerageCity { get; set; }
        public string BrokerageState { get; set; }
        public string BrokeragePostalCode { get; set; }

        public string[] Appliance { get; set; }
        public string ArchitectureStyle { get; set; }
        public string CoolingSystem { get; set; }
        public string[] ExteriorType { get; set; }
        public string HeatingSystem { get; set; }
        public bool IsNewConstruction { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsSpotLight { get; set; }
        public double NoOfFloor { get; set; }
        public string NoOfParkingSpace { get; set; }
        public string[] ParkingType { get; set; }
        public string RoofTypes { get; set; }
        public string[] Rooms { get; set; }
        public double LivingArea { get; set; }
        public List<Plan> Plans { get; set; }
        public string ElevationImages { get; set; }
        public string VirtualTour { get; set; }
        public string PlanViewer { get; set; }

        //Added for  ClassifiedFeed
        public DateTime? ClassifiedExpireDate { get; set; }
        public string AgentName { get; set; }
        public string AgentEmail { get; set; }
        public string LogoImage { get; set; }
        public string WebsiteUrl { get; set; }
        //End classified

        public List<Repositories.Models.ViewModel.DateTimeRange> DateTimeRanges { get; set; }

    }
}
