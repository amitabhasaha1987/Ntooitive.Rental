using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Utility
{
    public class ImageDelete
    {
        public void DeleteImage(string imageurl)
        {
            if (!string.IsNullOrEmpty(imageurl))
            {
                string[] file = imageurl.Split('/');
                string fileName = "/" + file[3] + "/" + file[4] + "/" + file[5];
                var fullpath = System.Web.HttpContext.Current.Server.MapPath(fileName);
                if (Directory.Exists(Path.GetDirectoryName(fullpath)))
                {
                    System.IO.File.Delete(fullpath);
                }
            }
        }
        public void DeleteImageWithoutUnique(string imageurl)
        {
            if (!string.IsNullOrEmpty(imageurl))
            {
                string[] file = imageurl.Split('/');
                string fileName = "/" + file[3] + "/" + file[4];
                var fullpath = System.Web.HttpContext.Current.Server.MapPath(fileName);
                if (Directory.Exists(Path.GetDirectoryName(fullpath)))
                {
                    System.IO.File.Delete(fullpath);
                }
            }
        }
    }
}
