using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Models.Admin.Office
{
    public class OfficeDataTable
    {
        public bool isOfficeIdSearchable { get; set; }
        public bool isNameSearchable { get; set; }
        public bool isCorporateNameSearchable { get; set; }
        public bool isBrokerIdSearchable { get; set; }
        public bool isPhoneNumberSearchable { get; set; }
        public bool isOfficeEmailSearchable { get; set; }
        public bool isWebsiteSearchable { get; set; }

        public bool isOfficeIdSortable { get; set; }
        public bool isNameSortable { get; set; }
        public bool isCorporateNameSortable { get; set; }
        public bool isBrokerIdSortable { get; set; }
        public bool isPhoneNumberSortable { get; set; }
        public bool isOfficeEmailSortable { get; set; }
        public bool isWebsiteSortable { get; set; }

        public int sortColumnIndex { get; set; }
        public string sortDirection { get; set; }
    }
}
