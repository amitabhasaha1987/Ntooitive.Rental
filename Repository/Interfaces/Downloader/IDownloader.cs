using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces.Downloader
{
    public interface IDownloader
    {
        string DownloadFile(string remoteFtpPath, string localDestinationPath);

        string DownloadFileBDXFeed(string remoteFtpPath, string localDestinationPath);
    }
}
