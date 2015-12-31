namespace Repositories.Models.Admin.User
{
    public class UserDataTable
    {

        public bool isParticipantIdSearchable { get; set; }
        public bool isFirstNameSearchable { get; set; }
        public bool isLastNameSearchable { get; set; }
        public bool isEmailSearchable { get; set; }
        public bool isPrimaryContactPhoneSearchable { get; set; }
        public bool isOfficePhoneSearchable { get; set; }
        public bool isRolesSearchable { get; set; }

        public bool isParticipantIdSortable { get; set; }
        public bool isFirstNameSortable { get; set; }
        public bool isLastNameSortable { get; set; }
        public bool isEmailSortable { get; set; }
        public bool isPrimaryContactPhoneSortable { get; set; }
        public bool isOfficePhoneSortable { get; set; }
        public bool isRolesSortable { get; set; }


        public int sortColumnIndex { get; set; }
        public string sortDirection { get; set; }
    }
}