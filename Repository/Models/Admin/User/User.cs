using System;
using Repositories.Models.ListHub;

namespace Repositories.Models.Admin.User
{
    public class User : Participant
    {
        public string BuilderId { get; set; }
        public string dre_number { get; set; }
        public string logo_url { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }

        public bool IsActive { get; set; }
        public bool IsEmailSend { get; set; }
        public DateTime ActivateDate { get; set; }
        public DateTime DeactivateDate { get; set; }
        public DateTime InitiateDate { get; set; }
        public bool IsCertified { get; set; }
        public bool IsFeatured { get; set; }
        public string Password { get; set; }
        public string[] Roles { get; set; }
        public bool IsAllRentPropertyFeatured { get; set; }
        public bool IsAllPurchasePropertyFeatured { get; set; }
        public bool IsUpdatedByAgent { get; set; }
        //public string  OfficeId{ get; set; }


    }
}
