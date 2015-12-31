using System;
using System.IO;
using System.Net;
using Core.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using Repositories.Interfaces.Downloader;
using Repositories.Models.Downloader;
using Utility;

namespace Core.Implementation.Downloader
{
    public class ListHubDataFetch : Repository<SaveLink>, IFetcher
    {
        private readonly IProcesser _processer;
        private new const string CollectionName = "";

        public ListHubDataFetch()
            : base(CollectionName)
        {
        }

        public ListHubDataFetch(IMongoDatabase database, IProcesser processer)
            : base(database, CollectionName)
        {
            _processer = processer;

        }

        public ListHubDataFetch(string connectionString, string databaseName)
            : base(connectionString, databaseName, CollectionName)
        {
        }
        public string FetchViaWebClient(string baseurl, string username, string password, string channelID,
            string filename, string saveAs)
        {
            using (WebClient client = new WebClient())
            {
                var fileName = channelID + "_" + DateTime.Now.ToString("MM_dd_yyyy_hh_mm_tt");

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

                fileName = saveAs + fileName + ".xml";
                _processer.ProcessGz(tmpsaveAs, fileName);

                return fileName;

            }
        }

        public bool SetUrl(string fileName)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.ArchiveLink);

                var update = Builders<SaveLink>.Update
                    .Set("Url", fileName);

                var results = GetCollection().UpdateOneAsync(new BsonDocument(), update, new UpdateOptions()
                {
                    IsUpsert = true
                }).Result;

                return results.ModifiedCount > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string GetUrl()
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.ArchiveLink);
                var aggregate =
                     GetCollection().Find(new BsonDocument()).FirstOrDefaultAsync().Result;
                return aggregate != null ? aggregate.Url : string.Empty;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
