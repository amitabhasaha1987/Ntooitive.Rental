using MongoDB.Bson.Serialization.Attributes;
using Repositories.Models.Base;
using Repositories.Models.ListHub;
using Repositories.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Repositories.Models.NewHome
{

    [XmlRoot(ElementName = "community_amenity")]
    public class Community_amenity
    {
        [XmlElement(ElementName = "category")]
        public string Category { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "website")]
        public string Website { get; set; }

        #region "For BDX Feed"
        [XmlIgnore]
        public string TextValue { get; set; }
        #endregion
       
    }

    [XmlRoot(ElementName = "community_amenities")]
    public class Community_amenities
    {
        public Community_amenities() {
            Community_amenity = new List<Community_amenity>();
        }
        [XmlElement(ElementName = "community_amenity")]
        public List<Community_amenity> Community_amenity { get; set; }
    }

    [XmlRoot(ElementName = "image")]
    public class Image
    {
        public Image() { 
        
        }

        [XmlElement(ElementName = "reference")]
        public string Reference { get; set; }
        [XmlElement(ElementName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "sequence_position")]
        public string Sequence_position { get; set; }
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "caption")]
        public string Caption { get; set; }
    }

    [XmlRoot(ElementName = "images")]
    public class Images
    {
        public Images()
        {
            Image = new List<Image>();
        }
        [XmlElement(ElementName = "image")]
        public List<Image> Image { get; set; }
    }

    [XmlRoot(ElementName = "sales_agent")]
    public class Sales_agent
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "agent_email")]
        public string Agent_email { get; set; }
        [XmlElement(ElementName = "agent_phone")]
        public string Agent_phone { get; set; }
        [XmlElement(ElementName = "phone_ext")]
        public string Phone_ext { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "sales_agents")]
    public class Sales_agents
    {
        [XmlElement(ElementName = "sales_agent")]
        public Sales_agent Sales_agent { get; set; }
    }

    [XmlRoot(ElementName = "video")]
    public class Video
    {
        [XmlElement(ElementName = "sequence_position")]
        public string Sequence_position { get; set; }
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
    }

    [XmlRoot(ElementName = "community_videos")]
    public class Community_videos
    {
        [XmlElement(ElementName = "video")]
        public Video Video { get; set; }
    }

    [XmlRoot(ElementName = "sales_office")]
    public class Sales_office
    {
        [XmlElement(ElementName = "address")]
        public string Address { get; set; }
        [XmlElement(ElementName = "address_2")]
        public string Address_2 { get; set; }
        [XmlElement(ElementName = "city")]
        public string City { get; set; }
        [XmlElement(ElementName = "state")]
        public string State { get; set; }
        [XmlElement(ElementName = "zip")]
        public string Zip { get; set; }
        [XmlElement(ElementName = "latitude")]
        public string Latitude { get; set; }
        [XmlElement(ElementName = "longitude")]
        public string Longitude { get; set; }
        [XmlElement(ElementName = "office_phone")]
        public string Office_phone { get; set; }
        [XmlElement(ElementName = "fax")]
        public string Fax { get; set; }
        [XmlElement(ElementName = "office_email")]
        public string Office_email { get; set; }
        [XmlElement(ElementName = "hours")]
        public string Hours { get; set; }
        [XmlElement(ElementName = "mls_code")]
        public string Mls_code { get; set; }
        [XmlElement(ElementName = "website")]
        public string Website { get; set; }
        [XmlElement(ElementName = "sales_agents")]
        public Sales_agents Sales_agents { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "homes")]
    public class Homes
    {
        [XmlAttribute(AttributeName = "total")]
        public string Total { get; set; }
        [XmlElement(ElementName = "home")]
        public List<Home> Home { get; set; }
    }

    [XmlRoot(ElementName = "plan")]
    public class Plan : BaseEntity
    {
        //start added property
        [XmlIgnore]
        public string BuilderNumber { get; set; }
        [XmlIgnore]
        public string BuilderName { get; set; }
        [XmlIgnore]
        public string BuilderEmail { get; set; }
        [XmlIgnore]
        public string Builder_dre_number { get; set; }
        [XmlIgnore]
        public ManagePropertyViewModel ExtProperties { get; set; }
        [XmlIgnore]
        public string CommunityName { get; set; }
        [XmlIgnore]
        public string CommunityNumber { get; set; }
        [XmlIgnore]
        public string CommunityWebsite { get; set; }
        [XmlIgnore]
        public double Communityprice_low { get; set; }
        [XmlIgnore]
        public double Communityprice_high { get; set; }
        [XmlIgnore]
        public double Communitysqft_high { get; set; }
        [XmlIgnore]
        public double Communitysqft_low { get; set; }
        [XmlIgnore]
        public string Communityaddress { get; set; }
        [XmlIgnore]
        public string Communitycity { get; set; }
        [XmlIgnore]
        public string Communitystate { get; set; }
        [XmlIgnore]
        public string Communityzip { get; set; }

        #region "For BDX Feed"
        [XmlIgnore]
        public string CommunityPhone { get; set; }
        [XmlIgnore]
        public string CommunityMarketID { get; set; }
        [XmlIgnore]
        public string CommunityMarketName { get; set; }
        #endregion
        [XmlIgnore]
        public double Latitude { get; set; }
        [XmlIgnore]
        public double Longitude { get; set; }




        private GeoPoint _geoPoint;
        [XmlIgnore]
        public GeoPoint GeoPoint
        {
            get
            {
                return new GeoPoint(Convert.ToDouble(Latitude), Convert.ToDouble(Longitude));
            }
            set
            {
                _geoPoint = value;
            }
        }
        [BsonIgnore]
        private string _fullAddress;
        public string FullAddress
        {
            get
            {
                TextInfo myTi = new CultureInfo("en-US", false).TextInfo;
                return myTi.ToTitleCase(string.IsNullOrEmpty(this.Communitystate) ? "" : this.Communitystate.ToLower()) + ", " + this.Communitycity + ", " + this.Communityzip;
            }
            set
            {
                _fullAddress = value;
            }
        }




        //end added property

        [XmlElement(ElementName = "number")]
        public string Number { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "style")]
        public string Style { get; set; }
        [XmlElement(ElementName = "base_price")]
        public string Base_price { get; set; }
        [XmlElement(ElementName = "sqft_low")]
        public string Sqft_low { get; set; }
        [XmlElement(ElementName = "stories")]
        public string Stories { get; set; }

        //this is considered as Baths_high
        [XmlElement(ElementName = "baths")]
        public int Baths { get; set; }

        [XmlIgnore]
        public int Baths_low { get; set; }
        //this is considered as Half_baths_high
        [XmlElement(ElementName = "half_baths")]
        public int Half_baths { get; set; }
        [XmlIgnore]
        public int Half_baths_low { get; set; }
        //this is considered as bedrooms_high
        [XmlElement(ElementName = "bedrooms")]
        public int Bedrooms { get; set; }
        [XmlIgnore]
        public int Bedrooms_low { get; set; }
        [XmlElement(ElementName = "master_bedroom_location")]
        public string Master_bedroom_location { get; set; }
        //this is considered as garage_high
        [XmlElement(ElementName = "garage")]
        public int Garage { get; set; }
        [XmlIgnore]
        public int Garage_low { get; set; }
        [XmlElement(ElementName = "garage_entry")]
        public string Garage_entry { get; set; }
        [XmlElement(ElementName = "garage_detach")]
        public string Garage_detach { get; set; }
        [XmlElement(ElementName = "dining_areas")]
        public string Dining_areas { get; set; }
        [XmlElement(ElementName = "basement")]
        public int Basement { get; set; }
        [XmlElement(ElementName = "land_price")]
        public string Land_price { get; set; }
        [XmlElement(ElementName = "marketing_headline")]
        public string Marketing_headline { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "virtual_tour")]
        public string Virtual_tour { get; set; }
        [XmlElement(ElementName = "plan_viewer")]
        public string Plan_viewer { get; set; }
        [XmlElement(ElementName = "brochure")]
        public string Brochure { get; set; }
        [XmlElement(ElementName = "plan_amenities")]
        public string Plan_amenities { get; set; }
        [XmlElement(ElementName = "images")]
        public Images Images { get; set; }
        [XmlElement(ElementName = "plan_options")]
        public string Plan_options { get; set; }
        [XmlElement(ElementName = "homes")]
        public Homes Homes { get; set; }
        [XmlAttribute(AttributeName = "is_active")]
        public string Is_active { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string PlanId { get; set; }
        [XmlIgnore]
        public List<Repositories.Models.Community.Communities> CommunityList { get; set; }
        [XmlIgnore]
        public List<KeyValuePair<string, string>> BuilderList { get; set; }
        [XmlIgnore]
        public List<Plan> Plans { get; set; }

        #region "For BDX Feed"
         [XmlIgnore]
        public string LivingAreas { get; set; }
         [XmlIgnore]
        public int HalfBedrooms { get; set; }

         [XmlIgnore]
         public string communityImageUrl { get; set; }


        [XmlIgnore]
        public string BuilderLogo { get; set; }
        [XmlIgnore]
        public string BuilderWebsite { get; set; }
        #endregion
    }

    [XmlRoot(ElementName = "plans")]
    public class Plans
    {
        [XmlElement(ElementName = "plan")]
        public List<Plan> Plan { get; set; }
        [XmlAttribute(AttributeName = "total")]
        public string Total { get; set; }
    }

    [XmlRoot(ElementName = "community")]
    public class Community
    {


        [XmlElement(ElementName = "number")]
        public string Number { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "leads_email")]
        public string Leads_email { get; set; }
        [XmlElement(ElementName = "website")]
        public string Website { get; set; }
        [XmlElement(ElementName = "share_with_realtors")]
        public string Share_with_realtors { get; set; }
        [XmlElement(ElementName = "price_low")]
        public string Price_low { get; set; }
        [XmlElement(ElementName = "price_high")]
        public string Price_high { get; set; }
        [XmlElement(ElementName = "sqft_low")]
        public string Sqft_low { get; set; }
        [XmlElement(ElementName = "sqft_high")]
        public string Sqft_high { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "marketing_status")]
        public string Marketing_status { get; set; }
        [XmlElement(ElementName = "address")]
        public string Address { get; set; }
        [XmlElement(ElementName = "address_2")]
        public string Address_2 { get; set; }
        [XmlElement(ElementName = "city")]
        public string City { get; set; }
        [XmlElement(ElementName = "state")]
        public string State { get; set; }
        [XmlElement(ElementName = "zip")]
        public string Zip { get; set; }
        [XmlElement(ElementName = "county")]
        public string County { get; set; }
        [XmlElement(ElementName = "country")]
        public string Country { get; set; }
        [XmlElement(ElementName = "driving_directions")]
        public string Driving_directions { get; set; }
        [XmlElement(ElementName = "latitude")]
        public double Latitude { get; set; }
        [XmlElement(ElementName = "longitude")]
        public double Longitude { get; set; }
        [XmlElement(ElementName = "promo_headline")]


        public string Promo_headline { get; set; }
        [XmlElement(ElementName = "promo_description")]
        public string Promo_description { get; set; }
        [XmlElement(ElementName = "promo_url")]
        public string Promo_url { get; set; }
        [XmlElement(ElementName = "neighborhood_name")]
        public string Neighborhood_name { get; set; }
        [XmlElement(ElementName = "neighborhood_description")]
        public string Neighborhood_description { get; set; }
        [XmlElement(ElementName = "style")]
        public string Style { get; set; }

        [XmlElement(ElementName = "images")]
        public Images Images { get; set; }
        [XmlElement(ElementName = "community_services")]
        public string Community_services { get; set; }
        [XmlElement(ElementName = "community_testimonials")]
        public string Community_testimonials { get; set; }
        [XmlElement(ElementName = "community_utilities")]
        public string Community_utilities { get; set; }

        [XmlElement(ElementName = "sales_office")]
        public Sales_office Sales_office { get; set; }
        [XmlElement(ElementName = "plans")]
        public Plans Plans { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string CommunityId { get; set; }
        [XmlAttribute(AttributeName = "is_active")]
        public string Is_active { get; set; }
        [XmlElement(ElementName = "schools")]
        public Schools Schools { get; set; }

        [XmlElement(ElementName = "community_videos")]
        public Community_videos Community_videos { get; set; }

        [XmlElement(ElementName = "community_amenities")]
        public Community_amenities Community_amenities { get; set; }

        #region "For BDX Feed"
        [XmlIgnore]
        public string Phone { get; set; }
       [XmlIgnore]
        public string MarketID { get; set; }
        [XmlIgnore]
        public string MarketName { get; set; }
        #endregion
    }

    [XmlRoot(ElementName = "home")]
    public class Home
    {
        [XmlElement(ElementName = "number")]
       
        public string Number { get; set; }
        
        [XmlElement(ElementName = "listing_title")]
        [Required(ErrorMessage = "Please Enter Title")]
        public string Listing_title { get; set; }

        [XmlElement(ElementName = "home_type")]
        [Required(ErrorMessage = "Please Enter Home Type")]
        public string Home_type { get; set; }
        [XmlElement(ElementName = "property_type")]
        public string Property_type { get; set; }
        [XmlElement(ElementName = "sales_status")]
        public string Sales_status { get; set; }
        [XmlElement(ElementName = "lot")]
        public string Lot { get; set; }
        [XmlElement(ElementName = "parcel_id")]
        public string Parcel_id { get; set; }
        [XmlElement(ElementName = "lot_size")]
        public string Lot_size { get; set; }
        [XmlElement(ElementName = "display_address")]
        public string Display_address { get; set; }
        [XmlElement(ElementName = "address")]
        [Required(ErrorMessage = "Please Enter Address")]
        public string Address { get; set; }
        [XmlElement(ElementName = "address_2")]
        public string Address_2 { get; set; }
        [XmlElement(ElementName = "city")]
        [Required(ErrorMessage = "Please Enter City")]
        public string City { get; set; }
        [XmlElement(ElementName = "state")]
        [Required(ErrorMessage = "Please Enter State")]
        public string State { get; set; }
        [XmlElement(ElementName = "zip")]
        [Required(ErrorMessage = "Please Enter Zip")]
        public string Zip { get; set; }
        [XmlElement(ElementName = "street_intersection")]
        public string Street_intersection { get; set; }
        [XmlElement(ElementName = "county")]
        [Required(ErrorMessage = "Please Enter County")]
        public string County { get; set; }
        [XmlElement(ElementName = "country")]
        [Required(ErrorMessage = "Please Enter Country")]
        public string Country { get; set; }
        [XmlElement(ElementName = "latitude")]
        [Required(ErrorMessage = "Please Enter Latitude")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Please enter Number only")]
        public string Latitude { get; set; }
        [XmlElement(ElementName = "longitude")]
        [Required(ErrorMessage = "Please Enter Longitude")]
        [RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "Please enter Number only")]
        public string Longitude { get; set; }
        [XmlElement(ElementName = "elevation")]
        public string Elevation { get; set; }
        [XmlElement(ElementName = "date_listed")]
        public string Date_listed { get; set; }
        [XmlElement(ElementName = "date_sold")]
        public string Date_sold { get; set; }
        [XmlElement(ElementName = "move_in_date")]
        public string Move_in_date { get; set; }
        [XmlElement(ElementName = "term")]
        public string Term { get; set; }
        [XmlElement(ElementName = "price")]
        [Required(ErrorMessage = "Please Enter Price")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Please enter Number only")]
        public string Price { get; set; }
        [XmlElement(ElementName = "sqft")]
        [Required(ErrorMessage = "Please Enter Sqft")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter Number only")]
        public string Sqft { get; set; }
        [XmlElement(ElementName = "stories")]
        public string Stories { get; set; }
        [XmlElement(ElementName = "unit_level")]
        public string Unit_level { get; set; }
        [XmlElement(ElementName = "number_of_units")]
        public string Number_of_units { get; set; }
        [XmlElement(ElementName = "location")]
        public string Location { get; set; }
        [XmlElement(ElementName = "year_built")]
        [Required(ErrorMessage = "Please Enter Year Built")]
        public string Year_built { get; set; }
        [XmlElement(ElementName = "baths")]
        public string Baths { get; set; }
        [XmlElement(ElementName = "half_baths")]
        public string Half_baths { get; set; }
        [XmlElement(ElementName = "bedrooms")]
        public string Bedrooms { get; set; }
        [XmlElement(ElementName = "master_bedroom_location")]
        public string Master_bedroom_location { get; set; }
        [XmlElement(ElementName = "garage")]
        public string Garage { get; set; }
        [XmlElement(ElementName = "garage_entry")]
        public string Garage_entry { get; set; }
        [XmlElement(ElementName = "garage_detach")]
        public string Garage_detach { get; set; }
        [XmlElement(ElementName = "dining_areas")]
        public string Dining_areas { get; set; }
        [XmlElement(ElementName = "basement")]
        public string Basement { get; set; }
        [XmlElement(ElementName = "marketing_headline")]
        public string Marketing_headline { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "directions")]
        public string Directions { get; set; }
        [XmlElement(ElementName = "listing_url")]
        public string Listing_url { get; set; }
        [XmlElement(ElementName = "mls_id")]
        public string Mls_id { get; set; }
        [XmlElement(ElementName = "mls_name")]
        public string Mls_name { get; set; }
        [XmlElement(ElementName = "mls_number")]
        public string Mls_number { get; set; }
        [XmlElement(ElementName = "neighborhood_name")]
        public string Neighborhood_name { get; set; }
        [XmlElement(ElementName = "neighborhood_description")]
        public string Neighborhood_description { get; set; }
        [XmlElement(ElementName = "broker_name")]
        public string Broker_name { get; set; }
        [XmlElement(ElementName = "broker_phone")]
        public string Broker_phone { get; set; }
        [XmlElement(ElementName = "broker_email")]
        public string Broker_email { get; set; }
        [XmlElement(ElementName = "broker_website")]
        public string Broker_website { get; set; }
        [XmlElement(ElementName = "broker_logo_url")]
        public string Broker_logo_url { get; set; }
        [XmlElement(ElementName = "broker_address")]
        public string Broker_address { get; set; }
        [XmlElement(ElementName = "broker_city")]
        public string Broker_city { get; set; }
        [XmlElement(ElementName = "broker_state")]
        public string Broker_state { get; set; }
        [XmlElement(ElementName = "broker_zip")]
        public string Broker_zip { get; set; }
        [XmlElement(ElementName = "home_fees")]
        public string Home_fees { get; set; }
        [XmlElement(ElementName = "images")]
        public Images Images { get; set; }
        [XmlElement(ElementName = "home_options")]
        public string Home_options { get; set; }
        [XmlElement(ElementName = "home_videos")]
        public string Home_videos { get; set; }
        [XmlElement(ElementName = "home_virtual_tours")]
        public string Home_virtual_tours { get; set; }
        [XmlAttribute(AttributeName = "is_active")]
        public string Is_active { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        public int RowIndex { get; set; }

        #region "For BDX Feed"
         [XmlIgnore]
        public string ListingType { get; set; }
         [XmlIgnore]
        public string HalfBedrooms { get; set; }
        #endregion

    }

    [XmlRoot(ElementName = "school")]
    public class School
    {
        [XmlElement(ElementName = "ncesid")]
        public string Ncesid { get; set; }
    }

    [XmlRoot(ElementName = "schools")]
    public class Schools
    {
        [XmlElement(ElementName = "school")]
        public List<School> School { get; set; }
    }

    [XmlRoot(ElementName = "communities")]
    public class Communities
    {
        public Communities() {
            Community = new List<Community>();
        }
        [XmlElement(ElementName = "community")]
        public List<Community> Community { get; set; }
        [XmlAttribute(AttributeName = "total")]
        public string Total { get; set; }
    }

    [XmlRoot(ElementName = "builder")]
    public class Builder
    {
        [XmlElement(ElementName = "number")]
        public string Number { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "name_reporting")]
        public string Name_reporting { get; set; }
        [XmlElement(ElementName = "phone")]
        public string Phone { get; set; }
        [XmlElement(ElementName = "email")]
        public string Email { get; set; }
        [XmlElement(ElementName = "leads_email")]
        public string Leads_email { get; set; }
        [XmlElement(ElementName = "dre_number")]
        public string Dre_number { get; set; }
        [XmlElement(ElementName = "website")]
        public string Website { get; set; }
        [XmlElement(ElementName = "logo_url")]
        public string Logo_url { get; set; }
        [XmlElement(ElementName = "address")]
        public string Address { get; set; }
        [XmlElement(ElementName = "city")]
        public string City { get; set; }
        [XmlElement(ElementName = "state")]
        public string State { get; set; }
        [XmlElement(ElementName = "zip")]
        public string Zip { get; set; }
        [XmlElement(ElementName = "communities")]
        public Communities Communities { get; set; }
        [XmlElement(ElementName = "homes")]
        public Homes Homes { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string BuilderId { get; set; }
        [XmlAttribute(AttributeName = "is_active")]
        public string Is_active { get; set; }
    }

    [XmlRoot(ElementName = "builders")]
    public class Builders
    {
        [XmlElement(ElementName = "builder")]
        public List<Builder> Builder { get; set; }
        [XmlAttribute(AttributeName = "total")]
        public string Total { get; set; }
    }

    [XmlRoot(ElementName = "root")]
    public class NewHomeListingRoot
    {
        [XmlElement(ElementName = "builders")]
        public Builders Builders { get; set; }
        [XmlAttribute(AttributeName = "created_on")]
        public string Created_on { get; set; }
    }

}

