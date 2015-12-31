using System.Collections.Generic;
using PagedList;
using Repositories.Models.Common;

namespace UserInterface.Models
{
    public class PropertyListingViewModel
    {
        public AdvanceSearch advSearch { get; set; }
        public List<PropertyTypeCheckBox> PropertyType { get; set; }
        public List<SubPropertyTypeCheckBox> SubPropertyType { get; set; }
        public string Command { get; set; }
        public string SearchValue { get; set; }
        public List<NearbyArea> NearbyAreas { get; set; }
        public StaticPagedList<PropertyListing> PropertyListings { get; set; }
        public StaticPagedList<PropertyListing> FeaturedPropertyListings { get; set; }
    }
}