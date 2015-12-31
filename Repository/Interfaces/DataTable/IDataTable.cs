using System.Collections.Generic;
using Repositories.Models.DataTable;

namespace Repositories.Interfaces.DataTable
{
   public interface IDataTable<T,S>
   {
       List<T> GetDataSet(string userEmail, JQueryDataTableParamModel dataTableParamModel, S serachCriteria, out long filteredCount,string type = "");
       long GetTotalCount(string userEmail, string type = "");
   }
}
