using Repositories.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminInterface.Models.NewHome
{
    public class NewHomeViewModel
    {
        public NewHomeViewModel()
        {
            this.PlanModelViewList = new HashSet<HomePlan>();
        }
        public string BuilderNumber { get; set; }
        public string BrandName { get; set; }
        public ManagePropertyViewModel ExtProperties { get; set; }
        public string ListingDescription { get; set; }
        public string[] Photos { get; set; }
        public virtual ICollection<HomePlan> PlanModelViewList { get; set; }

        public void CreateNewPlanModels(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                PlanModelViewList.Add(new HomePlan());
            }
        }
    }
}