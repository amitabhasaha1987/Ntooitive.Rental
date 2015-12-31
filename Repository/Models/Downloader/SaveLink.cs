using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Models.Base;

namespace Repositories.Models.Downloader
{
   public class SaveLink : BaseEntity
    {
       public string Url { get; set; }
    }
}
