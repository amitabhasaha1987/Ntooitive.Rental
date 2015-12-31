using Repositories.Interfaces.DataTable;
using Repositories.Models.Admin.Office;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces.Admin.Office
{
    public interface IOffice : IDataTable<Repositories.Models.ListHub.Office, OfficeDataTable>
    {
        bool InsertBulkOffices(List<Repositories.Models.ListHub.Office> users);

        bool UpdateOffice(Repositories.Models.ListHub.Office office);

        bool DeleteOffice(string uniquid, bool isDeleted);
        Repositories.Models.ListHub.Office GetOfficeDetails(string officeId);
        Repositories.Models.ListHub.Office GetOfficeDetailsByName(string officeName);
        IEnumerable<State> GetStateList();
        IEnumerable<Repositories.Models.Admin.Office.Cities> GetCityList(string StateName);
        IEnumerable<Repositories.Models.Admin.Office.Cities> GetCityList();
        IEnumerable<Repositories.Models.Admin.Office.ZipCode> GetZipCodeList(string City);
        IEnumerable<Repositories.Models.Admin.Office.ZipCode> GetZipCodeList();
        IEnumerable<Repositories.Models.Admin.Office.StreetAddress> GetStreetAddressList(string City);
        IEnumerable<Repositories.Models.Admin.Office.StreetAddress> GetStreetAddressList();
        bool UploadProfileImage(string uniquid, string url);
        List<Repositories.Models.ListHub.Office> GetOffice();
        bool Validation(Repositories.Models.ListHub.Office offc);
        string GetPreviousImageUrl(string uniqueId);
    }
}
