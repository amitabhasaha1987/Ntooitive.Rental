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
using Repositories.Models.NewHome;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace DataImportConsole.NewHomeProcess
{
    class NewHomeBaseFeedProcess
    {
        public static void RunProcess()
        {
            if (!Initialize())
            {
                Console.WriteLine("Error!!! Unable to initialize.");
                Console.ReadKey(true);
                return;
            }


            var elasticSearchindicesforNewHome = NinjectConfig.Get<IElasticSearchIndices<NewHome>>();

            var newHomeFeedService = NinjectConfig.Get<INewHome>();
            var agentService = NinjectConfig.Get<IAgent>();

            var communities = NinjectConfig.Get<ICommunityProvider>();
            var fetchService = NinjectConfig.Get<INewHomeDownloader>();



            var fullurl = ConfigurationManager.AppSettings["NewHome:Fullurl"].ToString();
            foreach (var url in fullurl.Split(','))
            {
                var baseURL = url;
                var filenames = baseURL.Split('/');
                var filename = filenames[filenames.Length - 1];
                //filename = baseURL.Split('/').LastOrDefault();
                var saveas = ConfigurationManager.AppSettings["NewHome:Saveas"];




                #region download feed file

                var previousUrl = fetchService.GetUrl();
                var currentUrl = fetchService.FetchViaWebClient(baseURL, null, null, null, filename, saveas);
                var setCurrentUrl = fetchService.SetUrl(currentUrl);

                var newhomedeserializer = new XmlSerializer(typeof(NewHomeListingRoot));
                var newhomereader = new StreamReader(currentUrl);
                var newhomeobj = newhomedeserializer.Deserialize(newhomereader);
                var newhomeListing = (NewHomeListingRoot)newhomeobj;
                #endregion

                #region "ProcessFeed"

                ProcessFeed(newhomeListing);
                #endregion


                newhomereader.Close();

            }
        }

        public static void ProcessFeed(NewHomeListingRoot newhomeListing)
        {
            var elasticSearchindicesforNewHome = NinjectConfig.Get<IElasticSearchIndices<NewHome>>();

            var newHomeFeedService = NinjectConfig.Get<INewHome>();
            var agentService = NinjectConfig.Get<IAgent>();

            var communities = NinjectConfig.Get<ICommunityProvider>();
            var fetchService = NinjectConfig.Get<INewHomeDownloader>();

            List<Repositories.Models.Community.Communities> lstNewHomeCommunity =
                new List<Repositories.Models.Community.Communities>();
            var plans = new List<Plan>();

            foreach (var item in newhomeListing.Builders.Builder)
            {
                //Initialize the user object
                Repositories.Models.Admin.User.User user = new Repositories.Models.Admin.User.User()
                {
                    ParticipantId = Utility.UtilityClass.GetUniqueKey(),
                    BuilderId = item.Number,
                    FirstName = item.Name,
                    PrimaryContactPhone = item.Phone,
                    Email = item.Email,
                    dre_number = item.Dre_number,
                    WebsiteURL = item.Website,
                    logo_url = item.Logo_url,
                    address = item.Address,
                    city = item.City,
                    state = item.State,
                    zip = item.Zip,
                    Roles = new string[1] { Convert.ToString(Utility.Roles.Builder) }
                };

                //Process Community and Plans
                ProcessManger.SetLatLong<Repositories.Models.NewHome.Community>(item.Communities.Community);
                foreach (var community in item.Communities.Community)
                {
                    if (lstNewHomeCommunity.Where(m => m.CommunityName == community.Name).FirstOrDefault() == null)
                    {
                        if (!string.IsNullOrEmpty(community.Name))
                        {
                            lstNewHomeCommunity.Add(new Repositories.Models.Community.Communities
                            {

                                CommunityId = string.IsNullOrEmpty(community.CommunityId) ? Utility.UtilityClass.GetUniqueKey() : community.CommunityId,
                                CommunityName = community.Name,
                                Number = string.IsNullOrEmpty(community.Number) ? Utility.UtilityClass.GetUniqueKey() : community.Number,
                                WebSite = community.Website,
                                Address = community.Address,
                                City = community.City,
                                State = community.State,
                                Zip1 = community.Zip
                            });
                        }

                    }
                    foreach (var plan in community.Plans.Plan)
                    {
                        //if (plan.Homes.Home.Count > 0)
                        //{
                        plan.BuilderNumber = item.Number;
                        plan.BuilderName = item.Name_reporting;
                        plan.BuilderEmail = item.Email;
                        plan.Builder_dre_number = item.Dre_number;
                        plan.CommunityName = community.Name;
                        plan.CommunityNumber = community.Number;
                        plan.CommunityWebsite = community.Website;
                        plan.Communityprice_low = string.IsNullOrEmpty(community.Price_low)
                            ? 0
                            : Convert.ToDouble(community.Price_low);
                        plan.Communityprice_high = string.IsNullOrEmpty(community.Price_high)
                            ? 0
                            : Convert.ToDouble(community.Price_high);
                        plan.Communitysqft_high = string.IsNullOrEmpty(community.Sqft_high)
                            ? 0
                            : Convert.ToDouble(community.Sqft_high);
                        plan.Communitysqft_low = string.IsNullOrEmpty(community.Sqft_low)
                            ? 0
                            : Convert.ToDouble(community.Sqft_low);
                        plan.Communityaddress = community.Address;
                        plan.Communitycity = community.City;
                        plan.Communitystate = community.State;
                        plan.Communityzip = community.Zip;
                        plan.Latitude = community.Latitude;
                        plan.Longitude = community.Longitude;
                        plans.Add(plan);
                        //}

                    }
                }

                #region Insert in mongodb
                agentService.UpSertFromFeed(user);
                newHomeFeedService.InsertFromFeed(plans);
                communities.InsertFromFeed(lstNewHomeCommunity);
                newHomeFeedService.CreateIndex();

                #endregion

                #region Process and Insert in elastic search db

                var elasticsearchNewHome = plans.Select(Mapper.Map<Repositories.Models.ListHub.NewHome>).ToList();
                elasticSearchindicesforNewHome.CreateBulkIndex(elasticsearchNewHome);

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
