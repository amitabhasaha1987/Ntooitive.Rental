using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using AutoMapper;
using Configuration;
using DataImportConsole.PropertySetter;
using Nest;
using Repositories.Interfaces.Admin.Users;
using Repositories.Interfaces.Community;
using Repositories.Interfaces.Downloader;
using Repositories.Interfaces.ElasticSearch;
using Repositories.Interfaces.ListHub;
using Repositories.Interfaces.NewHome;
using Repositories.Models.Admin.User;
using Repositories.Models.ListHub;
using Repositories.Models.NewHome;
using Repositories.Interfaces.Admin.Office;
using Repositories.Models.Classified;

namespace DataImportConsole
{

    public class Search : ElasticSearchConnection
    {
        public Lazy<ElasticClient> GetConnection()
        {
            return new Lazy<ElasticClient>(BuildConnection);
        }
    }
    class Program : ElasticSearchConnection
    {

        public static void Main(string[] args) //public static 
        {
            RunProcess();
            //SerachProcess();
        }

        private static void RunProcess()
        {
            if (!Initialize())
            {
                Console.WriteLine("Error!!! Unable to initialize.");
                Console.ReadKey(true);
                return;
            }

            var communityfilepath = ConfigurationManager.AppSettings["Community:Filepath"];



            var elasticSearchsettings = NinjectConfig.Get<IElasticSearchSettings>();

            var elasticSearchindicesforRental = NinjectConfig.Get<IElasticSearchIndices<Rental>>();
            var elasticSearchindicesforPurchase = NinjectConfig.Get<IElasticSearchIndices<Purchase>>();
            var elasticSearchindicesforNewHome = NinjectConfig.Get<IElasticSearchIndices<NewHome>>();

            var realestateService = NinjectConfig.Get<IListHub>();
            var newHomeFeedService = NinjectConfig.Get<INewHome>();
            var agentService = NinjectConfig.Get<IAgent>();
            var officeService = NinjectConfig.Get<IOffice>();
            var fetchService = NinjectConfig.Get<IFetcher>();
            var communities = NinjectConfig.Get<ICommunityProvider>();

            //if (!string.IsNullOrEmpty(communityfilepath))
            //{
            //    communities.Process(communityfilepath);
            //}


            bool ack = elasticSearchsettings.CreateSettings();

            var baseurl = ConfigurationManager.AppSettings["ListHub:Baseurl"];
            var username = ConfigurationManager.AppSettings["ListHub:Username"];
            var password = ConfigurationManager.AppSettings["ListHub:Password"];
            var channelId = ConfigurationManager.AppSettings["ListHub:Channelid"];
            var filename = ConfigurationManager.AppSettings["ListHub:Filename"];
            var saveas = ConfigurationManager.AppSettings["ListHub:Saveas"];


            #region Download & Process The File
            var previousUrl = fetchService.GetUrl();
            var currentUrl = fetchService.FetchViaWebClient(baseurl, username, password, channelId, filename, saveas);//@"C:\Users\Habib\Source\Repos\RealEstateVerticals\Ntooitive.Rental\DataImportConsole\DumpData\sduniontrib_12_28_2015_01_28_PM.xml"; //
            var setCurrentUrl = fetchService.SetUrl(currentUrl);

            #endregion

            #region ListHub

            #region Process Feed

            var newlistingDeserializer = new XmlSerializer(typeof(ListHubListingRoot));
            var newlistingreader = new StreamReader(currentUrl);
            var obj = newlistingDeserializer.Deserialize(newlistingreader);
            var newlisting = (ListHubListingRoot)obj;

            newlistingreader.Close();

            ProcessManger.SetLatLong<ListHubListing>(newlisting.Listing);
            ProcessManger.SetCommunities<ListHubListing>(newlisting.Listing);


            realestateService.ProcessFeed(newlisting.Listing);
            #endregion


            #region Create elastic object
            var rentListing = newlisting.Listing.Where(x => x.ListingCategory == "Rent").Select(Mapper.Map<Rental>).ToList();
            var purchaseListing = newlisting.Listing.Where(x => x.ListingCategory == "Purchase").Select(Mapper.Map<Purchase>).ToList();


            elasticSearchindicesforRental.CreateBulkIndex(rentListing);
            elasticSearchindicesforPurchase.CreateBulkIndex(purchaseListing);


            #endregion


            #region Delete Previous Data

            if (!string.IsNullOrEmpty(previousUrl))
            {
                var newMlsiDsforRent = newlisting.Listing.Where(x => x.ListingCategory == "Rent").Select(m => m.MlsNumber);
                var newMlsiDsforPurchase = newlisting.Listing.Where(x => x.ListingCategory == "Purchase").Select(m => m.MlsNumber);

                var previouslistingDeserializer = new XmlSerializer(typeof(ListHubListingRoot));

                var previouslistingreader = new StreamReader(previousUrl);
                var oldobj = previouslistingDeserializer.Deserialize(previouslistingreader);
                var oldListing = (ListHubListingRoot)oldobj;
                previouslistingreader.Close();


                var oldMlsiDsforRent = oldListing.Listing.Where(x => x.ListingCategory == "Rent").Select(m => m.MlsNumber);
                var oldMlsiDsforPurchase = oldListing.Listing.Where(x => x.ListingCategory == "Purchase").Select(m => m.MlsNumber);

                var propertynotlistedforRent = oldMlsiDsforRent.Except(newMlsiDsforRent);
                var propertynotlistedforPurchase = oldMlsiDsforPurchase.Except(newMlsiDsforPurchase);


                foreach (var mlsid in propertynotlistedforRent)
                {
                    realestateService.DeleteProperty("rent", mlsid, false);
                    elasticSearchindicesforRental.RemoveIndex(mlsid);
                }
                foreach (var mlsid in propertynotlistedforPurchase)
                {
                    realestateService.DeleteProperty("purchase", mlsid, false);
                    elasticSearchindicesforPurchase.RemoveIndex(mlsid);
                }
            }

            #endregion

            #endregion

            #region Agents
            var listofagents = new List<User>();


            foreach (var listing in newlisting.Listing)
            {
                var user = Mapper.Map<User>(listing.ListingParticipants.Participant);
                if (listing.Offices!= null)
                {
                    if (listing.Offices.Office!= null)
                    {
                        user.OfficeId = listing.Offices.Office.OfficeId;
                    }
                }
               
                //var hasAgent = agentService.GetAgentDetails(user.ParticipantId);
                var hasAgent = agentService.GetAgentDetailsByEmail(user.Email);
                
                if (hasAgent == null)
                {
                    if (listofagents.FirstOrDefault(m => m.Email == user.Email) == null)
                    {
                        listofagents.Add(user);
                    }
                }
            }

            realestateService.CreateIndex();


            //var countAgent = agentService.InsertBulkUsers(listofagents);
            foreach (var agent in listofagents)
            {
                var agentresult = agentService.UpdateAgentListHub(agent);
                if(agentresult == false)
                {
                    realestateService.UpdateForAgent(agent, "purchase");
                    realestateService.UpdateForAgent(agent, "rent");
                }
            }
            #endregion

            #region Office
            var listofoffice = new List<Office>();


            foreach (var listing in newlisting.Listing)
            {
                var offc = Mapper.Map<Office>(listing.Offices.Office);

                //var hasOffice = officeService.GetOfficeDetails(offc.OfficeId);
                var hasOffice = officeService.GetOfficeDetailsByName(offc.Name);
                if (hasOffice == null)
                {
                    if (listofoffice.FirstOrDefault(m => m.OfficeId == offc.OfficeId) == null)
                    {
                        listofoffice.Add(offc);
                    }
                }
            }

            realestateService.CreateIndex();


            var countOffice = officeService.InsertBulkOffices(listofoffice);
            #endregion

           

        }

        private static bool Initialize()
        {
            NinjectConfig.StartScheduler();
            AutoMapperConfiguration.Configure();
            return true;
        }




        private static void SerachProcess()
        {
            if (!Initialize())
            {
                Console.WriteLine("Error!!! Unable to initialize.");
                Console.ReadKey(true);
                return;
            }
            Search s = new Search();
            string searchText = "4 beds with colling system near newport beach and 2 parkings";
            // var ack = s.GetConnection().Value.DeleteIndex("ntooive_test").Acknowledged;



            var SimpleQuery_result = s.GetConnection().Value.Search<Rental>
                (
                    body => body.Query(query =>
                                        query.SimpleQueryString(qs =>
                                                                qs.Query(searchText)
                                                               )
                                      )
                );
            var Query_result = s.GetConnection().Value.Search<Rental>
                (
                    body => body.Query(query =>
                                        query.QueryString(qs =>
                                                            qs.Query(searchText)
                                                         )
                                       ).Take(10000)
                );

            var match_result = s.GetConnection().Value.Search<Purchase>
               (
                    body => body.Query(q =>
                                            q.Bool(bq =>
                                                    bq.Must(mq =>
                                                            mq.Match(x => x.OnField(m => m.Attributes).Query(searchText))
                                                            )
                                                       .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))

                                                  )
                                          )
                );

            //var ee = match_result.Documents.FirstOrDefault(m => m.MLSNumber == "NP15179864");
            var termQuery = Query<Rental>.Term(p => p.Attributes.First(), searchText);
            QueryContainer queryById = new TermQuery() { Field = "Attributes", Value = searchText };

            var matchAll_result = s.GetConnection().Value.Search<Rental>
                (
                //body=>body.Query(q=>q.MatchAll() && queryById)
                    body => body.TrackScores(true)
                        .Query(q =>
                                q.Filtered(f =>
                                             f.Query(fq =>
                                                        fq.Match(fqm =>
                                                                    fqm.OnField("attributes").Query(searchText)
                                                                )
                                                    )
                                          )
                               )
                        .Sort(sort => sort.OnField("_score").Descending())
                );
        }

    }
}
