using System.Collections.Generic;
using Repositories.Interfaces.DataTable;
using Repositories.Models.Admin.NewHome;
using Repositories.Models.NewHome;
using Repositories.Models.ViewModel;

namespace Repositories.Interfaces.Admin.NewHome
{
    public interface INewHomes : IDataTable<Plan, NewHomesPropertyDataTable>
    {
        List<Plan> GetPlans(string builderId);
    }
}