using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Repositories.Interfaces.Admin.ListHub;
using Repositories.Interfaces.Admin.NewHome;
using Repositories.Interfaces.ListHub;
using Repositories.Interfaces.NewHome;
using Repositories.Models.Admin.ListHub;
using Repositories.Models.Admin.NewHome;
using Repositories.Models.DataTable;
using Repositories.Models.ViewModel;
using Security.Models;
using System.IO;
using System.Web;
using Repositories.Models.ListHub;
using System.Web.Script.Serialization;
using Repositories.Models.NewHome;
using AdminInterface.Models.NewHome;
using Repositories.Interfaces.ElasticSearch;
using AutoMapper;
using Repositories.Models.Common;
using Repositories.Interfaces.Community;
using Repositories.Models.Admin.Community;
using Repositories.Interfaces.Admin.Users;
using Configuration;
using Repositories.Interfaces.Admin.Office;
using Utility;

namespace AdminInterface.Controllers
{
    [RoutePrefix("property")]
    [Route("{action}")]
    [CustomAuthorize(Roles = "Admin,Agent")]

    public class PropertyController : BaseController
    {
        private readonly IProperties _properties;
        private readonly IListHub _listHub;
        private readonly INewHome _newHomeMain;
        private readonly ICommunityProvider _newHomeCommunity;
        private readonly IAgent _agent;
        private readonly IOffice _office;

        private readonly INewHomes _newHome;
        private readonly IElasticSearchIndices<Purchase> _ElasticSearchIndicesPurchase;
        private readonly IElasticSearchIndices<Rental> _ElasticSearchIndicesRental;
        readonly IElasticSearchIndices<Repositories.Models.ListHub.NewHome> _ElasticSearchIndicesNewHome;
        public PropertyController(IProperties properties,
            IListHub listHub,
            INewHomes newHome,
            INewHome newHomeMain,
            IAgent agent,
            IOffice office,
            IElasticSearchIndices<Purchase> ElasticSearchIndicesPurchase,
            IElasticSearchIndices<Rental> ElasticSearchIndicesRental,
            IElasticSearchIndices<Repositories.Models.ListHub.NewHome> ElasticSearchIndicesNewHome,
            ICommunityProvider newHomeCommunity)
        {
            _properties = properties;
            _listHub = listHub;
            _newHome = newHome;
            _newHomeMain = newHomeMain;
            _ElasticSearchIndicesPurchase = ElasticSearchIndicesPurchase;
            _ElasticSearchIndicesRental = ElasticSearchIndicesRental;
            _ElasticSearchIndicesNewHome = ElasticSearchIndicesNewHome;
            _newHomeCommunity = newHomeCommunity;
            _agent = agent;
            _office = office;
        }
        #region Purchase

        [Route("~/")]
        [Route]
        [Route("purchase-property-listing")]
        public ActionResult PurchaseListing()
        {
            return View();
        }

        [Route("purchase-listing-ajax-handler")]
        public ActionResult PurchaseAjaxHandler(JQueryDataTableParamModel param)
        {
            var email = @User.Email;

            if (User.Roles.Contains("Admin"))
            {
                email = "";
            }
            long filterCount = 0;
            var listHubPropertysearchCriteria = new ListHubPropertyDataTable()
            {
                isImageSearchable = Convert.ToBoolean(Request["bSearchable_0"]),
                isMlsSearchable = Convert.ToBoolean(Request["bSearchable_1"]),
                isPriceSearchable = Convert.ToBoolean(Request["bSearchable_2"]),
                isPropertySearchable = Convert.ToBoolean(Request["bSearchable_3"]),
                isLivingArearSearchable = Convert.ToBoolean(Request["bSearchable_4"]),
                isBathSearchable = Convert.ToBoolean(Request["bSearchable_5"]),
                isBedSearchable = Convert.ToBoolean(Request["bSearchable_6"]),
                isNewConstuctionSearchable = Convert.ToBoolean(Request["bSearchable_7"]),
                isFeaturedSearchable = Convert.ToBoolean(Request["bSearchable_8"]),
                isAddressSearchable = Convert.ToBoolean(Request["bSearchable_9"]),

                isImageSortable = Convert.ToBoolean(Request["bSortable_0"]),
                isMlsSortable = Convert.ToBoolean(Request["bSortable_1"]),
                isPriceSortable = Convert.ToBoolean(Request["bSortable_2"]),
                isPropertySortable = Convert.ToBoolean(Request["bSortable_3"]),
                isLivingArearSortable = Convert.ToBoolean(Request["bSortable_4"]),
                isBathSortable = Convert.ToBoolean(Request["bSortable_5"]),
                isBedSortable = Convert.ToBoolean(Request["bSortable_6"]),
                isNewConstuctionSortable = Convert.ToBoolean(Request["bSortable_7"]),
                isFeaturedSortable = Convert.ToBoolean(Request["bSortable_8"]),
                isAddressSortable = Convert.ToBoolean(Request["bSortable_9"]),

                sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]),
                sortDirection = Request["sSortDir_0"]
            };

            var filteredData = _properties.GetDataSet(email, param, listHubPropertysearchCriteria, out filterCount, "purchase");

            var totalCount = _properties.GetTotalCount(email);

            var result = from c in filteredData
                         select
                             new[]
                             {
                         (c.DefaultPhoto == null || c.DefaultPhoto.Count()==0) ? "" : c.DefaultPhoto[0],
                         c.MlsNumber,
                         c.Price.ToString("#,##,##0.00"),
                         c.PropertyType,
                          c.LivingArea.ToString("#,##,##0.00"),
                         Convert.ToString(c.NoOfBathRooms)+ "/" + Convert.ToString(c.NoOfHalfBathRooms),
                         Convert.ToString(c.NoOfBedRooms),
                         Convert.ToString(c.IsNewConstruction),
                         Convert.ToString(c.IsFeatured),
                         c.FullStreetAddress
                             };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filterCount,
                aaData = result
            },
                JsonRequestBehavior.AllowGet);
        }

        [Route("delete-PurchaseListing")]
        public JsonResult DeletePurchaseListing(string mlsid)
        {
            var result = _listHub.DeleteListHub(mlsid, "purchase");
            return Json(new { success = result });
        }

        [Route("delete-RentListing")]
        public JsonResult DeleteRentListing(string mlsid)
        {
            var result = _listHub.DeleteListHub(mlsid, "rent");
            return Json(new { success = result });
        }


        [Route("getdetails/{type}/{uniqueid}")]
        public ActionResult GetDetails(string type, string uniqueid)
        {
            if (type == "purchase")
            {
                return Json(_listHub.GetPurchasePropertyDetailsByMLSNumber(uniqueid), JsonRequestBehavior.AllowGet);
            }
            else if (type == "rent")
            {
                return Json(_listHub.GetRentPropertyDetailsByMLSNumber(uniqueid), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(_newHomeMain.GetNewHomePropertyDetailsByMLSNumber(uniqueid), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Rent
        [Route("rent-property-listing")]
        public ActionResult RentListing()
        {
            return View();
        }

        [Route("rent-listing-ajax-handler")]
        public ActionResult RentAjaxHandler(JQueryDataTableParamModel param)
        {
            var email = @User.Email;

            if (User.Roles.Contains("Admin"))
            {
                email = "";
            }
            long filterCount = 0;
            var listHubPropertysearchCriteria = new ListHubPropertyDataTable()
            {
                isImageSearchable = Convert.ToBoolean(Request["bSearchable_0"]),
                isMlsSearchable = Convert.ToBoolean(Request["bSearchable_1"]),
                isPriceSearchable = Convert.ToBoolean(Request["bSearchable_2"]),
                isPropertySearchable = Convert.ToBoolean(Request["bSearchable_3"]),
                isLivingArearSearchable = Convert.ToBoolean(Request["bSearchable_4"]),
                isBathSearchable = Convert.ToBoolean(Request["bSearchable_5"]),
                isBedSearchable = Convert.ToBoolean(Request["bSearchable_6"]),
                isNewConstuctionSearchable = Convert.ToBoolean(Request["bSearchable_7"]),
                isFeaturedSearchable = Convert.ToBoolean(Request["bSearchable_8"]),
                isAddressSearchable = Convert.ToBoolean(Request["bSearchable_9"]),

                isImageSortable = Convert.ToBoolean(Request["bSortable_0"]),
                isMlsSortable = Convert.ToBoolean(Request["bSortable_1"]),
                isPriceSortable = Convert.ToBoolean(Request["bSortable_2"]),
                isPropertySortable = Convert.ToBoolean(Request["bSortable_3"]),
                isLivingArearSortable = Convert.ToBoolean(Request["bSortable_4"]),
                isBathSortable = Convert.ToBoolean(Request["bSortable_5"]),
                isBedSortable = Convert.ToBoolean(Request["bSortable_6"]),
                isNewConstuctionSortable = Convert.ToBoolean(Request["bSortable_7"]),
                isFeaturedSortable = Convert.ToBoolean(Request["bSortable_8"]),
                isAddressSortable = Convert.ToBoolean(Request["bSortable_9"]),

                sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]),
                sortDirection = Request["sSortDir_0"]
            };

            var filteredData = _properties.GetDataSet(email, param, listHubPropertysearchCriteria, out filterCount, "rent");

            var totalCount = _properties.GetTotalCount(email, "rent");

            var result = from c in filteredData
                         select
                             new[]
                    {
                         (c.DefaultPhoto == null || c.DefaultPhoto.Count()==0) ? "" : c.DefaultPhoto[0],
                         c.MlsNumber,
                         c.Price.ToString("#,##,##0.00"),
                         c.PropertyType,
                          c.LivingArea.ToString("#,##,##0.00"),
                         Convert.ToString(c.NoOfBathRooms)+ "/" + Convert.ToString(c.NoOfHalfBathRooms),
                         Convert.ToString(c.NoOfBedRooms),
                         Convert.ToString(c.IsNewConstruction),
                         Convert.ToString(c.IsFeatured),
                         c.FullStreetAddress
                    };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filterCount,
                aaData = result
            },
                JsonRequestBehavior.AllowGet);
        }

        [Route("{type}/new-Property")]
        public ActionResult AddNewProperty(string type, string MlsNumber)
        {
            if (!string.IsNullOrEmpty(MlsNumber))
            {
                ViewBag.MlsNumber = MlsNumber;
            }
            ListHubListing lhl = new ListHubListing();
            ViewBag.Office = _office.GetOffice();
            ViewBag.Communities = _newHomeCommunity.GetCommunities();
            ViewBag.PropertyType = _listHub.GetPropertyType(type);
            ViewBag.PropertyTypeDesc = _listHub.GetPropertyTypeDesc(type);
            ViewBag.PropertySubType = _listHub.GetSubPropertyType(type);
            ViewBag.PropertySubTypeDesc = _listHub.GetSubPropertyTypeDesc(type);
            ViewBag.ListingStatus = _listHub.GetListingStatus(type);
            ManagePropertyViewModel managePropertyViewModel = new ManagePropertyViewModel();
            ListPrice listPrice = new ListPrice();
            lhl.ListingCategory = type;
            lhl.ExtProperties = managePropertyViewModel;
            lhl.ListPrice = listPrice;

            return View(lhl);
        }



        [HttpPost]
        [Route("{type}/new-Property")]
        public ActionResult AddNewProperty(string type, ListHubListing listHubListing)   //only add
        {
            if (ModelState.IsValid)
            {
                //Insert
                if (string.IsNullOrEmpty(listHubListing.MlsNumber))
                {
                    listHubListing.Address.Preferenceorder = "1";
                    listHubListing.Address.Addresspreferenceorder = "1";
                    ListPrice lstPrice = new ListPrice();
                    lstPrice.IsgSecurityClass = "Public";
                    lstPrice.Text = listHubListing.ListPrice.Text;
                    listHubListing.ListPrice = lstPrice;
                    LotSize lotSize = new LotSize();
                    lotSize.AreaUnits = "acre";
                    lotSize.Text = listHubListing.LotSize.Text;
                    listHubListing.LotSize = lotSize;
                    listHubListing.ProviderCategory = User.Roles.FirstOrDefault();
                    listHubListing.ListingCategory = type;
                    listHubListing.Bathrooms = listHubListing.FullBathrooms + listHubListing.HalfBathrooms + listHubListing.ThreeQuarterBathrooms + listHubListing.OneQuarterBathrooms + listHubListing.PartialBathrooms;
                    listHubListing.MlsId = "SDRE";
                    listHubListing.MlsNumber = Utility.UtilityClass.GetUniqueKey();
                    listHubListing.MlsName = "San Diego Real Estate";
                    listHubListing.ListingKey = "3yd-" + listHubListing.MlsId + "-" + listHubListing.MlsNumber;
                    ManagePropertyViewModel managePropertyViewModel = new ManagePropertyViewModel();
                    managePropertyViewModel.UniqueId = listHubListing.MlsNumber;
                    listHubListing.ExtProperties = managePropertyViewModel;
                    ViewBag.MlsNumber = listHubListing.MlsNumber;
                    Brokerage brokerage = new Brokerage();
                    brokerage.Email = User.Email;
                    brokerage.Name = User.FirstName + " " + User.LastName;
                    listHubListing.Brokerage = brokerage;
                    listHubListing.ListedBy = "AdminPanel";
                    listHubListing.CreatedBy = User.ParticipantId;
                    listHubListing.CreatedDate = DateTime.Now.ToString();
                    listHubListing.IsUpdateByPortal = true;


                    if (listHubListing.ListingParticipants != null)
                    {
                        listHubListing.ListingParticipants.Participant = _agent.GetAgentDetails(listHubListing.ListingParticipants.Participant.ParticipantId);
                    }

                    if (!string.IsNullOrEmpty(listHubListing.ProviderName))
                    {

                        if (listHubListing.Offices == null) listHubListing.Offices = new Offices();
                        listHubListing.Offices.Office = _office.GetOfficeDetails(listHubListing.ProviderName);
                        listHubListing.ProviderName = listHubListing.Offices.Office.CorporateName;
                    }


                    _listHub.InsertRealestate(listHubListing, type);

                    if (type.ToLower() == "purchase")
                    {
                        var purchase = Mapper.Map<Purchase>(listHubListing);
                        var response = _ElasticSearchIndicesPurchase.CreateIndex(purchase);
                    }
                    else if (type.ToLower() == "rent")
                    {
                        var rent = Mapper.Map<Rental>(listHubListing);
                        var response = _ElasticSearchIndicesRental.CreateIndex(rent);
                    }
                }
                else  //Update
                {
                    //var update = _listHub.UpdateListHub(type,listHubListing);
                    //done from another function
                }
            }
            ViewBag.Office = _office.GetOffice();
            ViewBag.Communities = _newHomeCommunity.GetCommunities();
            ViewBag.PropertyType = _listHub.GetPropertyType(type);
            ViewBag.PropertyTypeDesc = _listHub.GetPropertyTypeDesc(type);
            ViewBag.PropertySubType = _listHub.GetSubPropertyType(type);
            ViewBag.PropertySubTypeDesc = _listHub.GetSubPropertyTypeDesc(type);
            ViewBag.ListingStatus = _listHub.GetListingStatus(type);

            if (ViewBag.MlsNumber != null)
            {
                return RedirectToAction("AddNewProperty", new { type = type, MlsNumber = ViewBag.MlsNumber });
            }
            else
            {
                return View(listHubListing);
            }
        }

        [HttpGet]
        [Route("{type}/new-Property/{MlsNumber}")]
        public ActionResult EditListhubProperty(string type, string MlsNumber)
        {
            //ListHubListing lhl = new ListHubListing();
            ViewBag.Office = _office.GetOffice();
            ViewBag.Communities = _newHomeCommunity.GetCommunities();
            ViewBag.PropertyType = _listHub.GetPropertyType(type);
            ViewBag.PropertyTypeDesc = _listHub.GetPropertyTypeDesc(type);
            ViewBag.PropertySubType = _listHub.GetSubPropertyType(type);
            ViewBag.PropertySubTypeDesc = _listHub.GetSubPropertyTypeDesc(type);
            ViewBag.ListingStatus = _listHub.GetListingStatus(type);
            var listhubListing = new ListHubListing();

            if (type == "purchase") listhubListing = _listHub.GetPurchasePropertyDetailsByMLSNumber(MlsNumber);
            if (type == "rent") listhubListing = _listHub.GetRentPropertyDetailsByMLSNumber(MlsNumber);
            return View(listhubListing);
        }
        [HttpPost]
        [Route("{type}/new-Property/{MlsNumber}")]
        public ActionResult EditListhubProperty(string type, ListHubListing listhubListing)
        {

            ViewBag.Office = _office.GetOffice();
            ViewBag.Communities = _newHomeCommunity.GetCommunities();
            ViewBag.PropertyType = _listHub.GetPropertyType(type);
            ViewBag.PropertyTypeDesc = _listHub.GetPropertyTypeDesc(type);
            ViewBag.PropertySubType = _listHub.GetSubPropertyType(type);
            ViewBag.PropertySubTypeDesc = _listHub.GetSubPropertyTypeDesc(type);
            ViewBag.ListingStatus = _listHub.GetListingStatus(type);


            if (ModelState.IsValid)
            {
                if (listhubListing.ListingParticipants != null)
                {
                    listhubListing.ListingParticipants.Participant = _agent.GetAgentDetails(listhubListing.ListingParticipants.Participant.ParticipantId);
                }

                if (!string.IsNullOrEmpty(listhubListing.ProviderName))
                {
                    if (listhubListing.Offices == null) listhubListing.Offices = new Offices();
                    listhubListing.Offices.Office = _office.GetOfficeDetails(listhubListing.ProviderName);
                    listhubListing.ProviderName = listhubListing.Offices.Office.CorporateName;
                }

                listhubListing.IsUpdateByPortal = true;
                var update = _listHub.UpdateListHub(type, listhubListing);

                if (type.ToLower() == "purchase")
                {
                    var purchase = Mapper.Map<Purchase>(listhubListing);
                    var response = _ElasticSearchIndicesPurchase.CreateIndex(purchase);
                }
                else if (type.ToLower() == "rent")
                {
                    var rent = Mapper.Map<Rental>(listhubListing);
                    var response = _ElasticSearchIndicesRental.CreateIndex(rent);
                }


                ViewBag.success = 1;
            }
            else
            {
                ViewBag.success = 0;
            }
            return View(listhubListing);
        }

        [HttpGet]
        [Route("GetOfficeAgents")]
        public JsonResult GetOfficeAgents(string officeid)
        {
            //var office = _office.GetOfficeDetailsByName(officeName);
            var agentList = _agent.GetAgents(officeid);
            return Json(agentList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region NewHome
        [Route("newhome-property-listing")]

        public ActionResult NewHomeListing()
        {
            return View();
        }

        [Route("{type}/new-home")]
        public ActionResult AddNewHome(string planId, string homeId, string success)
        {
            //NewHomeViewModel

            var newHomeModel = new Repositories.Models.NewHome.Home();

            ViewBag.PropertyType = _newHomeMain.GetPropertyType();
            ViewBag.planId = planId;
            if (homeId == "0" || string.IsNullOrEmpty(homeId))//for new home add
            {
                if (success == "1")
                {
                    ViewBag.AddSuccess = true;
                }
                else if (success == "0")
                {
                    ViewBag.AddSuccess = false;
                }
                return View(newHomeModel);
            }
            else// this is for update home
            {
                var plan = _newHomeMain.PlanDetails(planId);
                newHomeModel = plan.Homes.Home.Where(x => x.Id == homeId).FirstOrDefault();
                return View(newHomeModel);
            }

        }

        /// <summary>
        /// Delete home under plan
        /// </summary>
        /// <param name="planid">Unique Id of plan</param>
        /// <param name="homeId">Under plan, multiple home listed, each home have a unique id.</param>
        /// <param name="success"></param>
        /// <returns></returns>
        [Route("{type}/delete-home")]
        public ActionResult DeleteNewHome(string planid, string homeId, string success)
        {
            //NewHomeViewModel

            var newHomeModel = new Repositories.Models.NewHome.Home();

            ViewBag.PropertyType = _newHomeMain.GetPropertyType();
            ViewBag.planid = planid;
            _newHomeMain.PullHomeFromPlan(homeId, planid);

            //Update Elastic Search

            var elasticSearchindicesforNewHome = NinjectConfig.Get<IElasticSearchIndices<NewHome>>();
            var plan = _newHomeMain.PlanDetails(planid);
            var elasticSearchNewHome = Mapper.Map<Repositories.Models.ListHub.NewHome>(plan);
            elasticSearchindicesforNewHome.UpdateIndex(elasticSearchNewHome);  
          
            return RedirectToAction("newhome/new-plan", new { planid = planid });
 

        }

        [Route("newhome/Homes/Images/{planid}/{homeid}")]
        public ActionResult AddHomeImages(string planid, string homeid)
        {
            if (homeid == "null")
            {
                return View(new Home());
            }
            else
            {
                ViewBag.homeid = homeid;
                ViewBag.planid = planid;
                var plan = _newHomeMain.PlanDetails(planid);
                Home newHomeModel = plan.Homes.Home.Where(x => x.Id == homeid).FirstOrDefault();
                return View(newHomeModel);
            }
        }

        [Route("newhome/UploadHomeImages/{planid}/{homeid}")]
        [HttpPost]
        public ActionResult UploadHomeImages(string[] imageList, string planid, string homeid)
        {

            var result = _newHomeMain.UpdateHomeImageList(imageList, planid, homeid);

            return Json(result);
        }

        [Route("{type}/new-plan")]
        public ActionResult AddNewPlan(string type, string planid)
        {
            Plan p = new Plan();

            if (!string.IsNullOrEmpty(planid))
            {
                ViewBag.planId = planid;
                p = _newHomeMain.PlanDetails(planid);
                p.PlanId = planid;
            }
            p.CommunityList = _newHomeCommunity.GetCommunities();
            p.BuilderList = _agent.GetBuilderList();
            return View(p);
        }

        //for both add and edit
        [Route("{type}/newhomeplan")]
        [HttpPost]
        public JsonResult AddNewPlan(string type, Plan p, string planid)
        {
            var elasticSearchindicesforNewHome = NinjectConfig.Get<IElasticSearchIndices<NewHome>>();

            Repositories.Models.Community.Communities com = _newHomeCommunity.GetCommunitiesByName(p.CommunityName);
            var user = _agent.GetAgentDetails(p.BuilderNumber);// we are sending builder participant id in email field.
            if (planid == "0")  //insert
            {
                p.BuilderEmail = user.Email;
                p.BuilderName = user.FirstName + "" + user.LastName;
                p.BuilderNumber = user.BuilderId;
                p.Builder_dre_number = user.dre_number;

                p.Communityaddress = com.Address;
                p.CommunityName = com.CommunityName;
                p.CommunityNumber = com.Number;
                p.Communitycity = com.City;
                p.Communitystate = com.State;
                p.Communityzip = com.Zips == null ? com.Zip1 : com.Zips.FirstOrDefault();
                p.PlanId = Utility.UtilityClass.GetUniqueKey();
                _newHomeMain.InsertNewHome(p);

                List<Plan> plans = new List<Plan>();
                plans.Add(p);
                var elasticsearchNewHome = plans.Select(Mapper.Map<Repositories.Models.ListHub.NewHome>).ToList();
                _newHomeMain.InsertFromFeed(plans);
                elasticSearchindicesforNewHome.CreateBulkIndex(elasticsearchNewHome);
                return Json(new { success = true, PlanId = p.PlanId });
            }
            else  //update
            {
                p.PlanId = planid;
                p.BuilderEmail = user.Email;
                p.BuilderName = user.FirstName + "" + user.LastName;
                p.BuilderNumber = user.BuilderId;
                p.Builder_dre_number = user.dre_number;

                p.Communityaddress = com.Address;
                p.CommunityName = com.CommunityName;
                p.CommunityNumber = com.Number;
                p.Communitycity = com.City;
                p.Communitystate = com.State;
                p.Communityzip = com.Zips == null ? com.Zip1 : com.Zips.FirstOrDefault();
                _newHomeMain.UpdateNewHomePlan(p);
                List<Plan> plans = new List<Plan>();
                plans.Add(p);
                var elasticsearchNewHome = plans.Select(Mapper.Map<Repositories.Models.ListHub.NewHome>).FirstOrDefault();
                elasticSearchindicesforNewHome.UpdateIndex(elasticsearchNewHome);
                return Json(new { success = true, PlanId = p.PlanId });
            }
        }

        [Route("{type}/UploadPlanImages")]
        [HttpPost]
        public JsonResult UploadPlanImages(string type, string[] imageList, string planid)
        {
            if (type == "newhome")
            {
                _newHomeMain.UpdatePlanImageList(imageList, planid);
                var plan = _newHomeMain.PlanDetails(planid);
                var elasticsearchNewHome = Mapper.Map<Repositories.Models.ListHub.NewHome>(plan);
                var elasticSearchindicesforNewHome = NinjectConfig.Get<IElasticSearchIndices<NewHome>>();
                elasticSearchindicesforNewHome.UpdateIndex(elasticsearchNewHome);
            }
            TempData["planid"] = planid;
            return Json(true);
        }

        [Route("newhome/Images/{planid}")]
        [HttpGet]
        public ActionResult PlanImages(string planid)
        {
            planid = planid == "[object Object]" ? ((string)TempData["planid"]) : planid;
            var plan = _newHomeMain.PlanDetails(planid);
            ViewBag.planid = planid;
            return View(plan);
        }

        #region community
        [Route("community-listing")]
        public ActionResult CommunityListing()
        {
            return View();
        }

        [Route("community-listing-ajax-handler")]
        public ActionResult CommunityAjaxHandler(JQueryDataTableParamModel param)
        {
            var email = @User.Email;

            if (User.Roles.Contains("Admin"))
            {
                email = "";
            }
            long filterCount = 0;
            var communitySearchCriteria = new CommunityDataTable()
            {
                isIdSearchable = Convert.ToBoolean(Request["bSearchable_0"]),
                isNameSearchable = Convert.ToBoolean(Request["bSearchable_1"]),
                isNumberSearchable = Convert.ToBoolean(Request["bSearchable_2"]),
                isAddressSearchable = Convert.ToBoolean(Request["bSearchable_3"]),
                isCitySearchable = Convert.ToBoolean(Request["bSearchable_4"]),
                isEmailSearchable = Convert.ToBoolean(Request["bSearchable_5"]),

                isIdSortable = Convert.ToBoolean(Request["bSortable_0"]),
                isNameSortable = Convert.ToBoolean(Request["bSortable_1"]),
                isNumberSortable = Convert.ToBoolean(Request["bSortable_2"]),
                isAddressSortable = Convert.ToBoolean(Request["bSortable_3"]),
                isCitySortable = Convert.ToBoolean(Request["bSortable_4"]),
                isEmailSortable = Convert.ToBoolean(Request["bSortable_5"]),


                sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]),
                sortDirection = Request["sSortDir_0"]
            };

            var filteredData = _newHomeCommunity.GetDataSet(email, param, communitySearchCriteria, out filterCount, "purchase");

            var totalCount = _newHomeCommunity.GetTotalCount(email);

            var result = from c in filteredData
                         select
                             new[]
                             {
                                 c.CommunityId,
                                 c.CommunityName,
                                 c.Number,
                                 c.Address,
                                 c.City,
                                 c.Email,
                                 c.PhoneNo
                             };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filterCount,
                aaData = result
            },
                JsonRequestBehavior.AllowGet);
        }
        [Route("new-community")]
        public ActionResult NewHomeCommunity(string communityId)
        {
            Repositories.Models.Community.Communities com = new Repositories.Models.Community.Communities();
            com = _newHomeCommunity.GetCommunities(communityId);
            if (string.IsNullOrEmpty(communityId))
            {
                return View(new Repositories.Models.Community.Communities());
            }
            else
            {
                if (com.Zips != null)
                {
                    com.Zip1 = string.Empty;
                    foreach (var item in com.Zips)
                    {
                        com.Zip1 = string.IsNullOrEmpty(com.Zip1) ? item : com.Zip1 + "," + item;
                    }
                }
                return View(com);
            }
        }

        [Route("addcommunity")]
        public JsonResult NewHomeCommunity(string type, Repositories.Models.Community.Communities newHomeCommunity, string Zip)
        {
            if (_newHomeCommunity.Validation(newHomeCommunity))
            {
                newHomeCommunity.Zips = Zip.Split(',');
                List<Repositories.Models.Community.Communities> lstCom = new List<Repositories.Models.Community.Communities>();
                if (string.IsNullOrEmpty(newHomeCommunity.CommunityId))
                {
                    newHomeCommunity.CommunityId = Utility.UtilityClass.GetUniqueKey();
                    newHomeCommunity.Number = Utility.UtilityClass.GetUniqueKey();
                    newHomeCommunity.IsUpdateByPortal = true;
                    lstCom.Add(newHomeCommunity);
                    _newHomeCommunity.InsertCommunities(lstCom);
                }
                else
                {
                    string a = _newHomeCommunity.GetPreviousImageUrl(newHomeCommunity.CommunityId);
                    ImageDelete imgDelete = new ImageDelete();
                    imgDelete.DeleteImageWithoutUnique(a);
                    _newHomeCommunity.UpdateCommunities(newHomeCommunity);
                }
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        [Route("delete-community")]
        public JsonResult DeleteCommunity(string communityId)
        {
            _newHomeCommunity.DeleteCommunities(communityId);
            return Json(true);
        }
        #endregion

        [HttpPost]
        [Route("{type}/new-home")]
        public ActionResult AddNewHome(Home home, string planId)
        {
            if (ModelState.IsValid)
            {
                if (planId != null)
                {

                    if (String.IsNullOrEmpty(home.Id))
                    {
                        home.Id = Utility.UtilityClass.GetUniqueKey();
                        _newHomeMain.UpdateNewHomePlan(home, planId);
                    }
                    else
                    {
                        _newHomeMain.UpdateHome(home, planId);

                    }

                    //Update Elastic Search
                    var plan = _newHomeMain.PlanDetails(planId);
                    var elasticSearchNewHome = Mapper.Map<Repositories.Models.ListHub.NewHome>(plan);

                    _ElasticSearchIndicesNewHome.CreateIndex(elasticSearchNewHome);


                    return RedirectToAction("AddNewHome", new { planId = planId, success = "1" });
                }
                else
                {
                    return RedirectToAction("AddNewHome", new { planId = planId, sucess = "0" });
                }
            }
            else
            {
                return RedirectToAction("AddNewHome", new { planId = planId, sucess = "0" });
            }
            //return View(new Home());
        }

        /*
        [Route("{type}/newhomeupdateimage")]
        [HttpPost]
        public ActionResult EditNewHomeImageList(string type, NewHomeViewModel viewModel, string[] imageList, string UniqueId)
        {
            Repositories.Models.NewHome.NewHomeListing newHomeListing = new Repositories.Models.NewHome.NewHomeListing();
            newHomeListing.BuilderNumber = UniqueId;
            List<SubImage> lstSubImage = new List<SubImage>();
            var i = 0;//_newHomeMain.ImageListCount(UniqueId);
            foreach (var item in imageList)
            {
                i++;
                SubImage si = new SubImage();
                si.Text = item;
                si.ReferenceType = "URL";
                si.SequencePosition = i.ToString();
                si.Type = "Standard";
                lstSubImage.Add(si);
            }
            Repositories.Models.NewHome.Subdivision subdivision = new Repositories.Models.NewHome.Subdivision();
            subdivision.SubImage = lstSubImage;
            newHomeListing.Subdivision = subdivision;
            var updateProperty = _newHomeMain.UpdateImageList(newHomeListing);
            return Json(updateProperty);
        }
        */

        [HttpGet]
        [Route("{type}/editnewhome/{mlsid}")]
        public ActionResult EditNewHomePropertyPlan(string type, string mlsid)
        {
            var ListedBy = type == "newhome" ? _newHomeMain.GetListedType(mlsid) : _listHub.GetListedType(type, mlsid);
            ViewBag.ListedBy = ListedBy;
            ManagePropertyViewModel managePropertyViewModel = new ManagePropertyViewModel();
            managePropertyViewModel = _newHomeMain.GetExtraProperty(mlsid);
            if (managePropertyViewModel != null)
            {
                managePropertyViewModel.UniqueId = mlsid;
                managePropertyViewModel.Type = type;
            }
            else
            {
                managePropertyViewModel = new ManagePropertyViewModel { UniqueId = mlsid, Type = type, IsDeleted = false };
            }
            var newHomeViewModel = new NewHomeViewModel();
            newHomeViewModel.CreateNewPlanModels(0);
            newHomeViewModel.ExtProperties = managePropertyViewModel;
            newHomeViewModel.BuilderNumber = mlsid;
            return View(newHomeViewModel);
        }

        [Route("delete-newhome")]
        public JsonResult DeleteNewHome(string planId)
        {
            var retVal = _newHomeMain.DeleteNewHome(planId);
            return Json(retVal);
        }

        /*
        [HttpPost]
        [Route("{type}/editnewhome/{mlsid}")]
        public ActionResult EditNewHomePropertyPlan(string type, NewHomeViewModel newHomeViewModel)
        {
            NewHomeListing newHomeListing = new NewHomeListing();
            Repositories.Models.NewHome.Subdivision subDiv = new Repositories.Models.NewHome.Subdivision();
            List<Plan> lstPlan = new List<Plan>();
            foreach (HomePlan item in newHomeViewModel.PlanModelViewList)
            {
                Plan plan = new Plan();

                plan.PlanNumber = Utility.UtilityClass.GetUniqueKey();
                plan.PlanName = item.PlanName;

                BasePrice basePrice = new BasePrice();
                basePrice.ExcludesLand = item.ExcludesLand;
                basePrice.Text = item.BasePrice;
                plan.BasePrice = basePrice;

                plan.BaseSqft = item.BaseSqft;
                plan.Stories = item.Stories;
                plan.Baths = item.Baths;
                plan.HalfBaths = item.HalfBaths;

                Bedrooms bdRoom = new Bedrooms();
                bdRoom.MasterBedLocation = item.MasterBedLocation;
                bdRoom.Text = item.Bedrooms;
                plan.Bedrooms = bdRoom;

                Garage garage = new Garage();
                garage.Entry = item.Entry;
                garage.Text = item.Garage;
                plan.Garage = garage;

                plan.Basement = item.Basement;

                plan.MarketingHeadline = item.MarketingHeadline;
                plan.Description = item.Description;

                PlanImages planImages = new PlanImages();
                ElevationImage elevationImage = new ElevationImage();
                elevationImage.SequencePosition = "1";
                elevationImage.Title = item.Title;
                elevationImage.Caption = item.Caption;
                elevationImage.ReferenceType = "URL";
                elevationImage.Text = item.ElevationImageText;
                planImages.ElevationImage = elevationImage;
                planImages.PlanViewer = item.PlanViewer;
                planImages.VirtualTour = item.VirtualTour;
                plan.PlanImages = planImages;

                plan.Type = item.Type;
                lstPlan.Add(plan);
            }
            subDiv.Plan = lstPlan;
            newHomeListing.Subdivision = subDiv;
            newHomeListing.BuilderNumber = newHomeViewModel.BuilderNumber;
            bool varResult = _newHomeMain.UpdateNewHomePlanList(newHomeListing);

            var newHome = Mapper.Map<NewHome>(newHomeListing);
            _ElasticSearchIndicesNewHome.CreateIndex(newHome);

            ManagePropertyViewModel managePropertyViewModel = new ManagePropertyViewModel();
            managePropertyViewModel.UniqueId = newHomeListing.BuilderNumber;
            managePropertyViewModel.Type = type;
            managePropertyViewModel = _newHomeMain.GetExtraProperty(newHomeListing.BuilderNumber);
            newHomeViewModel.ExtProperties = managePropertyViewModel == null ? new ManagePropertyViewModel() : managePropertyViewModel;
            ViewBag.PlanNumber = varResult;

            var ListedBy = type == "newhome" ? _newHomeMain.GetListedType(newHomeListing.BuilderNumber) : _listHub.GetListedType(type, newHomeListing.BuilderNumber);
            ViewBag.ListedBy = ListedBy;

            return View(newHomeViewModel);
        }

        [Route("{type}/updatenewhome")]
        [HttpPost]
        public ActionResult EditNewHomeProperty(string type, ManageNewHomePropertyViewModel manageNewHomePropertyViewModel)
        {
            var updateProperty = _newHomeMain.UpdateExtraProperty(manageNewHomePropertyViewModel);
            return Json(updateProperty);
        }

        */

        /*
        [HttpGet]
        [Route("{type}/editplan/{mlsid}")]
        public ActionResult EditPlan(string type, string mlsid)
        {
            Plan p = _newHomeMain.GetPlan(mlsid);
            HomePlan plan = new HomePlan();
            plan.PlanNumber = p.PlanNumber;
            plan.PlanName = p.PlanName;
            plan.BasePrice = p.BasePrice.Text;
            plan.ExcludesLand = p.BasePrice.ExcludesLand;
            plan.BaseSqft = p.BaseSqft;
            plan.Stories = p.Stories;
            plan.Baths = p.Baths;
            plan.HalfBaths = p.HalfBaths;
            plan.Bedrooms = p.Bedrooms.Text;
            plan.MasterBedLocation = p.Bedrooms.MasterBedLocation;
            plan.Garage = p.Garage.Text;
            plan.Entry = p.Garage.Entry;
            plan.Basement = p.Basement;
            plan.MarketingHeadline = p.MarketingHeadline;
            plan.Description = p.Description;
            if (p.PlanImages != null)
            {
                plan.VirtualTour = p.PlanImages.VirtualTour;
                plan.PlanViewer = p.PlanImages.PlanViewer;
                if (p.PlanImages.ElevationImage != null)
                {
                    plan.Caption = p.PlanImages.ElevationImage.Caption;
                    plan.ElevationImageText = p.PlanImages.ElevationImage.Text;
                    plan.Title = p.PlanImages.ElevationImage.Title;
                }
            }

            return View(plan);
        }
        */
        /*
        [HttpPost]
        [Route("{type}/editplan/{mlsid}")]
        public ActionResult EditPlan(string type, HomePlan homePlan)
        {
            Plan plan = new Plan();

            plan.PlanNumber = homePlan.PlanNumber;
            plan.PlanName = homePlan.PlanName;

            BasePrice basePrice = new BasePrice();
            basePrice.ExcludesLand = homePlan.ExcludesLand;
            basePrice.Text = homePlan.BasePrice;
            plan.BasePrice = basePrice;

            plan.BaseSqft = homePlan.BaseSqft;
            plan.Stories = homePlan.Stories;
            plan.Baths = homePlan.Baths;
            plan.HalfBaths = homePlan.HalfBaths;

            Bedrooms bdRoom = new Bedrooms();
            bdRoom.MasterBedLocation = homePlan.MasterBedLocation;
            bdRoom.Text = homePlan.Bedrooms;
            plan.Bedrooms = bdRoom;

            Garage garage = new Garage();
            garage.Entry = homePlan.Entry;
            garage.Text = homePlan.Garage;
            plan.Garage = garage;

            plan.Basement = homePlan.Basement;

            plan.MarketingHeadline = homePlan.MarketingHeadline;
            plan.Description = homePlan.Description;

            PlanImages planImages = new PlanImages();
            ElevationImage elevationImage = new ElevationImage();
            elevationImage.SequencePosition = "1";
            elevationImage.Title = homePlan.Title;
            elevationImage.Caption = homePlan.Caption;
            elevationImage.ReferenceType = "URL";
            elevationImage.Text = homePlan.ElevationImageText;
            planImages.ElevationImage = elevationImage;
            planImages.PlanViewer = homePlan.PlanViewer;
            planImages.VirtualTour = homePlan.VirtualTour;
            plan.PlanImages = planImages;

            plan.Type = homePlan.Type;


            bool varResult = _newHomeMain.UpdateNewHomePlan(plan);
            ViewBag.PlanNumber = varResult;

            #region Elasticsearch
            NewHomeListing newHomeListing = new NewHomeListing();
            newHomeListing.BuilderNumber = _newHomeMain.GetBuilderNo(plan.PlanNumber);
            Repositories.Models.NewHome.Subdivision subDiv = new Repositories.Models.NewHome.Subdivision();
            List<Plan> p = new List<Plan>();
            p.Add(plan);
            subDiv.Plan = p;
            newHomeListing.Subdivision = subDiv;
            var newHome = Mapper.Map<NewHome>(newHomeListing);
            _ElasticSearchIndicesNewHome.UpdateIndex(newHome);
            #endregion

            return View(homePlan);
        }
        */

        #endregion


        [Route("newhome-listing-ajax-handler")]
        public ActionResult NewHomeAjaxHandler(JQueryDataTableParamModel param)
        {
            var email = @User.Email;

            if (User.Roles.Contains("Admin"))
            {
                email = "";
            }
            long filterCount = 0;
            var newHomePropertysearchCriteria = new NewHomesPropertyDataTable()
            {
                isImageSearchable = Convert.ToBoolean(Request["bSearchable_0"]),
                isBuilderNoSearchable = Convert.ToBoolean(Request["bSearchable_1"]),
                isBuilderNameSearchable = Convert.ToBoolean(Request["bSearchable_2"]),
                isPriceHighSearchable = Convert.ToBoolean(Request["bSearchable_3"]),
                isPriceLowSearchable = Convert.ToBoolean(Request["bSearchable_4"]),
                isSqFtHighSearchable = Convert.ToBoolean(Request["bSearchable_5"]),
                isSqFtLowSearchable = Convert.ToBoolean(Request["bSearchable_6"]),
                isStatusSearchable = Convert.ToBoolean(Request["bSearchable_7"]),
                isAddressSearchable = Convert.ToBoolean(Request["bSearchable_8"]),

                isImageSortable = Convert.ToBoolean(Request["bSortable_0"]),
                isBuilderNoSortable = Convert.ToBoolean(Request["bSortable_1"]),
                isBuilderNameSortable = Convert.ToBoolean(Request["bSortable_2"]),
                isPriceHighSortable = Convert.ToBoolean(Request["bSortable_3"]),
                isPriceLowSortable = Convert.ToBoolean(Request["bSortable_4"]),
                isSqFtHighSortable = Convert.ToBoolean(Request["bSortable_5"]),
                isSqFtLowSortable = Convert.ToBoolean(Request["bSortable_6"]),
                isStatusSortable = Convert.ToBoolean(Request["bSortable_7"]),
                isAddressSortable = Convert.ToBoolean(Request["bSortable_8"]),

                sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]),
                sortDirection = Request["sSortDir_0"]
            };


            var filteredData = _newHome.GetDataSet(email, param, newHomePropertysearchCriteria, out filterCount, "rent");

            var totalCount = _newHome.GetTotalCount(email, "rent");

            var result = from c in filteredData
                         select
                             new[]
                    {
                         c.PlanId,
                         c.BuilderNumber,
                         c.BuilderName,
                         string.IsNullOrEmpty(c.Base_price)?"0.00": Convert.ToDouble(c.Base_price).ToString("#,##,##0.00"),
                         string.IsNullOrEmpty(c.Sqft_low)?"0.00": Convert.ToDouble(c.Sqft_low).ToString("#,##,##0.00"),
                         c.Is_active??"",
                         c.Communityaddress == null ? "" :c.Communityaddress
                    };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filterCount,
                aaData = result
            },
                JsonRequestBehavior.AllowGet);
        }

        [Route("home-listing-ajax-handler")]
        public ActionResult HomeAjaxHandler(JQueryDataTableParamModel param, string planid)
        {
            /*
            var email = @User.Email;

            if (User.Roles.Contains("Admin"))
            {
                email = "jenniferb@warmingtongroup.com";
            }
            long filterCount = 0;
            var newHomePropertysearchCriteria = new NewHomesPropertyDataTable()
            {
                isImageSearchable = Convert.ToBoolean(Request["bSearchable_0"]),
                isBuilderNoSearchable = Convert.ToBoolean(Request["bSearchable_1"]),
                isBuilderNameSearchable = Convert.ToBoolean(Request["bSearchable_2"]),
                isPriceHighSearchable = Convert.ToBoolean(Request["bSearchable_3"]),
                isPriceLowSearchable = Convert.ToBoolean(Request["bSearchable_4"]),
                isSqFtHighSearchable = Convert.ToBoolean(Request["bSearchable_5"]),
                isSqFtLowSearchable = Convert.ToBoolean(Request["bSearchable_6"]),
                isStatusSearchable = Convert.ToBoolean(Request["bSearchable_7"]),
                isAddressSearchable = Convert.ToBoolean(Request["bSearchable_8"]),

                isImageSortable = Convert.ToBoolean(Request["bSortable_0"]),
                isBuilderNoSortable = Convert.ToBoolean(Request["bSortable_1"]),
                isBuilderNameSortable = Convert.ToBoolean(Request["bSortable_2"]),
                isPriceHighSortable = Convert.ToBoolean(Request["bSortable_3"]),
                isPriceLowSortable = Convert.ToBoolean(Request["bSortable_4"]),
                isSqFtHighSortable = Convert.ToBoolean(Request["bSortable_5"]),
                isSqFtLowSortable = Convert.ToBoolean(Request["bSortable_6"]),
                isStatusSortable = Convert.ToBoolean(Request["bSortable_7"]),
                isAddressSortable = Convert.ToBoolean(Request["bSortable_8"]),

                sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]),
                sortDirection = Request["sSortDir_0"]
            };

            */
            var plandetails = _newHomeMain.PlanDetails(planid);// GetDataSet(email, param, newHomePropertysearchCriteria, out filterCount, "rent");
            if (plandetails.Homes != null)
            {


                var homeList = plandetails.Homes.Home.Where(m => m.Id != null);

                var totalCount = homeList.Count(); //_newHome.GetTotalCount(email, "rent");

                var result = from c in homeList
                             select new[]
                    {
                        c.Id,
                        Convert.ToDouble(c.Price).ToString("#,##,##0.00") ?? "0.00",
                        c.Property_type,
                        Convert.ToDouble(c.Sqft).ToString("#,##,##0.00") ?? "0.00",
                        c.Baths,
                        c.Bedrooms,
                        c.Address
                    };

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = totalCount,
                    iTotalDisplayRecords = totalCount,
                    aaData = result
                },
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new string[] { }
                },
                                   JsonRequestBehavior.AllowGet);
            }
        }

        /*
        [Route("plan-details/{builderId}")]
        public ActionResult PlanDetails(string builderId)
        {
            var plans = _newHome.GetPlans(builderId);

            return View("PlanDetails", plans);
        }
        */
        [HttpGet]
        [Route("{type}/edit/{mlsid}")]
        public ActionResult EditProperty(string type, string mlsid)
        {
            var extraPropertyDetails = type == "newhome" ? _newHomeMain.GetExtraProperty(mlsid) : _listHub.GetExtraProperty(type, mlsid);
            var ListedBy = type == "newhome" ? _newHomeMain.GetListedType(mlsid) : _listHub.GetListedType(type, mlsid);
            ViewBag.ListedBy = ListedBy;
            if (extraPropertyDetails != null)
            {
                extraPropertyDetails.UniqueId = mlsid;
                extraPropertyDetails.Type = type;
            }
            else
            {
                extraPropertyDetails = new ManagePropertyViewModel { UniqueId = mlsid, Type = type };
            }
            ViewBag.Type = type;
            return View(extraPropertyDetails);
        }

        [Route("{type}/update")]
        [HttpPost]
        public ActionResult EditProperty(string type, ManagePropertyViewModel viewModel)
        {
            var updateProperty = type == "newhome" ? _newHomeMain.SetExtraProperty(viewModel) : _listHub.SetExtraProperty(type, viewModel);
            if (type.ToLower() == "purchase")
            {
                var purchase = _ElasticSearchIndicesPurchase.Get(viewModel.UniqueId);
                if (purchase != null)
                {
                    purchase.IsFeatured = viewModel.IsFeatured;
                    purchase.IsSpotlight = viewModel.IsSpotlight;
                    if (viewModel.DateTimeRanges.Count() > 0)
                    {
                        purchase.DateTimeRanges = new List<DateTimeRange>();
                        viewModel.DateTimeRanges.ForEach(x =>
                        {
                            purchase.DateTimeRanges.Add(x);
                        });

                    }
                    _ElasticSearchIndicesPurchase.UpdateIndex(purchase);
                }
            }
            else if (type.ToLower() == "rent")
            {
                var rent = _ElasticSearchIndicesRental.Get(viewModel.UniqueId);
                if (rent != null)
                {
                    rent.IsFeatured = viewModel.IsFeatured;
                    rent.IsSpotlight = viewModel.IsSpotlight;
                    _ElasticSearchIndicesRental.UpdateIndex(rent);
                }
            }
            else if (type.ToLower() == "newhome")
            {
                //Class is not created for elastic search
                //var newhome = _ElasticSearchIndicesNewHome.Get(viewModel.UniqueId);
                //newhome.IsFeatured = viewModel.IsFeatured;
                //newhome.IsSpotlight = viewModel.IsSpotlight;
                //_ElasticSearchIndicesNewHome.UpdateIndex(newhome);
            }

            return Json(updateProperty);
        }


        [Route("{type}/updateimage")]
        [HttpPost]
        public ActionResult EditImageList(string type, ListHubListing viewModel)
        {
            if (type == "newhome")
            {
                //Repositories.Models.NewHome.NewHomeListing newHomeListing = new Repositories.Models.NewHome.NewHomeListing();
                //newHomeListing.BuilderNumber = viewModel.MlsNumber;
                //List<SubImage> lstSubImage = new List<SubImage>();
                //var i = _newHomeMain.ImageListCount(viewModel.MlsNumber);
                //foreach (var item in viewModel.Photos.Photo)
                //{
                //    i++;
                //    SubImage si = new SubImage();
                //    si.Text = item.MediaURL;
                //    si.ReferenceType = "URL";
                //    si.SequencePosition = i.ToString();
                //    si.Type = "Standard";
                //    lstSubImage.Add(si);
                //}
                //Repositories.Models.NewHome.Subdivision subdivision = new Repositories.Models.NewHome.Subdivision();
                //subdivision.SubImage = lstSubImage;
                //newHomeListing.Subdivision = subdivision;
                //var updateProperty = _newHomeMain.UpdateImageList(newHomeListing);
                //return Json(updateProperty);
            }
            else
            {
                foreach (Photo item in viewModel.Photos.Photo)
                {
                    MediaModificationTimestamp mp = new MediaModificationTimestamp();
                    mp.IsgSecurityClass = "Public";
                    mp.Text = DateTime.Now.ToString();
                    item.MediaModificationTimestamp = mp;
                }
                var updateProperty = _listHub.UpdateImageList(type, viewModel);
                return Json(updateProperty);
            }
            return Json(true);
        }


        [Route("{type}/deleteproperty")]
        [HttpPost]
        public ActionResult DeleteProperty(string type, string MlsNumber, bool isDeleted)
        {
            var updateProperty = type == "newhome" ? _newHomeMain.DeleteProperty(MlsNumber, isDeleted) : _listHub.DeleteProperty(type, MlsNumber, isDeleted);
            return Json(updateProperty);
        }

        #region FileUpload
        private string StorageRoot
        {
            get { return Path.Combine(Server.MapPath("~/Files")); }
        }

        //DONT USE THIS IF YOU NEED TO ALLOW LARGE FILES UPLOADS
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public void Delete(string id)
        {
            var filename = id;
            var filePath = Path.Combine(Server.MapPath("~/Files"), filename);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        //DONT USE THIS IF YOU NEED TO ALLOW LARGE FILES UPLOADS
        [HttpGet]
        public void Download(string id)
        {
            var filename = id;
            var filePath = Path.Combine(Server.MapPath("~/Files"), filename);

            var context = HttpContext;

            if (System.IO.File.Exists(filePath))
            {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            }
            else
                context.Response.StatusCode = 404;
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            var r = new List<ViewDataUploadFilesResult>();

            foreach (string file in Request.Files)
            {
                var statuses = new List<ViewDataUploadFilesResult>();
                var headers = Request.Headers;

                if (string.IsNullOrEmpty(headers["X-File-Name"]))
                {
                    UploadWholeFile(Request, statuses);
                }
                else
                {
                    UploadPartialFile(headers["X-File-Name"], Request, statuses);
                }

                JsonResult result = Json(statuses);
                result.ContentType = "text/plain";

                return result;
            }

            return Json(r);
        }

        private string EncodeFile(string fileName)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName));
        }

        //DONT USE THIS IF YOU NEED TO ALLOW LARGE FILES UPLOADS
        //Credit to i-e-b and his ASP.Net uploader for the bulk of the upload helper methods - https://github.com/i-e-b/jQueryFileUpload.Net
        private void UploadPartialFile(string fileName, HttpRequestBase request, List<ViewDataUploadFilesResult> statuses)
        {
            if (request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var file = request.Files[0];
            var inputStream = file.InputStream;

            var fullName = Path.Combine(StorageRoot, Path.GetFileName(fileName));

            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }
            statuses.Add(new ViewDataUploadFilesResult()
            {
                name = fileName,
                size = file.ContentLength,
                type = file.ContentType,
                url = "/Home/Download/" + fileName,
                delete_url = "/Home/Delete/" + fileName,
                thumbnail_url = @"data:image/png;base64," + EncodeFile(fullName),
                delete_type = "GET",
            });
        }

        //DONT USE THIS IF YOU NEED TO ALLOW LARGE FILES UPLOADS
        //Credit to i-e-b and his ASP.Net uploader for the bulk of the upload helper methods - https://github.com/i-e-b/jQueryFileUpload.Net
        private void UploadWholeFile(HttpRequestBase request, List<ViewDataUploadFilesResult> statuses)
        {
            for (int i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];

                var fullPath = Path.Combine(StorageRoot, Path.GetFileName(file.FileName));

                file.SaveAs(fullPath);

                statuses.Add(new ViewDataUploadFilesResult()
                {
                    name = file.FileName,
                    size = file.ContentLength,
                    type = file.ContentType,
                    url = "/Home/Download/" + file.FileName,
                    delete_url = "/Home/Delete/" + file.FileName,
                    thumbnail_url = @"data:image/png;base64," + EncodeFile(fullPath),
                    delete_type = "GET",
                });
            }
        }


        #endregion

        [Route("{vin}/{imageurl}")]
        [HttpPost]
        public JsonResult DeleteImageList(string vin, string imageurl, string type)
        {
            var url = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority;
            var file = imageurl.Replace(url, "");
            var fullpath = Server.MapPath(file);
            if (Directory.Exists(Path.GetDirectoryName(fullpath)))
            {
                System.IO.File.Delete(fullpath);
            }
            var updateProperty = type == "newhome" ? _newHomeMain.PullImage(imageurl, vin) : _listHub.PullImage(imageurl, vin, type);
            return Json(updateProperty);

        }

        [Route("DeleteNewHomeImageList")]
        [HttpPost]
        public JsonResult DeleteNewHomeImageList(string imageurl, string homeid, string planid)
        {
            var url = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority;
            var file = imageurl.Replace(url, "");
            var fullpath = Server.MapPath(file);
            if (Directory.Exists(Path.GetDirectoryName(fullpath)))
            {
                System.IO.File.Delete(fullpath);
            }
            imageurl = Server.UrlDecode(imageurl);
            var updateProperty = _newHomeMain.PullImageFromHome(imageurl, homeid, planid);
            return Json(updateProperty);
            //return Json(true);
        }
    }
    public class ViewDataUploadFilesResult
    {
        public string name { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string delete_url { get; set; }
        public string thumbnail_url { get; set; }
        public string delete_type { get; set; }
    }
}