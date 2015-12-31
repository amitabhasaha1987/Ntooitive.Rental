
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace Repositories.Models.Classified
{
	[XmlRoot(ElementName="Images")]
	public class Images {
		[XmlElement(ElementName="Image1")]
		public string Image1 { get; set; }
		[XmlElement(ElementName="Image2")]
		public string Image2 { get; set; }
		[XmlElement(ElementName="Image3")]
		public string Image3 { get; set; }
		[XmlElement(ElementName="Image4")]
		public string Image4 { get; set; }
		[XmlElement(ElementName="Image5")]
		public string Image5 { get; set; }
		[XmlElement(ElementName="Image6")]
		public string Image6 { get; set; }
		[XmlElement(ElementName="Image7")]
		public string Image7 { get; set; }
		[XmlElement(ElementName="Image8")]
		public string Image8 { get; set; }
		[XmlElement(ElementName="Image9")]
		public string Image9 { get; set; }
		[XmlElement(ElementName="Image10")]
		public string Image10 { get; set; }
	}

	[XmlRoot(ElementName="Property")]
	public class Property {
		[XmlElement(ElementName="AdId")]
		public string AdId { get; set; }
		[XmlElement(ElementName="Address1")]
		public string Address1 { get; set; }
		[XmlElement(ElementName="City")]
		public string City { get; set; }
		[XmlElement(ElementName="State")]
		public string State { get; set; }
		[XmlElement(ElementName="Zip")]
		public string Zip { get; set; }
		[XmlElement(ElementName="Community")]
		public string Community { get; set; }
		[XmlElement(ElementName="Country")]
		public string Country { get; set; }
		[XmlElement(ElementName="ContactEmail")]
		public string ContactEmail { get; set; }
		[XmlElement(ElementName="PostDate")]
		public string PostDate { get; set; }
		[XmlElement(ElementName="ExpireDate")]
		public string ExpireDate { get; set; }
		[XmlElement(ElementName="Description")]
		public string Description { get; set; }
		[XmlElement(ElementName="MLSNumber")]
		public string MLSNumber { get; set; }
		[XmlElement(ElementName="AvailableDate")]
		public string AvailableDate { get; set; }
		[XmlElement(ElementName="HiddenAdress")]
		public string HiddenAdress { get; set; }
		[XmlElement(ElementName="SqFt")]
		public string SqFt { get; set; }
		[XmlElement(ElementName="PropertyType")]
		public string PropertyType { get; set; }
		[XmlElement(ElementName="Terms")]
		public string Terms { get; set; }
		[XmlElement(ElementName="Minprice")]
		public string Minprice { get; set; }
		[XmlElement(ElementName="Bathroomnumber")]
		public string Bathroomnumber { get; set; }
		[XmlElement(ElementName="Bedroomnumber")]
		public string Bedroomnumber { get; set; }
		[XmlElement(ElementName="UpsellFeaturedAd")]
		public string UpsellFeaturedAd { get; set; }
		[XmlElement(ElementName="UpsellSpotlightAd")]
		public string UpsellSpotlightAd { get; set; }
		[XmlElement(ElementName="Images")]
        public Images Images { get; set; }
        public bool HasAddress { get; set; }
	}

	[XmlRoot(ElementName="Properties")]
	public class Properties {
		[XmlElement(ElementName="Property")]
		public Property Property { get; set; }

	}

	[XmlRoot(ElementName="xml")]
    public class ClassifiedListingRoot
    {
		[XmlElement(ElementName="Properties")]
		public List<Properties> Properties { get; set; }
	}

}
