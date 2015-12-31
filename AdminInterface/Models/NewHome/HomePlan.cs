using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminInterface.Models.NewHome
{
    public class HomePlan
    {
        //public int PlanId { get; set; }
        public string PlanNumber { get; set; }
        public string PlanName { get; set; }
        public string ExcludesLand { get; set; }
        public int BasePrice { get; set; }
        public double BaseSqft { get; set; }
        public string Stories { get; set; }
        public int Baths { get; set; }
        public int HalfBaths { get; set; }
        public string MasterBedLocation { get; set; }
        public int Bedrooms { get; set; }
        public string Entry { get; set; }
        public int Garage { get; set; }
        public string Basement { get; set; }
        public string MarketingHeadline { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
        public string ElevationImageText { get; set; }
        public string VirtualTour { get; set; }
        public string PlanViewer { get; set; }
        public string Type { get; set; }
        public Boolean DeletePlan { get; set; }
        public string BathsUp { get; set; }
        //public bool IsSelected { get; set; }
    }
}