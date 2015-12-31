using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces.Downloader
{
    public interface INewHomeDownloader
    {

        string FetchViaWebClient(string baseurl, string username, string password, string channelID,
            string filename, string saveAs);

        bool SetUrl(string fileName);
        string GetUrl();
    }
}
