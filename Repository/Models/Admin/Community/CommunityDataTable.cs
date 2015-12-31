using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Models.Admin.Community
{
    public class CommunityDataTable
    {
        public bool isIdSearchable { get; set; }
        public bool isNameSearchable { get; set; }
        public bool isNumberSearchable { get; set; }
        public bool isAddressSearchable { get; set; }
        public bool isCitySearchable { get; set; }
        public bool isEmailSearchable { get; set; }
        public bool isPhoneSearchable { get; set; }

        public bool isIdSortable { get; set; }
        public bool isNameSortable { get; set; }
        public bool isNumberSortable { get; set; }
        public bool isAddressSortable { get; set; }
        public bool isCitySortable { get; set; }
        public bool isEmailSortable { get; set; }
        public bool isPhoneSortable { get; set; }


        public int sortColumnIndex { get; set; }
        public string sortDirection { get; set; }
    }
}
