using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Repositories.Models.NewHome;
using Repositories.Models.ListHub;
using Newtonsoft.Json;
using Nest;
using Repositories.Models.ViewModel;

namespace Repositories.Models.Common
{
    [ElasticType(IdProperty = "MlsNumber")]
    public class PropertyListing
    {
        public string[] Attributes { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string objId { get; set; }
        public string MlsNumber { get; set; }
        public string FullStreetAddress { get; set; }
        public string FullAddress { get; set; }
        public double Price { get; set; }
        //NoOfBedRooms considered as NoOfBedRooms_high
        public int NoOfBedRooms { get; set; }
        public int NoOfBedRooms_low { get; set; }
        //NoOfBathRooms considered as NoOfBathRooms_hign
        public int NoOfBathRooms { get; set; }
        public int NoOfBathRooms_low { get; set; }
        public int FullBathrooms { get; set; }
        //NoOfHalfBathRooms considered as NoOfHalfBathRooms_hign
        public int NoOfHalfBathRooms { get; set; }
        public int NoOfHalfBathRooms_low { get; set; }
        public int ThreeQuarterBathrooms { get; set; }
        public int OneQuarterBathrooms { get; set; }
        public int PartialBathrooms { get; set; }
        public LotSize LotSize { get; set; }
        public string ProviderName { get; set; }
        public MarketingInformation MarketingInformation { get; set; }
        public string PropertyType { get; set; }
        public string BrokerageName { get; set; }
        public string BrokerageLogoUrl { get; set; }
        public string ListingParticipantsName { get; set; }
        public List<Plan> Plans { get; set; }
        public GeoLocation GeoLocation { get; set; }
        public string[] DefaultPhoto { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string[] CommunityName { get; set; }

        public double LivingArea { get; set; }
        public bool IsNewConstruction { get; set; }

        public bool IsFeatured { get; set; }
        public bool IsSpotlight { get; set; }

        public DateTime ListingDate { get; set; }
        public PropertySubType PropertySubType { get; set; }

        public string YearBuilt { get; set; }
        public List<DateTimeRange> DateTimeRanges { get; set; }
        public string VirtualTour { get; set; }
        public DateTime EntryDate { get; set; }
        public int ImageCount { get; set; }
        //public string OfficeImage { get; set; }
        public DateTime? ClassifiedExpireDate { get; set; }
        public string AgentName { get; set; }
        public bool IsClassified { get; set; }

        public bool IsAgentFeauterd { get; set; }

        public double Communityprice_low { get; set; }
        public double Communityprice_high { get; set; }
        public double Communitysqft_high { get; set; }
        public double Communitysqft_low { get; set; }



    }



    public class GeoLocation
    {
        [BsonElement("type")]
        public string Type { get; set; }//Point
        [BsonElement("coordinates")]
        public List<double> Coordinates { get; set; }
    }

    public class LatLong
    {
        public double Latitude { get; set; }//Point

        public double Longitude { get; set; }
    }
}
