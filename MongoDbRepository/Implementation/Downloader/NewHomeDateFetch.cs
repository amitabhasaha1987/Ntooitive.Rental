using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using Repositories.Interfaces.Downloader;
using Repositories.Models.Downloader;
using Utility;

namespace Core.Implementation.Downloader
{
    public class NewHomeDateFetch : Repository<SaveLink>, INewHomeDownloader
    {
        private new const string CollectionName = "";

        public NewHomeDateFetch()
            : base(CollectionName)
        {

        }
        public NewHomeDateFetch(string collectionName)
            : base(collectionName)
        {
        }

        public NewHomeDateFetch(IMongoDatabase database, string collectionName)
            : base(database, collectionName)
        {
        }

        public NewHomeDateFetch(string connectionString, string databaseName, string collectionName)
            : base(connectionString, databaseName, collectionName)
        {
        }

        public NewHomeDateFetch(string connectionString, string databaseName)
            : base(connectionString, databaseName)
        {
        }

        public string FetchViaWebClient(string baseurl, string username, string password, string channelID, string filename,
            string saveAs)
        {
            var fileNames = filename.Split('.'); //+ "_" + 
            var fileName = string.Empty;
            foreach (var item in fileNames)
            {
                if (item.ToLower() != "xml")
                    fileName = string.IsNullOrEmpty(fileName) ? item : fileName + "." + item;
            }
            fileName = fileName + "_" + DateTime.Now.ToString("MM_dd_yyyy_hh_mm_tt");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseurl);
            request.Method = "GET";

            Stream responseStream = request.GetResponse().GetResponseStream();
            var tmpsaveAs = saveAs + fileName + ".xml";

            FileStream fileStream = File.Create(tmpsaveAs);

            byte[] buffer = new byte[32 * 1024];
            int indx = 0;

            Console.WriteLine("****** Start Downloading Feed ******");
            Console.WriteLine(Environment.NewLine);
            while ((indx = responseStream.Read(buffer, 0, buffer.Length)) > 0)
            {

                Console.Write(".");
                fileStream.Write(buffer, 0, indx);
                indx++;
            }
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("****** End Downloading Feed ******");
            fileStream.Flush();
            fileStream.Close();
            responseStream.Close();

            fileName = saveAs + fileName + ".xml";





            return fileName;
        }

        public bool SetUrl(string fileName)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomeArchiveLink);

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
                base.CollectionName = Convert.ToString(DbCollections.NewHomeArchiveLink);
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
