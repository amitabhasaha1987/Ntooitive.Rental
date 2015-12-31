using AutoMapper;
using Configuration;
using DataImportConsole.PropertySetter;
using Repositories.Interfaces.Admin.Users;
using Repositories.Interfaces.Community;
using Repositories.Interfaces.Downloader;
using Repositories.Interfaces.ElasticSearch;
using Repositories.Interfaces.NewHome;
using Repositories.Models.ListHub;
using Repositories.Models.NewHome;
using Repositories.Models.NewHomeBDXFeed;
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
    public class BDXFeedProcess
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



            // var fullurl = ConfigurationManager.AppSettings["NewHome:Fullurl"].ToString();
            //foreach (var url in fullurl.Split(','))
            //{
            //var baseURL = url;
            //var filename = baseURL.Split('/')[1];
            //var saveas = ConfigurationManager.AppSettings["NewHome:Saveas"];




            #region download feed file and Unzip

            //var previousUrl = fetchService.GetUrl();
            //var currentUrl = fetchService.FetchViaWebClient(fullurl, null, null, null, filename, saveas);

            //var setCurrentUrl = fetchService.SetUrl(currentUrl);
            var folderPath = ConfigurationManager.AppSettings["path"];
            var remoteFtpPath = ConfigurationManager.AppSettings["FTP:BDX:Path"];

            var downloader = NinjectConfig.Get<IDownloader>();

            Console.WriteLine("File download started");
            Console.WriteLine("Destination folder : " + folderPath);

            //download the file
            string fileName = string.Empty; //"NHS_I9125_12102015.zip";
            //fileName =  "NHS_I9125_12102015.zip";
            fileName = downloader.DownloadFileBDXFeed(remoteFtpPath, folderPath); //"proplinersdut12042015_0501.xml"; 
            Console.WriteLine("File download completed. File name " + fileName);

            if (string.IsNullOrEmpty(fileName)) return;

            //unzip the file
            var sourcePath = folderPath + "\\" + fileName;
            fileName = Utility.Zip.DecompressZip(sourcePath, folderPath);
            Console.WriteLine("unzip the file");

            if (string.IsNullOrEmpty(fileName)) return;
            string localDestinationPath = folderPath + "\\" + fileName;
            var newhomedeserializer = new XmlSerializer(typeof(NewHomeBDXListingRoot));
            var newhomereader = new StreamReader(localDestinationPath);
            var newhomeBdxobj = newhomedeserializer.Deserialize(newhomereader);
            var newhomeBdxListing = (NewHomeBDXListingRoot)newhomeBdxobj;

            #endregion


            Repositories.Models.NewHome.NewHomeListingRoot _NewHomeListingRoot =
                new Repositories.Models.NewHome.NewHomeListingRoot();
            _NewHomeListingRoot.Builders = new Builders { Builder = new List<Repositories.Models.NewHome.Builder>(), Total = "0" };


            foreach (var Subdivision in newhomeBdxListing.Subdivision)
            {
                var objCommunities = new Communities();
                var objBuilder = new Repositories.Models.NewHome.Builder();
                var objPlan = new Repositories.Models.NewHome.Plan();

                Repositories.Models.NewHome.Community objCommunity = new Repositories.Models.NewHome.Community();
                objCommunity.Driving_directions = Subdivision.DrivingDirections;
                objCommunity.Description = Subdivision.SubDescription;
                Repositories.Models.NewHome.Images lstImage = getImage(Subdivision);
                objCommunity.Images = lstImage;

                objCommunity.CommunityId = Subdivision.SubdivisionID;
                objCommunity.Number = Subdivision.SubdivisionNumber;
                objCommunity.Name = Subdivision.SubdivisionName;
                objCommunity.Address = Subdivision.Address;
                objCommunity.State = Subdivision.State;
                objCommunity.City = Subdivision.City;
                objCommunity.Zip = Subdivision.Zip;
                objCommunity.Longitude = Convert.ToDouble(Subdivision.Longitude);
                objCommunity.Latitude = Convert.ToDouble(Subdivision.Latitude);
                objCommunity.Phone = Subdivision.Phone;
                objCommunity.MarketID = Subdivision.MarketID;
                objCommunity.MarketName = Subdivision.MarketName;
                //Add all the Community amenities
                objCommunity.Community_amenities = getCommunities(Subdivision);
                objCommunity.Plans = getListingData(Subdivision, objCommunity);

                objCommunities.Community.Add(objCommunity);


                objBuilder = new Repositories.Models.NewHome.Builder();
                objBuilder.BuilderId = Subdivision.BuilderID;
                objBuilder.Number = Subdivision.BuilderNumber;
                objBuilder.Name = Subdivision.BrandName;
                objBuilder.Logo_url = Subdivision.BrandLogo_Med;
                objBuilder.Website = Subdivision.BuilderWebSite;
                objBuilder.Communities = objCommunities;

                if (_NewHomeListingRoot.Builders.Builder.Any(x => x.BuilderId == objBuilder.BuilderId))
                {
                    var _builder = _NewHomeListingRoot.Builders.Builder.FirstOrDefault(x => x.BuilderId == objBuilder.BuilderId);
                    objCommunities.Community.ForEach(x =>
                    {
                        if (_builder.Communities.Community.Any(y => y.CommunityId == y.CommunityId))
                        {
                            //no action could done
                        }
                        else
                        {
                            _builder.Communities.Community.Add(x);
                        }

                    });

                }
                else
                {
                    _NewHomeListingRoot.Builders.Builder.Add(objBuilder);
                }

            }

            #region "ProcessFeed"
            ProcessFeed(_NewHomeListingRoot);
            Console.WriteLine("Process Complted");
            //Console.ReadLine();
            #endregion

            // }foreach url
        }

        private static void ProcessFeed(NewHomeListingRoot newhomeListing)
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
                                Zip1 = community.Zip,
                                PhoneNo = community.Phone
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
                        //plan.Communityprice_low = string.IsNullOrEmpty(community.Price_low)
                        //    ? 0
                        //    : Convert.ToDouble(community.Price_low);
                        //plan.Communityprice_high = string.IsNullOrEmpty(community.Price_high)
                        //    ? 0
                        //    : Convert.ToDouble(community.Price_high);
                        //plan.Communitysqft_high = string.IsNullOrEmpty(community.Sqft_high)
                        //    ? 0
                        //    : Convert.ToDouble(community.Sqft_high);
                        //plan.Communitysqft_low = string.IsNullOrEmpty(community.Sqft_low)
                        //    ? 0
                        //    : Convert.ToDouble(community.Sqft_low);
                        plan.Communityaddress = community.Address;
                        plan.Communitycity = community.City;
                        plan.Communitystate = community.State;
                        plan.Communityzip = community.Zip;
                        plan.Latitude = community.Latitude;
                        plan.Longitude = community.Longitude;
                        plan.CommunityPhone = community.Phone;
                        plan.CommunityMarketID = community.MarketID;
                        plan.CommunityMarketName = community.MarketName;
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

        private static Repositories.Models.NewHome.Images getImage(Repositories.Models.NewHomeBDXFeed.Subdivision subData)
        {
            Repositories.Models.NewHome.Images lstImage = new Repositories.Models.NewHome.Images();

            foreach (var item in subData.Image)
            {
                var img = new Repositories.Models.NewHome.Image();
                img.Reference = item.Text;
                img.Sequence_position = item.Seq;
                img.Type = item.Type;
                lstImage.Image.Add(img);
            }
            return lstImage;
        }

        private static Repositories.Models.NewHome.Plans getListingData(
            Repositories.Models.NewHomeBDXFeed.Subdivision subData,
            Repositories.Models.NewHome.Community community)
        {
            var lstHomes = new Repositories.Models.NewHome.Homes();
            var objhome = new Repositories.Models.NewHome.Home();
            var objPlans = new Repositories.Models.NewHome.Plans() { Plan = new List<Plan>(), Total = "0" };
            var objPlan = new Repositories.Models.NewHome.Plan();
            foreach (var item in subData.Listing)
            {

                objPlan = new Repositories.Models.NewHome.Plan();
                objPlan.Garage =  Convert.ToInt32(item.Garage);
                objPlan.Description = item.Description;
                objPlan.PlanId = item.ListingID;
                objPlan.Images = getImage(item);
                objPlan.Number = item.ListingID;
                objPlan.CommunityName = item.PlanName;
                objPlan.Base_price = item.BasePrice;
                objPlan.Communityprice_low = string.IsNullOrEmpty(item.BasePrice) == true ? 0.0 : Convert.ToDouble(item.BasePrice);
                objPlan.Communityprice_low = Convert.ToDouble(item.BasePrice);
                string[] dataBedrooms = ResolveBathroom_BedRoom(item.Bedrooms);
                objPlan.Bedrooms = Convert.ToInt32(dataBedrooms[0]);
                objPlan.HalfBedrooms = Convert.ToInt32(dataBedrooms[1]);
                string[] dataBaths = ResolveBathroom_BedRoom(item.Baths);
                objPlan.Baths = Convert.ToInt32(dataBaths[0]);
                objPlan.Half_baths = Convert.ToInt32(dataBaths[1]);
                objPlan.Communitysqft_low = Convert.ToDouble(item.BaseSqft);
                objPlan.Stories = item.Stories;
                objPlan.Master_bedroom_location = item.MasterBedLocation;
                objPlan.BuilderNumber = item.ListingID;
                objPlan.BuilderName = item.PlanName;
                objPlan.BuilderEmail = subData.Email;
                if (!String.IsNullOrEmpty(item.ListingAddress))
                {
                    objPlan.Communityaddress = item.ListingAddress;
                    objPlan.Communitycity = item.ListingCity;
                    objPlan.Communitystate = item.ListingState;
                    objPlan.Communityzip = item.ListingZIP;
                    //objPlan.GeoPoint = new GeoPoint(community.Latitude, community.Longitude);  
                }
                else
                {
                    objPlan.Communityaddress = community.Address;
                    objPlan.Communitycity = community.City;
                    objPlan.Communitystate = community.State;
                    objPlan.Communityzip = community.Zip;
                    objPlan.GeoPoint = new GeoPoint(community.Latitude, community.Longitude);
                }

                if (subData.State == "Active")
                {
                    objPlan.Is_active = "1";
                }

                objPlan.LivingAreas = item.LivingAreas;
                objPlan.Dining_areas = item.DiningAreas;
                objPlan.Basement = Convert.ToInt32(String.IsNullOrEmpty(item.Basement) ? "0" : item.Basement);


                objhome.Description = item.Description;
                Repositories.Models.NewHome.Images lstImage = getImage(item);
                objhome.Images = lstImage;
                objhome.Number = item.ListingID;
                objhome.Listing_title = item.PlanName;
                objhome.Price = item.BasePrice;
                string[] data = ResolveBathroom_BedRoom(item.Bedrooms);
                objhome.Bedrooms = (data[0]);
                objhome.HalfBedrooms = (data[1]);
                string[] dataBaths1 = ResolveBathroom_BedRoom(item.Bedrooms);
                objhome.Baths = dataBaths1[0];
                objhome.Half_baths = dataBaths1[1];
                objhome.Sqft = item.BaseSqft;
                objhome.Garage = item.Garage;
                objhome.Stories = item.Stories;
                objhome.Master_bedroom_location = item.MasterBedLocation;
                objhome.Listing_url = item.NHSListingURL;
                objhome.Home_type = item.PlanType;
                objhome.Address = item.ListingAddress;
                objhome.City = item.ListingCity;
                objhome.State = item.ListingState;
                objhome.Zip = item.ListingZIP;
                objhome.Move_in_date = item.ListingMoveInDate;
                objhome.ListingType = item.ListingType;
                objhome.Id = Utility.UtilityClass.GetUniqueKey();
                //Added the data into plan, but need to understand hoe to intregrate the data with NewHomeListing?


                objPlan.Homes = new Homes() { Home = new List<Home>(), Total = "0" };
                objPlan.Homes.Home.Add(objhome);
                objPlan.Homes.Total = objPlan.Homes.Home.Count().ToString();

                objPlans.Plan.Add(objPlan);
                objPlans.Total = objPlans.Plan.Count().ToString();
            }
            return objPlans;
        }

        private static Repositories.Models.NewHome.Images getImage(Repositories.Models.NewHomeBDXFeed.Listing subData)
        {
            Repositories.Models.NewHome.Images lstImage = new Repositories.Models.NewHome.Images();

            foreach (var item in subData.Image)
            {
                lstImage.Image.Add(new Repositories.Models.NewHome.Image
                {
                    Reference = item.Text,
                    Sequence_position = item.Seq,
                    Type = item.Type
                });
            }
            return lstImage;
        }

        private static Repositories.Models.NewHome.Community_amenities getCommunities(Repositories.Models.NewHomeBDXFeed.Subdivision subData)
        {
            Repositories.Models.NewHome.Community_amenities objCommunity_amenities = new Repositories.Models.NewHome.Community_amenities();
            Repositories.Models.NewHome.Community_amenity objCommunity_amenity = new Repositories.Models.NewHome.Community_amenity();

            if (!String.IsNullOrEmpty(subData.Park))
            {
                objCommunity_amenity.Name = "Park";
                objCommunity_amenity.TextValue = subData.Park;
                objCommunity_amenities.Community_amenity.Add(objCommunity_amenity);
            }
            if (!String.IsNullOrEmpty(subData.Pool))
            {
                objCommunity_amenity = new Repositories.Models.NewHome.Community_amenity();
                objCommunity_amenity.Name = "Pool";
                objCommunity_amenity.TextValue = subData.Pool;
                objCommunity_amenities.Community_amenity.Add(objCommunity_amenity);
            }
            if (!String.IsNullOrEmpty(subData.SportFacilities))
            {
                objCommunity_amenity = new Repositories.Models.NewHome.Community_amenity();
                objCommunity_amenity.Name = "SportFacilities";
                objCommunity_amenity.TextValue = subData.SportFacilities;
                objCommunity_amenities.Community_amenity.Add(objCommunity_amenity);
            }
            if (!String.IsNullOrEmpty(subData.Greenbelt))
            {
                objCommunity_amenity = new Repositories.Models.NewHome.Community_amenity();
                objCommunity_amenity.Name = "Greenbelt";
                objCommunity_amenity.TextValue = subData.Greenbelt;
                objCommunity_amenities.Community_amenity.Add(objCommunity_amenity);
            }
            if (!String.IsNullOrEmpty(subData.GolfCourse))
            {
                objCommunity_amenity = new Repositories.Models.NewHome.Community_amenity();
                objCommunity_amenity.Name = "GolfCourse";
                objCommunity_amenity.TextValue = subData.GolfCourse;
                objCommunity_amenities.Community_amenity.Add(objCommunity_amenity);
            }
            if (!String.IsNullOrEmpty(subData.Views))
            {
                objCommunity_amenity = new Repositories.Models.NewHome.Community_amenity();
                objCommunity_amenity.Name = "Views";
                objCommunity_amenity.TextValue = subData.Views;
                objCommunity_amenities.Community_amenity.Add(objCommunity_amenity);
            }

            return objCommunity_amenities;
        }

        private static string[] ResolveBathroom_BedRoom(string bathroomnumber)
        {
            if (!String.IsNullOrEmpty(bathroomnumber))
            {
                if (bathroomnumber.Contains('.'))
                {

                    string[] strArr = bathroomnumber.Split('.');

                    return strArr;
                }
                else
                {
                    string[] strArr = { bathroomnumber, "0" };
                    return strArr;
                }
            }
            else
            {
                string[] strArr = { "0", "0" };
                return strArr;
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
