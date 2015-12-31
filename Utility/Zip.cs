using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class Zip
    {
        public static string DecompressZip(string sourcePath, string destinationPath)//,string fileType
        {
            string _fullName = string.Empty;
            using (ZipArchive archive = ZipFile.OpenRead(sourcePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    int index = entry.FullName.LastIndexOf('/');
                    _fullName = entry.FullName;
                    if (index != -1)
                    {
                        _fullName = entry.FullName.Substring(++index);
                    }
                    entry.ExtractToFile(Path.Combine(destinationPath, _fullName));
                    return _fullName;
                }
            }

            return _fullName;
        }
    }
}
