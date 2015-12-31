using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Repositories.Models.NewHomeBDXFeed
{
    [XmlRoot(ElementName = "Image")]
    public class Image
    {
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "Seq")]
        public string Seq { get; set; }
        [XmlAttribute(AttributeName = "ProcessDate")]
        public string ProcessDate { get; set; }
        [XmlText]
        public string Text { get; set; }
    }


    [XmlRoot(ElementName = "PlanAmenity")]
    public class PlanAmenity
    {
        [XmlElement(ElementName = "Type")]
        public string Type { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Listing")]
    public class Listing
    {
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "Image")]
        public List<Image> Image { get; set; }
        [XmlAttribute(AttributeName = "ListingID")]
        public string ListingID { get; set; }
        [XmlAttribute(AttributeName = "ListingType")]
        public string ListingType { get; set; }
        [XmlAttribute(AttributeName = "PlanName")]
        public string PlanName { get; set; }
        [XmlAttribute(AttributeName = "BasePrice")]
        public string BasePrice { get; set; }
        [XmlAttribute(AttributeName = "Bedrooms")]
        public string Bedrooms { get; set; }
        [XmlAttribute(AttributeName = "Baths")]
        public string Baths { get; set; }
        [XmlAttribute(AttributeName = "BaseSqft")]
        public string BaseSqft { get; set; }
        [XmlAttribute(AttributeName = "Garage")]
        public string Garage { get; set; }
        [XmlAttribute(AttributeName = "Stories")]
        public string Stories { get; set; }
        [XmlAttribute(AttributeName = "MasterBedLocation")]
        public string MasterBedLocation { get; set; }
        [XmlAttribute(AttributeName = "NHSListingURL")]
        public string NHSListingURL { get; set; }
        [XmlAttribute(AttributeName = "PlanType")]
        public string PlanType { get; set; }
        [XmlAttribute(AttributeName = "ListingAddress")]
        public string ListingAddress { get; set; }
        [XmlAttribute(AttributeName = "ListingCity")]
        public string ListingCity { get; set; }
        [XmlAttribute(AttributeName = "ListingState")]
        public string ListingState { get; set; }
        [XmlAttribute(AttributeName = "ListingZIP")]
        public string ListingZIP { get; set; }
        [XmlAttribute(AttributeName = "ListingMoveInDate")]
        public string ListingMoveInDate { get; set; }

        [XmlAttribute(AttributeName = "VirtualTour")]
        public string VirtualTour { get; set; }
        [XmlAttribute(AttributeName = "DiningAreas")]
        public string DiningAreas { get; set; }

        [XmlAttribute(AttributeName = "ListingURL")]
        public string ListingURL { get; set; }

        [XmlAttribute(AttributeName = "LivingAreas")]
        public string LivingAreas { get; set; }

        [XmlAttribute(AttributeName = "Basement")]
        public string Basement { get; set; }

        [XmlAttribute(AttributeName = "EnvisionDesignCenter")]
        public string EnvisionDesignCenter { get; set; }

        [XmlElement(ElementName = "PlanAmenity")]
        public List<PlanAmenity> PlanAmenity { get; set; }
    }

    [XmlRoot(ElementName = "Subdivision")]
    public class Subdivision
    {
        [XmlElement(ElementName = "DrivingDirections")]
        public string DrivingDirections { get; set; }
        [XmlElement(ElementName = "SubDescription")]
        public string SubDescription { get; set; }
        [XmlElement(ElementName = "Image")]
        public List<Image> Image { get; set; }
        [XmlElement(ElementName = "Listing")]
        public List<Listing> Listing { get; set; }
        [XmlAttribute(AttributeName = "BuilderID")]
        public string BuilderID { get; set; }
        [XmlAttribute(AttributeName = "BuilderNumber")]
        public string BuilderNumber { get; set; }
        [XmlAttribute(AttributeName = "BrandName")]
        public string BrandName { get; set; }
        [XmlAttribute(AttributeName = "BrandLogo_Med")]
        public string BrandLogo_Med { get; set; }
        [XmlAttribute(AttributeName = "BrandLogo_Sm")]
        public string BrandLogo_Sm { get; set; }
        [XmlAttribute(AttributeName = "BuilderWebSite")]
        public string BuilderWebSite { get; set; }
        [XmlAttribute(AttributeName = "SubdivisionID")]
        public string SubdivisionID { get; set; }
        [XmlAttribute(AttributeName = "SubdivisionNumber")]
        public string SubdivisionNumber { get; set; }
        [XmlAttribute(AttributeName = "SubdivisionName")]
        public string SubdivisionName { get; set; }
        [XmlAttribute(AttributeName = "Latitude")]
        public string Latitude { get; set; }
        [XmlAttribute(AttributeName = "Longitude")]
        public string Longitude { get; set; }
        [XmlAttribute(AttributeName = "BuildOnYourLot")]
        public string BuildOnYourLot { get; set; }
        [XmlAttribute(AttributeName = "MultiFamily")]
        public string MultiFamily { get; set; }
        [XmlAttribute(AttributeName = "GolfCourse")]
        public string GolfCourse { get; set; }
        [XmlAttribute(AttributeName = "Pool")]
        public string Pool { get; set; }
        [XmlAttribute(AttributeName = "Greenbelt")]
        public string Greenbelt { get; set; }
        [XmlAttribute(AttributeName = "Views")]
        public string Views { get; set; }
        [XmlAttribute(AttributeName = "Park")]
        public string Park { get; set; }
        [XmlAttribute(AttributeName = "SportFacilities")]
        public string SportFacilities { get; set; }
        [XmlAttribute(AttributeName = "Address")]
        public string Address { get; set; }
        [XmlAttribute(AttributeName = "City")]
        public string City { get; set; }
        [XmlAttribute(AttributeName = "State")]
        public string State { get; set; }
        [XmlAttribute(AttributeName = "Zip")]
        public string Zip { get; set; }
        [XmlAttribute(AttributeName = "Phone")]
        public string Phone { get; set; }
        [XmlAttribute(AttributeName = "Email")]
        public string Email { get; set; }
        [XmlAttribute(AttributeName = "MarketID")]
        public string MarketID { get; set; }
        [XmlAttribute(AttributeName = "MarketName")]
        public string MarketName { get; set; }
        [XmlAttribute(AttributeName = "Status")]
        public string Status { get; set; }
        [XmlAttribute(AttributeName = "SubWebsite")]
        public string SubWebsite { get; set; }
        [XmlAttribute(AttributeName = "NHSSubWebsite")]
        public string NHSSubWebsite { get; set; }
        [XmlAttribute(AttributeName = "PriceFrom")]
        public string PriceFrom { get; set; }
        [XmlAttribute(AttributeName = "PriceTo")]
        public string PriceTo { get; set; }

        [XmlAttribute(AttributeName = "Agent")]
        public string Agent { get; set; }
    }

    [XmlRoot(ElementName = "Distribution")]
    public class NewHomeBDXListingRoot
    {
        [XmlElement(ElementName = "Subdivision")]
        public List<Subdivision> Subdivision { get; set; }
        [XmlAttribute(AttributeName = "DateGenerated")]
        public string DateGenerated { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
    }

}
