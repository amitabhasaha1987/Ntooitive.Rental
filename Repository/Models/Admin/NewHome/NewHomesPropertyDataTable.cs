using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Models.Admin.NewHome
{
   public class NewHomesPropertyDataTable
    {
        public bool isImageSearchable { get; set; }
        public bool isBuilderNoSearchable { get; set; }
        public bool isBuilderNameSearchable { get; set; }
        public bool isPriceHighSearchable { get; set; }
        public bool isPriceLowSearchable { get; set; }
        public bool isSqFtHighSearchable { get; set; }
        public bool isSqFtLowSearchable { get; set; }
        public bool isStatusSearchable { get; set; }
        public bool isAddressSearchable { get; set; }

        public bool isImageSortable { get; set; }
        public bool isBuilderNoSortable { get; set; }
        public bool isBuilderNameSortable { get; set; }
        public bool isPriceHighSortable { get; set; }
        public bool isPriceLowSortable { get; set; }
        public bool isSqFtHighSortable { get; set; }
        public bool isSqFtLowSortable { get; set; }
        public bool isStatusSortable { get; set; }
        public bool isAddressSortable { get; set; }


        public int sortColumnIndex { get; set; }
        public string sortDirection { get; set; }
    }
}
