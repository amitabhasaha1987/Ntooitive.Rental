using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Xml.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Repositories.Models.Base;
using Repositories.Models.ListHub;
using Repositories.Models.ViewModel;

namespace Repositories.Models.NewHome
{
    [XmlRoot(ElementName = "BrandLogo_Med")]
    public class BrandLogoMed
    {
        [XmlAttribute(AttributeName = "ReferenceType")]
        public string ReferenceType { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "DefaultLeadsEmail")]
    public class DefaultLeadsEmail
    {
        [XmlAttribute(AttributeName = "LeadsPerMessage")]
        public string LeadsPerMessage { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Geocode")]
    public class Geocode
    {
        [XmlElement(ElementName = "Latitude")]
        public string Latitude { get; set; }
        [XmlElement(ElementName = "Longitude")]
        public string Longitude { get; set; }

        
    }

    [XmlRoot(ElementName = "Address")]
    public class Address
    {
        [XmlElement(ElementName = "Street1")]
        public string Street1 { get; set; }
        [XmlElement(ElementName = "City")]
        public string City { get; set; }
        [XmlElement(ElementName = "State")]
        public string State { get; set; }
        [XmlElement(ElementName = "ZIP")]
        public string Zip { get; set; }

        [XmlElement(ElementName = "Geocode")]
        public Geocode Geocode { get; set; }
        [XmlAttribute(AttributeName = "OutOfCommunity")]
        public string OutOfCommunity { get; set; }
        
    }

    [XmlRoot(ElementName = "SalesOffice")]
    public class SalesOffice
    {
        [XmlElement(ElementName = "Address")]
        public Address Address { get; set; }
        [XmlElement(ElementName = "Email")]
        public string Email { get; set; }
    }

    [XmlRoot(ElementName = "SubGeocode")]
    public class SubGeocode
    {
        [XmlElement(ElementName = "SubLatitude")]
        public string SubLatitude { get; set; }
        [XmlElement(ElementName = "SubLongitude")]
        public string SubLongitude { get; set; }
    }

    [XmlRoot(ElementName = "SubAddress")]
    public class SubAddress
    {
        [XmlElement(ElementName = "SubStreet1")]
        public string SubStreet1 { get; set; }
        [XmlElement(ElementName = "SubStreet2")]
        public string SubStreet2 { get; set; }
        [XmlElement(ElementName = "SubCounty")]
        public string SubCounty { get; set; }
        [XmlElement(ElementName = "SubCity")]
        public string SubCity { get; set; }
        [XmlElement(ElementName = "SubState")]
        public string SubState { get; set; }
        [XmlElement(ElementName = "SubZIP")]
        public string SubZip { get; set; }

        [XmlElement(ElementName = "SubGeocode")]
        public SubGeocode SubGeocode { get; set; }
        [BsonIgnore]
        private string _fullAddress;

        [BsonIgnore]
        private string[] _addressarray;

        public string[] AddressArray
        {
            get
            {
                var addressList = new List<string> { };
                if (!string.IsNullOrEmpty(this.SubStreet1))
                {
                    addressList.AddRange(this.SubStreet1.ToLowerInvariant().Split(' '));

                }
                if (!string.IsNullOrEmpty(this.SubStreet2))
                {
                    addressList.AddRange(this.SubStreet2.ToLowerInvariant().Split(' '));

                }
                if (!string.IsNullOrEmpty(this.SubCity))
                {
                    addressList.AddRange(this.SubCity.ToLowerInvariant().Split(' '));

                }
                if (!string.IsNullOrEmpty(this.SubState))
                {
                    addressList.AddRange(this.SubState.ToLowerInvariant().Split(' '));

                } if (!string.IsNullOrEmpty(this.SubZip))
                {
                    addressList.AddRange(this.SubZip.ToLowerInvariant().Split(' '));

                }

                return addressList.ToArray();
            }
            set
            {
                _addressarray = value;
            }
        }
        public string FullAddress
        {
            get
            {
                TextInfo myTi = new CultureInfo("en-US", false).TextInfo;
                return myTi.ToTitleCase(this.SubCity.ToLower()) + ", " + this.SubState + ", " + this.SubZip;
            }
            set
            {
                _fullAddress = value;
            }
        }

        private GeoPoint _geoPoint;
        [XmlIgnore]
        public GeoPoint GeoPoint
        {
            get
            {
                return new GeoPoint(Convert.ToDouble(SubGeocode.SubLatitude), Convert.ToDouble(SubGeocode.SubLongitude));
            }
            set
            {
                _geoPoint = value;
            }
        }
    }

    [XmlRoot(ElementName = "SubImage")]
    public class SubImage
    {
        [XmlAttribute(AttributeName = "ReferenceType")]
        public string ReferenceType { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "SequencePosition")]
        public string SequencePosition { get; set; }
        [XmlAttribute(AttributeName = "Title")]
        public string Title { get; set; }
        [XmlAttribute(AttributeName = "Caption")]
        public string Caption { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "BasePrice")]
    public class BasePrice
    {
        [XmlAttribute(AttributeName = "ExcludesLand")]
        public string ExcludesLand { get; set; }
        [DisplayFormat(DataFormatString="#,##,##0.00")]
        [XmlText]
        public int Text { get; set; }
    }

    [XmlRoot(ElementName = "Bedrooms")]
    public class Bedrooms
    {
        [XmlAttribute(AttributeName = "MasterBedLocation")]
        public string MasterBedLocation { get; set; }
        [XmlText]
        public int Text { get; set; }
    }

    [XmlRoot(ElementName = "Garage")]
    public class Garage
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry { get; set; }
        [XmlText]
        public int Text { get; set; }
    }

    [XmlRoot(ElementName = "PlanAmenity")]
    public class PlanAmenity
    {
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ElevationImage")]
    public class ElevationImage
    {
        [XmlAttribute(AttributeName = "SequencePosition")]
        public string SequencePosition { get; set; }
        [XmlAttribute(AttributeName = "Title")]
        public string Title { get; set; }
        [XmlAttribute(AttributeName = "Caption")]
        public string Caption { get; set; }
        [XmlAttribute(AttributeName = "ReferenceType")]
        public string ReferenceType { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "PlanImages")]
    public class PlanImages
    {
        [XmlElement(ElementName = "ElevationImage")]
        public ElevationImage ElevationImage { get; set; }
        [XmlElement(ElementName = "VirtualTour")]
        public string VirtualTour { get; set; }
        [XmlElement(ElementName = "PlanViewer")]
        public string PlanViewer { get; set; }
    }

    [XmlRoot(ElementName = "Plan")]
    public class Plan
    {
        [XmlElement(ElementName = "PlanNumber")]
        public string PlanNumber { get; set; }
        [XmlElement(ElementName = "PlanName")]
        public string PlanName { get; set; }
        [XmlElement(ElementName = "BasePrice")]
        public BasePrice BasePrice { get; set; }
        [XmlElement(ElementName = "BaseSqft")]
        public double BaseSqft { get; set; }
        [XmlElement(ElementName = "Stories")]
        public string Stories { get; set; }
        [XmlElement(ElementName = "Baths")]
        public int Baths { get; set; }

        [XmlElement(ElementName = "HalfBaths")]
        public int HalfBaths { get; set; }

        [XmlElement(ElementName = "Bedrooms")]
        public Bedrooms Bedrooms { get; set; }
        [XmlElement(ElementName = "Garage")]
        public Garage Garage { get; set; }
        [XmlElement(ElementName = "Basement")]
        public string Basement { get; set; }
        [XmlElement(ElementName = "PlanAmenity")]
        public List<PlanAmenity> PlanAmenity { get; set; }
        [XmlElement(ElementName = "MarketingHeadline")]
        public string MarketingHeadline { get; set; }
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "PlanImages")]
        public PlanImages PlanImages { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        //public Boolean DeletePlan { get; set; }

        //public bool IsSelected { get; set; }
    }

    [XmlRoot(ElementName = "Subdivision")]
    public class Subdivision
    {
        [XmlElement(ElementName = "SubdivisionNumber")]
        public string SubdivisionNumber { get; set; }
        [XmlElement(ElementName = "SubdivisionName")]
        public string SubdivisionName { get; set; }
        [XmlElement(ElementName = "UseDefaultLeadsEmail")]
        public string UseDefaultLeadsEmail { get; set; }
        [XmlElement(ElementName = "SubLeadsEmail")]
        public string SubLeadsEmail { get; set; }
        [XmlElement(ElementName = "SalesOffice")]
        public SalesOffice SalesOffice { get; set; }
        [XmlElement(ElementName = "SubAddress")]
        public SubAddress SubAddress { get; set; }
        [XmlElement(ElementName = "DrivingDirections")]
        public string DrivingDirections { get; set; }
        [XmlElement(ElementName = "SubDescription")]
        public string SubDescription { get; set; }
        [XmlElement(ElementName = "SubImage")]
        public List<SubImage> SubImage { get; set; }
        [XmlElement(ElementName = "SubWebsite")]
        public string SubWebsite { get; set; }
        [XmlElement(ElementName = "Plan")]
        public List<Plan> Plan { get; set; }
        [XmlAttribute(AttributeName = "Status")]
        public string Status { get; set; }
        [XmlAttribute(AttributeName = "PriceLow")]
        public double PriceLow { get; set; }

        [XmlAttribute(AttributeName = "PriceHigh")]
        public double PriceHigh { get; set; }

        [XmlAttribute(AttributeName = "SqftLow")]
        public double SqftLow { get; set; }

        [XmlAttribute(AttributeName = "SqftHigh")]
        public double SqftHigh { get; set; }
    }

    [XmlRoot(ElementName = "Builder")]
    public class NewHomeListing:BaseEntity
    {
        [XmlElement(ElementName = "BuilderNumber")]
        public string BuilderNumber { get; set; }
        [XmlElement(ElementName = "BrandName")]
        public string BrandName { get; set; }
        [XmlElement(ElementName = "BrandLogo_Med")]
        public BrandLogoMed BrandLogoMed { get; set; }
        [XmlElement(ElementName = "ReportingName")]
        public string ReportingName { get; set; }
        [XmlElement(ElementName = "DefaultLeadsEmail")]
        public DefaultLeadsEmail DefaultLeadsEmail { get; set; }
        [XmlElement(ElementName = "BuilderWebsite")]
        public string BuilderWebsite { get; set; }
        [XmlElement(ElementName = "Subdivision")]
        public Subdivision Subdivision { get; set; }
        public ManagePropertyViewModel ExtProperties { get; set; }
        public string CommunityName { get; set; }

    }

    [XmlRoot(ElementName = "Corporation")]
    public class Corporation
    {
        [XmlElement(ElementName = "CorporateBuilderNumber")]
        public string CorporateBuilderNumber { get; set; }
        [XmlElement(ElementName = "CorporateState")]
        public string CorporateState { get; set; }
        [XmlElement(ElementName = "CorporateName")]
        public string CorporateName { get; set; }
        [XmlElement(ElementName = "Builder")]
        public List<NewHomeListing> NewHomeListings { get; set; }
    }

    [XmlRoot(ElementName = "Builders")]
    public class NewHomeListingRoot
    {
        [XmlElement(ElementName = "Corporation")]
        public Corporation Corporation { get; set; }
        [XmlAttribute(AttributeName = "DateGenerated")]
        public string DateGenerated { get; set; }
    }

}
