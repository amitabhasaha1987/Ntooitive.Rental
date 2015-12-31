using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Repositories.Models.Base;
using Repositories.Models.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace Repositories.Models.ListHub
{

    [XmlRoot(ElementName = "Address", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class Address
    {
        [XmlElement(ElementName = "preference-order", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string Preferenceorder { get; set; }
        [XmlElement(ElementName = "address-preference-order", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string Addresspreferenceorder { get; set; }
        [Required(ErrorMessage = "Street Address Is Required")]
        [XmlElement(ElementName = "FullStreetAddress", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string FullStreetAddress { get; set; }

        [Required(ErrorMessage = "City Is Required")]
        [XmlElement(ElementName = "City", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string City { get; set; }

        [Required(ErrorMessage = "State Is Required")]
        [XmlElement(ElementName = "StateOrProvince", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string StateOrProvince { get; set; }

        [Required(ErrorMessage = "PostalCode Is Required")]
        [XmlElement(ElementName = "PostalCode", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string PostalCode { get; set; }
        [XmlElement(ElementName = "Country", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string Country { get; set; }

        [BsonIgnore]
        private string _fullAddress;

        [BsonIgnore]
        private string[] _addressarray;

        public string[] AddressArray
        {
            get
            {
                var addressList = new List<string> { };
                if (!string.IsNullOrEmpty(this.FullStreetAddress))
                {
                    addressList.AddRange(this.FullStreetAddress.ToLowerInvariant().Split(' '));

                }
                if (!string.IsNullOrEmpty(this.City))
                {
                    addressList.AddRange(this.City.ToLowerInvariant().Split(' '));

                }
                if (!string.IsNullOrEmpty(this.StateOrProvince))
                {
                    addressList.AddRange(this.StateOrProvince.ToLowerInvariant().Split(' '));

                } if (!string.IsNullOrEmpty(this.PostalCode))
                {
                    addressList.AddRange(this.PostalCode.ToLowerInvariant().Split(' '));

                }

                return addressList.ToArray();
            }
            set
            {
                _addressarray = value;
            }
        }
        #region "ClassifiedFeed"
        public string AdId { get; set; }
        #endregion
        public string FullAddress
        {
            get
            {
                TextInfo myTi = new CultureInfo("en-US", false).TextInfo;

                string temp = string.Empty;
                if (!String.IsNullOrEmpty(this.City))
                    temp = this.City;
                if (!String.IsNullOrEmpty(this.StateOrProvince))
                    temp = temp + " " + this.StateOrProvince;
                if (!String.IsNullOrEmpty(this.PostalCode))
                    temp = temp + " " + this.PostalCode;

                return myTi.ToTitleCase(temp);
                // return myTi.ToTitleCase(this.City.ToLower()) + ", " + this.StateOrProvince + ", " + this.PostalCode;
            }
            set
            {
                _fullAddress = value;
            }
        }
    }

    [XmlRoot(ElementName = "ListPrice", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class ListPrice
    {
        [XmlAttribute(AttributeName = "isgSecurityClass", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string IsgSecurityClass { get; set; }
        [XmlText]
        public double Text { get; set; }
    }

    [XmlRoot(ElementName = "PropertyType", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class PropertyType
    {
        [XmlAttribute(AttributeName = "otherDescription")]
        public string OtherDescription { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "PropertySubType", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class PropertySubType
    {
        [XmlAttribute(AttributeName = "otherDescription")]
        public string OtherDescription { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "PermitAddressOnInternet", Namespace = "http://rets.org/xsd/RETSCommons")]
    public class PermitAddressOnInternet
    {
        [XmlAttribute(AttributeName = "isgSecurityClass", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string IsgSecurityClass { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "VOWAddressDisplay", Namespace = "http://rets.org/xsd/RETSCommons")]
    public class VOWAddressDisplay
    {
        [XmlAttribute(AttributeName = "isgSecurityClass", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string IsgSecurityClass { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "VOWAutomatedValuationDisplay", Namespace = "http://rets.org/xsd/RETSCommons")]
    public class VOWAutomatedValuationDisplay
    {
        [XmlAttribute(AttributeName = "isgSecurityClass", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string IsgSecurityClass { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "VOWConsumerComment", Namespace = "http://rets.org/xsd/RETSCommons")]
    public class VOWConsumerComment
    {
        [XmlAttribute(AttributeName = "isgSecurityClass", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string IsgSecurityClass { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "MarketingInformation", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class MarketingInformation
    {
        [XmlElement(ElementName = "PermitAddressOnInternet", Namespace = "http://rets.org/xsd/RETSCommons")]
        public PermitAddressOnInternet PermitAddressOnInternet { get; set; }
        [XmlElement(ElementName = "VOWAddressDisplay", Namespace = "http://rets.org/xsd/RETSCommons")]
        public VOWAddressDisplay VOWAddressDisplay { get; set; }
        [XmlElement(ElementName = "VOWAutomatedValuationDisplay", Namespace = "http://rets.org/xsd/RETSCommons")]
        public VOWAutomatedValuationDisplay VOWAutomatedValuationDisplay { get; set; }
        [XmlElement(ElementName = "VOWConsumerComment", Namespace = "http://rets.org/xsd/RETSCommons")]
        public VOWConsumerComment VOWConsumerComment { get; set; }
    }

    [XmlRoot(ElementName = "MediaModificationTimestamp", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class MediaModificationTimestamp
    {
        [XmlAttribute(AttributeName = "isgSecurityClass", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string IsgSecurityClass { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Photo", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class Photo
    {
        [XmlElement(ElementName = "MediaModificationTimestamp", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public MediaModificationTimestamp MediaModificationTimestamp { get; set; }
        [XmlElement(ElementName = "MediaURL", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string MediaURL { get; set; }
    }

    [XmlRoot(ElementName = "Photos", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class Photos
    {
        [XmlElement(ElementName = "Photo", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public List<Photo> Photo { get; set; }
    }

    [XmlRoot(ElementName = "LotSize", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class LotSize
    {
        [XmlAttribute(AttributeName = "areaUnits", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string AreaUnits { get; set; }
        [XmlText]
        public double Text { get; set; }
    }

    [XmlRoot(ElementName = "Participant", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class Participant : BaseEntity
    {
        [XmlElement(ElementName = "ParticipantKey", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string ParticipantKey { get; set; }
        [XmlElement(ElementName = "ParticipantId", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string ParticipantId { get; set; }
        [XmlElement(ElementName = "FirstName", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string FirstName { get; set; }
        [XmlElement(ElementName = "LastName", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string LastName { get; set; }
        [XmlElement(ElementName = "Role", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string Role { get; set; }
        [XmlElement(ElementName = "PrimaryContactPhone", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string PrimaryContactPhone { get; set; }
        [XmlElement(ElementName = "OfficePhone", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string OfficePhone { get; set; }
        [XmlElement(ElementName = "Email", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string Email { get; set; }
        [XmlElement(ElementName = "WebsiteURL", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string WebsiteURL { get; set; }
        public string AgentDescription { get; set; }
        public List<Office> GetOfficeList { get; set; }
        public string OfficeName { get; set; }
        public string ProfileImage { get; set; }
        public string OfficeImage { get; set; }
        public string OfficeId { get; set; }
        public string LogoImage { get; set; }
    }

    [XmlRoot(ElementName = "ListingParticipants", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class ListingParticipants
    {
        [XmlElement(ElementName = "Participant", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Participant Participant { get; set; }
    }

    [XmlRoot(ElementName = "VirtualTour", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class VirtualTour
    {
        [XmlElement(ElementName = "MediaModificationTimestamp", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public MediaModificationTimestamp MediaModificationTimestamp { get; set; }
        [XmlElement(ElementName = "MediaURL", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string MediaURL { get; set; }
    }

    [XmlRoot(ElementName = "VirtualTours", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class VirtualTours
    {
        [XmlElement(ElementName = "VirtualTour", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public VirtualTour VirtualTour { get; set; }
    }

    [XmlRoot(ElementName = "OfficeCode", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class OfficeCode
    {
        [XmlElement(ElementName = "OfficeCodeId", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string OfficeCodeId { get; set; }
    }

    [XmlRoot(ElementName = "Office", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class Office : BaseEntity
    {
        [XmlElement(ElementName = "OfficeKey", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string OfficeKey { get; set; }
        [XmlElement(ElementName = "OfficeId", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string OfficeId { get; set; }
        [XmlElement(ElementName = "OfficeCode", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public OfficeCode OfficeCode { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string Name { get; set; }
        [XmlElement(ElementName = "CorporateName", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string CorporateName { get; set; }
        [XmlElement(ElementName = "BrokerId", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string BrokerId { get; set; }
        [XmlElement(ElementName = "MainOfficeId", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string MainOfficeId { get; set; }
        [XmlElement(ElementName = "PhoneNumber", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string PhoneNumber { get; set; }
        [XmlElement(ElementName = "Address", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Address Address { get; set; }
        [XmlElement(ElementName = "OfficeEmail", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string OfficeEmail { get; set; }
        [XmlElement(ElementName = "Website", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string Website { get; set; }
        public string OfficeImageUrl { get; set; }
        public string OfficeDescription { get; set; }
        public string OfficeLogo { get; set; }
        public bool IsAllRentPropertyFeatured { get; set; }
        public bool IsAllPurchasePropertyFeatured { get; set; }
    }

    [XmlRoot(ElementName = "Offices", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class Offices
    {
        [XmlElement(ElementName = "Office", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Office Office { get; set; }
    }

    [XmlRoot(ElementName = "Brokerage", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class Brokerage
    {
        [XmlElement(ElementName = "Name", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Phone", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string Phone { get; set; }
        [XmlElement(ElementName = "Email", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string Email { get; set; }
        [XmlElement(ElementName = "WebsiteURL", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string WebsiteURL { get; set; }
        [XmlElement(ElementName = "LogoURL", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string LogoURL { get; set; }
        [XmlElement(ElementName = "Address", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Address Address { get; set; }
    }

    [XmlRoot(ElementName = "Builder", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class Builder
    {
        [XmlElement(ElementName = "Name", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "Subdivision", Namespace = "http://rets.org/xsd/RETSCommons")]
    public class Subdivision
    {
        [XmlAttribute(AttributeName = "isgSecurityClass", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string IsgSecurityClass { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "District", Namespace = "http://rets.org/xsd/RETSCommons")]
    public class District
    {
        [XmlAttribute(AttributeName = "isgSecurityClass", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string IsgSecurityClass { get; set; }
    }

    [XmlRoot(ElementName = "School", Namespace = "http://rets.org/xsd/RETSCommons")]
    public class School
    {
        [XmlElement(ElementName = "Name", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string Name { get; set; }
        [XmlElement(ElementName = "SchoolCategory", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string SchoolCategory { get; set; }
        [XmlElement(ElementName = "District", Namespace = "http://rets.org/xsd/RETSCommons")]
        public District District { get; set; }
    }

    [XmlRoot(ElementName = "Schools", Namespace = "http://rets.org/xsd/RETSCommons")]
    public class Schools
    {
        [XmlElement(ElementName = "School", Namespace = "http://rets.org/xsd/RETSCommons")]
        public List<School> School { get; set; }
    }

    [XmlRoot(ElementName = "Community", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class Community
    {
        [XmlElement(ElementName = "Subdivision", Namespace = "http://rets.org/xsd/RETSCommons")]
        public Subdivision Subdivision { get; set; }
        [XmlElement(ElementName = "Schools", Namespace = "http://rets.org/xsd/RETSCommons")]
        public Schools Schools { get; set; }
    }

    [XmlRoot(ElementName = "Location", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class Location
    {
        [XmlElement(ElementName = "Latitude", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public double Latitude { get; set; }
        [XmlElement(ElementName = "Longitude", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public double Longitude { get; set; }
        [XmlElement(ElementName = "Directions", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string Directions { get; set; }
        [XmlElement(ElementName = "County", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string County { get; set; }
        [XmlElement(ElementName = "ParcelId", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string ParcelId { get; set; }
        [XmlElement(ElementName = "Community", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Community Community { get; set; }

        private GeoPoint _geoPoint;
        [XmlIgnore]
        public GeoPoint GeoPoint
        {
            get
            {
                return new GeoPoint(Convert.ToDouble(this.Latitude), Convert.ToDouble(this.Longitude));
            }
            set
            {
                _geoPoint = value;
            }
        }

    }

    [XmlRoot(ElementName = "ExpenseValue", Namespace = "http://rets.org/xsd/RETSCommons")]
    public class ExpenseValue
    {
        [XmlAttribute(AttributeName = "isgSecurityClass", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string IsgSecurityClass { get; set; }
        [XmlAttribute(AttributeName = "currencyPeriod", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string CurrencyPeriod { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Expense", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class Expense
    {
        [XmlElement(ElementName = "ExpenseCategory", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string ExpenseCategory { get; set; }
        [XmlElement(ElementName = "ExpenseValue", Namespace = "http://rets.org/xsd/RETSCommons")]
        public ExpenseValue ExpenseValue { get; set; }
    }

    [XmlRoot(ElementName = "Expenses", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class Expenses
    {
        [XmlElement(ElementName = "Expense", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Expense Expense { get; set; }
    }

    [XmlRoot(ElementName = "Appliances", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class Appliances
    {
        [XmlElement(ElementName = "Appliance", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public List<string> Appliance { get; set; }
    }

    [XmlRoot(ElementName = "ArchitectureStyle", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class ArchitectureStyle
    {
        [XmlAttribute(AttributeName = "otherDescription")]
        public string OtherDescription { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "CoolingSystems", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class CoolingSystems
    {
        [XmlElement(ElementName = "CoolingSystem", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string CoolingSystem { get; set; }
    }

    [XmlRoot(ElementName = "ExteriorTypes", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class ExteriorTypes
    {
        [XmlElement(ElementName = "ExteriorType", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public List<string> ExteriorType { get; set; }
    }

    [XmlRoot(ElementName = "HeatingSystems", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class HeatingSystems
    {
        [XmlElement(ElementName = "HeatingSystem", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string HeatingSystem { get; set; }
    }

    [XmlRoot(ElementName = "ParkingTypes", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class ParkingTypes
    {
        [XmlElement(ElementName = "ParkingType", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public List<string> ParkingType { get; set; }
    }

    [XmlRoot(ElementName = "RoofTypes", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class RoofTypes
    {
        [XmlElement(ElementName = "RoofType", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string RoofType { get; set; }
    }

    [XmlRoot(ElementName = "Rooms", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class Rooms
    {
        [XmlElement(ElementName = "Room", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public List<string> Room { get; set; }
    }

    [XmlRoot(ElementName = "DetailedCharacteristics", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class DetailedCharacteristics
    {
        [XmlElement(ElementName = "Appliances", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Appliances Appliances { get; set; }
        [XmlElement(ElementName = "ArchitectureStyle", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public ArchitectureStyle ArchitectureStyle { get; set; }
        [XmlElement(ElementName = "HasBasement", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public bool HasBasement { get; set; }
        [XmlElement(ElementName = "CoolingSystems", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public CoolingSystems CoolingSystems { get; set; }
        [XmlElement(ElementName = "ExteriorTypes", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public ExteriorTypes ExteriorTypes { get; set; }
        [XmlElement(ElementName = "HeatingSystems", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public HeatingSystems HeatingSystems { get; set; }
        [XmlElement(ElementName = "IsNewConstruction", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public bool IsNewConstruction { get; set; }
        [XmlElement(ElementName = "NumFloors", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public double NumFloors { get; set; }
        [XmlElement(ElementName = "NumParkingSpaces", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string NumParkingSpaces { get; set; }
        [XmlElement(ElementName = "ParkingTypes", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public ParkingTypes ParkingTypes { get; set; }
        [XmlElement(ElementName = "RoofTypes", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public RoofTypes RoofTypes { get; set; }
        [XmlElement(ElementName = "Rooms", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Rooms Rooms { get; set; }
    }

    [XmlRoot(ElementName = "ModificationTimestamp", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class ModificationTimestamp
    {
        [XmlAttribute(AttributeName = "isgSecurityClass", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string IsgSecurityClass { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Disclaimer", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class Disclaimer
    {
        [XmlAttribute(AttributeName = "isgSecurityClass", Namespace = "http://rets.org/xsd/RETSCommons")]
        public string IsgSecurityClass { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Listing", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class ListHubListing : BaseEntity
    {
        [XmlElement(ElementName = "Address", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Address Address { get; set; }
        [XmlElement(ElementName = "ListPrice", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public ListPrice ListPrice { get; set; }
        [XmlElement(ElementName = "ListingURL", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string ListingURL { get; set; }

        [Required(ErrorMessage = "Provider Name Is Required")]
        [XmlElement(ElementName = "ProviderName", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string ProviderName { get; set; }

        //[Required(ErrorMessage = "Provider Url Is Required")]
        [XmlElement(ElementName = "ProviderURL", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string ProviderURL { get; set; }
        [XmlElement(ElementName = "ProviderCategory", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string ProviderCategory { get; set; }
        [XmlElement(ElementName = "LeadRoutingEmail", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string LeadRoutingEmail { get; set; }
        [XmlElement(ElementName = "Bedrooms", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public int Bedrooms { get; set; }
        [XmlElement(ElementName = "Bathrooms", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public int Bathrooms { get; set; }
        [XmlElement(ElementName = "PropertyType", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public PropertyType PropertyType { get; set; }
        [XmlElement(ElementName = "PropertySubType", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public PropertySubType PropertySubType { get; set; }
        [XmlElement(ElementName = "ListingKey", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string ListingKey { get; set; }
        [XmlElement(ElementName = "ListingCategory", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string ListingCategory { get; set; }
        [XmlElement(ElementName = "ListingStatus", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string ListingStatus { get; set; }
        [XmlElement(ElementName = "MarketingInformation", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public MarketingInformation MarketingInformation { get; set; }
        [XmlElement(ElementName = "Photos", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Photos Photos { get; set; }
        [XmlElement(ElementName = "DiscloseAddress", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string DiscloseAddress { get; set; }
        [XmlElement(ElementName = "ListingDescription", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string ListingDescription { get; set; }
        [XmlElement(ElementName = "MlsId", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string MlsId { get; set; }
        [XmlElement(ElementName = "MlsName", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string MlsName { get; set; }
        [XmlElement(ElementName = "MlsNumber", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string MlsNumber { get; set; }
        [XmlElement(ElementName = "LivingArea", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public double LivingArea { get; set; }
        [XmlElement(ElementName = "LotSize", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public LotSize LotSize { get; set; }
        [XmlElement(ElementName = "YearBuilt", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public int YearBuilt { get; set; }
        [XmlElement(ElementName = "ListingDate", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string ListingDate { get; set; }
        [XmlElement(ElementName = "ListingTitle", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public string ListingTitle { get; set; }
        [XmlElement(ElementName = "FullBathrooms", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public int FullBathrooms { get; set; }
        [XmlElement(ElementName = "ThreeQuarterBathrooms", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public int ThreeQuarterBathrooms { get; set; }
        [XmlElement(ElementName = "HalfBathrooms", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public int HalfBathrooms { get; set; }
        [XmlElement(ElementName = "OneQuarterBathrooms", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public int OneQuarterBathrooms { get; set; }
        [XmlElement(ElementName = "PartialBathrooms", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public int PartialBathrooms { get; set; }
        [XmlElement(ElementName = "ListingParticipants", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public ListingParticipants ListingParticipants { get; set; }
        [XmlElement(ElementName = "VirtualTours", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public VirtualTours VirtualTours { get; set; }
        [XmlElement(ElementName = "Offices", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Offices Offices { get; set; }
        [XmlElement(ElementName = "Brokerage", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Brokerage Brokerage { get; set; }
        [XmlElement(ElementName = "Builder", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Builder Builder { get; set; }
        [XmlElement(ElementName = "Location", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Location Location { get; set; }
        [XmlElement(ElementName = "Expenses", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Expenses Expenses { get; set; }
        [XmlElement(ElementName = "DetailedCharacteristics", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public DetailedCharacteristics DetailedCharacteristics { get; set; }
        [XmlElement(ElementName = "ModificationTimestamp", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public ModificationTimestamp ModificationTimestamp { get; set; }
        [XmlElement(ElementName = "Disclaimer", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public Disclaimer Disclaimer { get; set; }

        public ManagePropertyViewModel ExtProperties { get; set; }

        [Required(ErrorMessage = "Community Name is required")]
        public string[] CommunityName { get; set; }
        #region "Added for ClassifiedFeed"
        public DateTime? ExpireDate { get; set; }
        public string HiddenAdress { get; set; }
        //     public Images Images { get; set; }
        public string Minprice { get; set; }
        public string UpsellFeaturedAd { get; set; }

        public string AdId { get; set; }

        public string UpsellSpotlightAd { get; set; }
        public bool HasAddress { get; set; }
        public bool IsClassified { get; set; }
        #endregion
    }

    [XmlRoot(ElementName = "Listings", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
    public class ListHubListingRoot
    {
        [XmlElement(ElementName = "Listing", Namespace = "http://rets.org/xsd/Syndication/2012-03")]
        public List<ListHubListing> Listing { get; set; }

        public string ListingsKey { get; set; }
    }

    public class GeoPoint
    {
        public GeoPoint(double lat, double lang)
        {
            Coordinates = new List<double> { lang, lat };
            this.Type = "Point";
        }
        [BsonElement("type")]
        public string Type { get; set; }//Point
        [BsonElement("coordinates")]
        public List<double> Coordinates { get; set; }
    }
}
