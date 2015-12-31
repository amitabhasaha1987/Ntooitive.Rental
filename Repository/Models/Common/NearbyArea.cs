using System.ComponentModel.DataAnnotations;

namespace Repositories.Models.Common
{
    public class NearbyArea
    {
        public string _id { get; set; }

        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double AvgDistance { get; set; }
        public bool IsSelected { get; set; }
    }
}
