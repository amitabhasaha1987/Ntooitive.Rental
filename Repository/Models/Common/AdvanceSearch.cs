using System.Collections.Generic;

namespace Repositories.Models.Common
{
    public class AdvanceSearch
    {
        public string MinPrice { get; set; }
        public string MaxPrice { get; set; }
        public string NoOfBeds { get; set; }
        public string NoOfBathroom { get; set; }
        public string Size { get; set; }
        public string LotSize { get; set; }
        public string HomeAge { get; set; }
        public string PropertyType { get; set; }
        public string Location { get; set; }
        public double NearByDistance { get; set; }
        public bool RadioByMiles { get; set; }
        public bool RadioNearByAreas { get; set; }
        public List<NearbyArea> SelectedNearByAreas { get; set; }
        public List<string> SelectedProperty { get; set; }
        public List<string> SelectedSubProperty { get; set; }
        public string SortBy { get; set; }
        public string MlsNumber { get; set; }
        public string KeyWards { get; set; }
        public string SearchTerm { get; set; }
        public string Suffix { get; set; }
        public string AgentName { get; set; }
    }
}
