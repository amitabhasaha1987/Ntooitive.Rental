using System;
using System.Collections.Generic;
using Repositories.Models.Base;
using Repositories.Models.ListHub;

namespace Repositories.Models.UserContact
{
    public class UserContactAndFindDetails : BaseEntity
    {
        public string UserName { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string MlsNumber { get; set; }
        public List<Object> PreferenceList { get; set; }
    }
}
