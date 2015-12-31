using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class DateTimeExtension
    {
        public static string GetTime(TimeSpan time)
        {
            if(time.Hours > 12)
            {
                return (time.Hours - 12).ToString() + " PM";
            }
            else
            {
                return time.Hours.ToString() + " AM";
            }
        }
    }
}
