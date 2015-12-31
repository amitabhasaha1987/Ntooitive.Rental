using Repositories.Interfaces.Downloader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Implementation.Downloader
{
    public class FileDownloader : IDownloader
    {
        public string DownloadFile(string remoteFtpPath, string localDestinationPath)
        {
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(remoteFtpPath);
            ftpRequest.Credentials = new NetworkCredential("ntootive", "ADITFILES1");
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            ftpRequest.KeepAlive = false;
            FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            StreamReader sr = new StreamReader(ftpResponse.GetResponseStream());
            string line = sr.ReadLine();

            string day = DateTime.Now.Day.ToString().PadLeft(2, '0');
            string month = DateTime.Now.Month.ToString();
            string year = DateTime.Now.Year.ToString();

            string yearStrngVal = month + day + year;
            List<int> lstTimeStamp = new List<int>();
            string filename = "proplinersdut";
            while (!string.IsNullOrEmpty(line))
            {
                if (line.Contains(yearStrngVal) && line.Contains(filename))
                {
                    string a = line;
                    line = sr.ReadLine();
                    string[] fileName = a.Split(' ');
                    int count = fileName.Count() - 1;
                    string[] var = fileName[count].Split('_');
                    string myTime = System.Text.RegularExpressions.Regex.Replace(var[1], @"\D", "");
                    lstTimeStamp.Add(Convert.ToInt16(myTime));
                }
                else
                {
                    line = sr.ReadLine();
                }
            }
            ftpResponse.Close();
            sr.Close();
            string fName = string.Empty;
            if (lstTimeStamp != null && lstTimeStamp.Count > 0)
            {
            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential("ntootive", "ADITFILES1");
                fName = filename + yearStrngVal + "_" + lstTimeStamp.Max().ToString().PadLeft(4, '0') + ".xml";
                byte[] fileData = request.DownloadData(remoteFtpPath + fName);

                using (FileStream file = File.Create(localDestinationPath + fName))
                {
                    file.Write(fileData, 0, fileData.Length);
                    file.Close();
                }
            }
            }
            return fName;
        }

        public string DownloadFileBDXFeed(string remoteFtpPath, string localDestinationPath)
        {
            try
            {
                //Your code

                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(remoteFtpPath);
                ftpRequest.Credentials = new NetworkCredential("sandiegouniontribune", "hkaj34Cej");
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftpRequest.KeepAlive = false;
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                StreamReader sr = new StreamReader(ftpResponse.GetResponseStream());
                string line = sr.ReadLine();

                string filename = "NHS";//NHS_I9125_12042015
                
                while (!string.IsNullOrEmpty(line))
                {
                    if (line.Contains(filename))
                    {
                        filename = line.Split(' ').LastOrDefault();
                        break;
                    }
                    else
                    {
                        line = sr.ReadLine();
                    }
                }
                ftpResponse.Close();
                sr.Close();
                string fName = string.Empty;
                using (WebClient request = new WebClient())
                {
                    request.Credentials = new NetworkCredential("sandiegouniontribune", "hkaj34Cej");
                    fName = filename;//+ ".zip";
                    byte[] fileData = request.DownloadData(remoteFtpPath + "/"+ fName);
                    Console.WriteLine("Local Destination path " + localDestinationPath + "/" + fName);

                    if (!File.Exists(localDestinationPath + "/" + fName))
                    {
                        using (FileStream file = File.Create(localDestinationPath + "/" + fName))
                        {
                            file.Write(fileData, 0, fileData.Length);
                            file.Close();
                            
                        }
                    }
                    else
                    {
                        fName = "";
                    }

                    
                }
                return fName;
        }
            catch (WebException e)
            {
                String status = ((FtpWebResponse)e.Response).StatusDescription;
                return status;
            }
        }
    }
}
