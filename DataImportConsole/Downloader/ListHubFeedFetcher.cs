using System;
using System.IO;
using System.Net;
using Utility;

namespace DataImportConsole.Downloader
{
    public class ListHubFeedFetcher
    {
        public static string  DownloadPickupViaWebClient(string baseurl,string username, string password, string channelID, string filename, string saveAs)
        {
            using (WebClient client = new WebClient())
            {
                var fileName = channelID + "_" + DateTime.Now.ToString("MM_dd_yyyy_hh_mm");

                string url = baseurl + channelID + "/" + filename;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "HEAD";
                request.Credentials = new NetworkCredential(username, password);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // you can check headers before continuing:
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return "";
                }
               
                // now perform GET request:
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential(username, password);
                request.Method = "GET";

                Stream responseStream = request.GetResponse().GetResponseStream();
                var tmpsaveAs = saveAs + fileName + ".gz";

                FileStream fileStream = File.Create(tmpsaveAs);

                byte[] buffer = new byte[32 * 1024];
                int indx = 0;

                Console.WriteLine("****** Start Downloading Feed ******");
                Console.WriteLine(Environment.NewLine);
                while ((indx = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {

                    Console.Write(".");
                    fileStream.Write(buffer, 0, indx);
                }
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("****** End Downloading Feed ******");
                fileStream.Flush();
                fileStream.Close();
                responseStream.Close();


                byte[] file = File.ReadAllBytes(tmpsaveAs);

                Console.WriteLine("****** Start Decompress Feed ******");
                Console.WriteLine(Environment.NewLine);
                byte[] decompressed = Gzip.Decompress(file);
               
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("****** End Decompress Feed ******");

                filename = saveAs + channelID + "_" + DateTime.Now.ToString("MM_dd_yyyy_hh_mm") +".xml";
                File.WriteAllBytes(filename, decompressed); // Requires System.IO

                return filename;
            }
        }
    }
}
