using Repositories.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Models.ListHub
{
    public class Purchase : Repositories.Models.Common.PropertyListing
    {
        public bool HasAddress { get; set; }
        public string ClassifiedExpireDate { get; set; }
    }
}
