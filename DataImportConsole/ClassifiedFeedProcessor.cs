using AutoMapper;
using Configuration;
using DataImportConsole.PropertySetter;
using Repositories.Interfaces.Admin.Office;
using Repositories.Interfaces.Admin.Users;
using Repositories.Interfaces.Community;
using Repositories.Interfaces.Downloader;
using Repositories.Interfaces.ElasticSearch;
using Repositories.Interfaces.ListHub;
using Repositories.Interfaces.NewHome;
using Repositories.Models.Classified;
using Repositories.Models.ListHub;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataImportConsole
{
    class ClassifiedFeedProcessor
    {


        void Main(string[] args)//public static 
        {
            RunProcess();

        }
        public static void RunProcess()
        {
            if (!Initialize())
            {
                Console.WriteLine("Error!!! Unable to initialize.");
                Console.ReadKey(true);
                return;
            }


            var elasticSearchsettings = NinjectConfig.Get<IElasticSearchSettings>();

            var elasticSearchindicesforRental = NinjectConfig.Get<IElasticSearchIndices<Rental>>();
            var elasticSearchindicesforPurchase = NinjectConfig.Get<IElasticSearchIndices<Purchase>>();
            var elasticSearchindicesforNewHome = NinjectConfig.Get<IElasticSearchIndices<NewHome>>();

            var realestateService = NinjectConfig.Get<IListHub>();
            var newHomeFeedService = NinjectConfig.Get<INewHome>();
            var agentService = NinjectConfig.Get<IAgent>();
            var officeService = NinjectConfig.Get<IOffice>();
            var fetchService = NinjectConfig.Get<IFetcher>();


            #region "Added for ClassifiedFeed"

            #region "Download & Process The File"

            var folderPath = ConfigurationManager.AppSettings["path"];
            var remoteFtpPath = ConfigurationManager.AppSettings["FTPpath"];

            var downloader = NinjectConfig.Get<IDownloader>();
            string fileName = downloader.DownloadFile(remoteFtpPath, folderPath); //"proplinersdut12042015_0501.xml"; 
            if (!string.IsNullOrEmpty(fileName))
            {
                string localDestinationPath = folderPath + fileName;

                var newlistingDeserializerClassifiedFeed = new XmlSerializer(typeof(ClassifiedListingRoot));
                var newlistingreaderClassifiedFeed = new StreamReader(localDestinationPath);
                var objClassifiedFeed = newlistingDeserializerClassifiedFeed.Deserialize(newlistingreaderClassifiedFeed);
                var classifiedFeed = (ClassifiedListingRoot)objClassifiedFeed;

                newlistingreaderClassifiedFeed.Close();


                var offcClassifiedFeed = Mapper.Map<List<ListHubListing>>(classifiedFeed.Properties);


            #endregion

                ProcessManger.SetLatLong(offcClassifiedFeed);
                ProcessManger.SetCommunities(offcClassifiedFeed);

                #region "ProcessFeed Mongo Insert"
                realestateService.ProcessFeed(offcClassifiedFeed);

                #endregion

                #region "Create elastic object"
                var rentListingClassifiedFeed = offcClassifiedFeed.Where(x => x.ListingCategory == "Rent").Select(Mapper.Map<Rental>).ToList();
                var purchaseListingClassifiedFeed = offcClassifiedFeed.Where(x => x.ListingCategory == "Purchase").Select(Mapper.Map<Purchase>).ToList();

                elasticSearchindicesforRental.CreateBulkIndex(rentListingClassifiedFeed);
                elasticSearchindicesforPurchase.CreateBulkIndex(purchaseListingClassifiedFeed);

                #endregion

            #endregion
            }
        }

        private static bool Initialize()
        {
            NinjectConfig.StartScheduler();
            AutoMapperConfiguration.Configure();
            return true;
        }
    }
}
