using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using Repositories.Interfaces.ListHub;
using Repositories.Interfaces.Mail;
using Repositories.Interfaces.NewHome;
using Repositories.Interfaces.UserContact;
using Repositories.Models.Common;
using Repositories.Models.NewHome;
using Repositories.Models.UserContact;
using UserInterface.Models;
using Repositories.Interfaces.ElasticSearch;
using Repositories.Models.ListHub;

using Configuration;
using Repositories.Interfaces.Admin.Users;
using System.Globalization;
using System.Web;
using Repositories.Interfaces.Community;
using Repositories.Interfaces.Admin.Office;



namespace UserInterface.Controllers
{
    public class IndexController : Controller
    {
        readonly IListHub _realEstaeService;
        readonly IUserContactDetails _userContactDetails;
        readonly ICommunityProvider _community;

        readonly IMailBase _mailService;
        readonly INewHome _newHome;
        readonly IAgent _iAgent;
        readonly IOffice _office;
        readonly IElasticSearchIndices<Repositories.Models.ListHub.Purchase> _ElasticSearchIndicesPurchase;
        readonly IElasticSearchIndices<Repositories.Models.ListHub.Rental> _ElasticSearchIndicesRental;
        readonly IElasticSearchIndices<Repositories.Models.ListHub.NewHome> _ElasticSearchIndicesNewHome;
        #region Index
        public IndexController(IListHub realEstaeService,
            IMailBase mailService,
            IUserContactDetails userContactDetails,
            INewHome newHome,
            IAgent iAgent,
            IOffice iOffice,
            ICommunityProvider community,
            IElasticSearchIndices<Repositories.Models.ListHub.Purchase> ElasticSearchIndicesPurchase,
            IElasticSearchIndices<Repositories.Models.ListHub.Rental> ElasticSearchIndicesRental,
            IElasticSearchIndices<Repositories.Models.ListHub.NewHome> ElasticSearchIndicesNewHome
            )
        {
            _realEstaeService = realEstaeService;
            _mailService = mailService;
            _userContactDetails = userContactDetails;
            _newHome = newHome;
            _iAgent = iAgent;
            _ElasticSearchIndicesPurchase = ElasticSearchIndicesPurchase;
            _ElasticSearchIndicesRental = ElasticSearchIndicesRental;
            _ElasticSearchIndicesNewHome = ElasticSearchIndicesNewHome;
            _community = community;
            _office = iOffice;
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAutoCompleteDataForPurchase(string term)
        {
            string searchText = term.Contains('#') ? term.Substring(1, term.Length - 1) : term;
            var lst = (term.Contains('#') && term.Length > 1) ? _realEstaeService.GetPurchaseMlsNumberList(searchText).Select(m => m.CityWithStateAndPostCode.ToList()).FirstOrDefault() : ((!term.Contains('#')) ? _realEstaeService.GetPurchaseAddressList(searchText).Select(m => m.CityWithStateAndPostCode.ToList()).FirstOrDefault() : null);
            List<string> lstMyList = new List<string>();
            if (lst != null)
            {
                var lst1 = lst.ConvertAll(d => d.ToLower()).Distinct();
                TextInfo myTi = new CultureInfo("en-US", false).TextInfo;
                foreach (var item in lst1)
                {
                    lstMyList.Add(myTi.ToTitleCase(item));
                }
            }
            return Json(lstMyList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAutoCompleteDataForRent(string term)
        {
            string searchText = term.Contains('#') ? term.Substring(1, term.Length - 1) : term;
            var lst = (term.Contains('#') && term.Length > 1) ? _realEstaeService.GetRentMlsNumberList(searchText).Select(m => m.CityWithStateAndPostCode.ToList()).FirstOrDefault() : ((!term.Contains('#')) ? _realEstaeService.GetRentAddressList(searchText).Select(m => m.CityWithStateAndPostCode.ToList()).FirstOrDefault() : null);
            List<string> lstMyList = new List<string>();
            if (lst != null)
            {
                var lst1 = lst.ConvertAll(d => d.ToLower()).Distinct();
                TextInfo myTi = new CultureInfo("en-US", false).TextInfo;
                foreach (var item in lst1)
                {
                    lstMyList.Add(myTi.ToTitleCase(item));
                }
            }
            return Json(lstMyList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAutoCompleteDataForHome(string term)
        {
            string searchText = term.Contains('#') ? term.Substring(1, term.Length - 1) : term;
            var lst = _newHome.GetNewHomeAddressList(searchText).Select(m => m.CityWithStateAndPostCode.ToList()).FirstOrDefault();
            List<string> lstMyList = new List<string>();
            if (lst != null)
            {
                var lst1 = lst.ToList().ConvertAll(d => d.ToLower()).Distinct();
                TextInfo myTi = new CultureInfo("en-US", false).TextInfo;
                foreach (var item in lst1)
                {
                    lstMyList.Add(myTi.ToTitleCase(item));
                }
            }
            return Json(lstMyList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAutoCompleteDataForAgent(string term)
        {
            var lst = _iAgent.GetAgentList(term.Trim()).Select(m => m.CityWithStateAndPostCode.ToList()).FirstOrDefault();
            List<string> lstMyList = new List<string>();
            TextInfo myTi = new CultureInfo("en-US", false).TextInfo;
            if (lst != null)
            {
                lst = lst.ConvertAll(d => d.ToLower()).Distinct().ToList();
                foreach (var item in lst)
                {
                    lstMyList.Add(myTi.ToTitleCase(item));
                }
            }
            return Json(lstMyList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAutoCompleteDataForAll(string term)
        {
            string searchText = term.Contains('#') ? term.Substring(1, term.Length - 1) : term;
            var lstPur = (term.Contains('#') && term.Length > 1) ? _realEstaeService.GetPurchaseMlsNumberList(searchText).Select(m => m.CityWithStateAndPostCode.ToList()).FirstOrDefault() : ((!term.Contains('#')) ? _realEstaeService.GetPurchaseAddressList(searchText).Select(m => m.CityWithStateAndPostCode.ToList()).FirstOrDefault() : null);
            var lstRen = (term.Contains('#') && term.Length > 1) ? _realEstaeService.GetRentMlsNumberList(searchText).Select(m => m.CityWithStateAndPostCode.ToList()).FirstOrDefault() : ((!term.Contains('#')) ? _realEstaeService.GetRentAddressList(searchText).Select(m => m.CityWithStateAndPostCode.ToList()).FirstOrDefault() : null);
            var lstNew = _newHome.GetNewHomeAddressList(searchText).Select(m => m.CityWithStateAndPostCode.ToList()).FirstOrDefault();
            List<string> lstPurchase = new List<string>();
            if (lstPur == null)
            {
                lstPur = new List<string>();
            }
            else
            {
                lstPurchase = lstPur.ConvertAll(d => d.ToLower()).Distinct().ToList();
            }
            List<string> lstRent = new List<string>();
            if (lstRen == null)
            {
                lstRen = new List<string>();
            }
            else
            {
                lstRent = lstRent.ConvertAll(d => d.ToLower()).Distinct().ToList();
            }
            List<string> lstNewHome = new List<string>();
            if (lstNew == null)
            {
                lstNew = new List<string>();
            }
            else
            {
                lstNewHome = lstNew.ConvertAll(d => d.ToLower()).Distinct().ToList();
            }
            var lst = lstPurchase.Concat(lstRent).Concat(lstNewHome).Distinct();
            List<string> lstMyList = new List<string>();
            TextInfo myTi = new CultureInfo("en-US", false).TextInfo;
            foreach (var item in lst)
            {
                lstMyList.Add(myTi.ToTitleCase(item));
            }
            return Json(lstMyList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PropertyList
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult PropertiesListing(string searchTerm, string Command, string SearchType, string MinPrice,
            string MaxPrice, string NoOfBeds, string NoOfBathRooms, string Size, string LotSize, string HomeAge,
            string PropertyType, string SubProperty, string Distance, string SortBy, string SubCommand, string SubTab,
            string KeyWords, string Location, string Suffix, string ViewStyle, string loadCount,
            bool ByMiles = true, int page = 1, int pageSize = 10, bool IsMls = false)
        {

            List<PropertyListing> lstPListing = new List<PropertyListing>();
            List<PropertyListing> isFeaturedPListing = new List<PropertyListing>();
            List<NearbyArea> nearByLcation = new List<NearbyArea>();
            List<PropertyTypeCheckBox> lstPropertyType = new List<PropertyTypeCheckBox>();
            List<SubPropertyTypeCheckBox> lstSubPropertyType = new List<SubPropertyTypeCheckBox>();
            PropertyListingViewModel propertyListingViewModel = new PropertyListingViewModel();
            int startIndex = (page - 1) * pageSize;
            int limit = pageSize;
            double lati = 0;
            double longi = 0;
            int count = 0;
            int isFeaturedCount = 0;
            AdvanceSearch advSearch = new AdvanceSearch();

            advSearch.RadioByMiles = ByMiles;
            //if (Command != "SearchByAgent")
            //{
            if (IsMls)
            {
                advSearch.MlsNumber = searchTerm;
                advSearch.SearchTerm = searchTerm;
            }
            else
            {
                advSearch.SearchTerm = searchTerm;
            }
            //}
            if (Command == "SearchByAgent")
            {
                advSearch.AgentName = searchTerm;
            }
            advSearch.Location = Location;
            advSearch.MinPrice = MinPrice;
            advSearch.MaxPrice = MaxPrice;
            advSearch.NoOfBeds = NoOfBeds;
            advSearch.NoOfBathroom = NoOfBathRooms;
            advSearch.Size = Size;
            advSearch.LotSize = LotSize;
            if (!string.IsNullOrEmpty(HomeAge) && HomeAge.Contains('_'))
            {
                advSearch.HomeAge = HomeAge.Split('_')[1];
            }
            else
            {
                advSearch.HomeAge = HomeAge;
            }
            List<string> lstStr = new List<string>();
            if (!string.IsNullOrEmpty(PropertyType) && PropertyType.Contains(','))
            {
                foreach (string item in PropertyType.Split(','))
                {
                    lstStr.Add(item);
                }
            }
            else
            {
                lstStr.Add(PropertyType);
            }
            advSearch.SelectedProperty = lstStr;

            List<string> lstSubPropStr = new List<string>();
            string a = HttpUtility.HtmlDecode(SubProperty);
            if (!string.IsNullOrEmpty(SubProperty) && SubProperty.Contains(','))
            {
                foreach (string item in SubProperty.Split(','))
                {
                    lstSubPropStr.Add(item.Contains("%20") ? item.Replace("%20", " ") : item);
                }
            }
            else
            {
                lstSubPropStr.Add(SubProperty);
            }
            advSearch.SelectedSubProperty = lstSubPropStr;

            advSearch.SortBy = string.IsNullOrEmpty(SortBy) ? "0" : SortBy;
            advSearch.NearByDistance = string.IsNullOrEmpty(Distance) || Distance == "undefined" ? 0 : Convert.ToDouble(Distance);
            advSearch.KeyWards = KeyWords;
            advSearch.Suffix = Suffix;
            Session["advSearch"] = advSearch;


            if (Command == "All")
            {
                if (SubCommand == "Rent")
                {
                    lstPListing = _ElasticSearchIndicesRental.SearchQuery(advSearch, startIndex, limit, out count);
                    lstPropertyType = _realEstaeService.GetRentPropertyType().ToList();
                    lstSubPropertyType = _realEstaeService.GetRentSubPropertyType().ToList();
                    if (lstPListing.Count > 0)
                    {
                        lati = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[0]);
                        longi = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[1]);
                        nearByLcation = _realEstaeService.GetNearByRentAreaDetails(longi, lati, 40233.6).OrderBy(m => m.AvgDistance).ToList();
                    }
                    isFeaturedPListing = _ElasticSearchIndicesRental.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedCount);
                }
                /*
                else if (SubCommand == "NewHome")
                {
                    lstPListing = _ElasticSearchIndicesNewHome.SearchQuery(advSearch, startIndex, limit, out count);
                    foreach (PropertyListing item in lstPListing)
                    {
                        //item.MlsNumber = item.objId;
                        item.NoOfBedRooms = item.Plans.Max(x => x.Bedrooms.Text);
                        item.NoOfBathRooms = item.Plans.Max(x => x.Baths);
                        item.NoOfHalfBathRooms = item.Plans.Max(x => x.HalfBaths);
                        item.Price = item.Plans.Max(x => x.BasePrice.Text);
                        item.LivingArea = Convert.ToDouble(item.Plans.Max(x => x.BaseSqft));
                        foreach (Plan item1 in item.Plans)
                        {
                            item.PropertyType = item1.Type;
                        }
                    }
                    lstPropertyType = _newHome.GetNewHomePropertyType().ToList();
                    if (lstPListing.Count > 0)
                    {
                        lati = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[0]);
                        longi = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[1]);
                        nearByLcation = _newHome.GetNearByNewHomeAreaDetails(longi, lati, 40233.6).OrderBy(m => m.AvgDistance).ToList();
                    }

                    #region Featured
                    isFeaturedPListing = _ElasticSearchIndicesNewHome.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedCount);
                    foreach (PropertyListing item in isFeaturedPListing)
                    {
                        //item.MlsNumber = item.objId;
                        if (item.Plans.Count > 0)
                        {
                            item.NoOfBedRooms = item.Plans.Max(x => x.Bedrooms.Text);
                            item.NoOfBathRooms = item.Plans.Max(x => x.Baths);
                            item.NoOfHalfBathRooms = item.Plans.Max(x => x.HalfBaths);
                            item.Price = item.Plans.Max(x => x.BasePrice.Text);
                            item.LivingArea = Convert.ToDouble(item.Plans.Max(x => x.BaseSqft));
                            foreach (Plan item1 in item.Plans)
                            {
                                item.PropertyType = item1.Type;
                            }
                        }
                        else
                        {
                            item.NoOfBedRooms = 0;
                            item.NoOfBathRooms = 0;
                            item.NoOfHalfBathRooms = 0;
                            item.Price = 0;
                            item.LivingArea = 0;
                            //item.PropertyType = null;
                        }
                    }
                    #endregion
                }
                    */
                else if (SubCommand == "OpenHouse")
                {
                    #region Purchase
                    int pcount = 0;

                    List<PropertyListing> lstPurchase = _ElasticSearchIndicesPurchase.SearchQuery(advSearch, startIndex, limit, out pcount);
                    List<PropertyTypeCheckBox> lstPType = _realEstaeService.GetPurchasePropertyType().ToList();
                    List<NearbyArea> nearByP = new List<NearbyArea>();
                    if (lstPurchase.Count > 0)
                    {
                        lati = lstPurchase.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[0]);
                        longi = lstPurchase.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[1]);
                        nearByP = _realEstaeService.GetNearByPurchaseAreaDetails(longi, lati, 40233.6).OrderBy(m => m.AvgDistance).ToList();
                    }

                    int isFeaturedPCount = 0;
                    List<PropertyListing> lstIsFeaturedPListing = _ElasticSearchIndicesPurchase.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedPCount);
                    #endregion

                    #region Rent
                    int rcount = 0;
                    List<PropertyListing> lstRent = _ElasticSearchIndicesRental.SearchQuery(advSearch, startIndex, limit, out rcount);
                    List<PropertyTypeCheckBox> lstRType = _realEstaeService.GetRentPropertyType().ToList();
                    List<NearbyArea> nearByR = new List<NearbyArea>();
                    if (lstRent.Count > 0)
                    {
                        lati = lstRent.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[0]);
                        longi = lstRent.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[1]);
                        nearByR = _realEstaeService.GetNearByRentAreaDetails(longi, lati, 40233.6).OrderBy(m => m.AvgDistance).ToList();
                    }

                    int isFeaturedRCount = 0;
                    List<PropertyListing> lstIsFeaturedRListing = _ElasticSearchIndicesRental.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedRCount);
                    #endregion

                    #region NewHome
                    int ncount = 0;
                    List<PropertyListing> lstNew = _ElasticSearchIndicesNewHome.SearchQuery(advSearch, startIndex, limit, out ncount);
                    foreach (PropertyListing item in lstNew)
                    {
                        //item.MlsNumber = item.objId;
                        if (item.Plans.Count > 0)
                        {
                            /*
                            item.NoOfBedRooms = item.Plans.Max(x => x.Bedrooms.Text);
                            item.NoOfBathRooms = item.Plans.Max(x => x.Baths);
                            item.NoOfHalfBathRooms = item.Plans.Max(x => x.HalfBaths);
                            item.Price = item.Plans.Max(x => x.BasePrice.Text);
                            item.LivingArea = Convert.ToDouble(item.Plans.Max(x => x.BaseSqft));
                            foreach (Plan item1 in item.Plans)
                            {
                                item.PropertyType = item1.Type;
                            }
                             * */
                        }
                        else
                        {
                            item.NoOfBedRooms = 0;
                            item.NoOfBathRooms = 0;
                            item.NoOfHalfBathRooms = 0;
                            item.Price = 0;
                            item.LivingArea = 0;
                            //item.PropertyType = null;
                        }
                    }
                    List<PropertyTypeCheckBox> lstNType = _newHome.GetNewHomePropertyType().ToList();
                    List<NearbyArea> nearByN = new List<NearbyArea>();
                    if (lstNew.Count > 0)
                    {
                        lati = lstNew.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[0]);
                        longi = lstNew.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[1]);
                        nearByN = _newHome.GetNearByNewHomeAreaDetails(longi, lati, 40233.6).OrderBy(m => m.AvgDistance).ToList();
                    }
                    #region Featured
                    int isFeaturedNCount = 0;
                    List<PropertyListing> lstIsFeaturedNListing = _ElasticSearchIndicesNewHome.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedNCount);
                    foreach (PropertyListing item in lstIsFeaturedNListing)
                    {
                        //item.MlsNumber = item.objId;
                        if (item.Plans.Count > 0)
                        {
                            /*
                            item.NoOfBedRooms = item.Plans.Max(x => x.Bedrooms.Text);
                            item.NoOfBathRooms = item.Plans.Max(x => x.Baths);
                            item.NoOfHalfBathRooms = item.Plans.Max(x => x.HalfBaths);
                            item.Price = item.Plans.Max(x => x.BasePrice.Text);
                            item.LivingArea = Convert.ToDouble(item.Plans.Max(x => x.BaseSqft));
                            foreach (Plan item1 in item.Plans)
                            {
                                item.PropertyType = item1.Type;
                            }
                             * */
                        }
                        else
                        {
                            item.NoOfBedRooms = 0;
                            item.NoOfBathRooms = 0;
                            item.NoOfHalfBathRooms = 0;
                            item.Price = 0;
                            item.LivingArea = 0;
                            //item.PropertyType = null;
                        }
                    }

                    #endregion
                    #endregion

                    lstPListing = (lstPurchase == null ? new List<PropertyListing>() : lstPurchase).Concat(lstRent == null ? new List<PropertyListing>() : lstRent).Concat(lstNew == null ? new List<PropertyListing>() : lstNew).Distinct().ToList();
                    count = pcount + rcount + ncount;
                    lstPropertyType = (lstPType == null ? new List<PropertyTypeCheckBox>() : lstPType).Concat(lstRType == null ? new List<PropertyTypeCheckBox>() : lstRType).Concat(lstNType == null ? new List<PropertyTypeCheckBox>() : lstNType).Distinct().ToList();
                    nearByLcation = (nearByP == null ? new List<NearbyArea>() : nearByP).Concat(nearByR == null ? new List<NearbyArea>() : nearByR).Concat(nearByN == null ? new List<NearbyArea>() : nearByN).Distinct().ToList();
                    isFeaturedPListing = (isFeaturedPListing == null ? new List<PropertyListing>() : isFeaturedPListing).Concat(lstIsFeaturedPListing == null ? new List<PropertyListing>() : lstIsFeaturedPListing).Concat(lstIsFeaturedNListing == null ? new List<PropertyListing>() : lstIsFeaturedNListing).Distinct().ToList();
                    isFeaturedCount = isFeaturedPCount + isFeaturedRCount + isFeaturedNCount;
                }
                else
                {
                    lstPListing = _ElasticSearchIndicesPurchase.SearchQuery(advSearch, startIndex, limit, out count);
                    lstPropertyType = _realEstaeService.GetPurchasePropertyType().ToList();
                    if (lstPListing.Count > 0)
                    {
                        lati = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[0]);
                        longi = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[1]);
                        nearByLcation = _realEstaeService.GetNearByPurchaseAreaDetails(longi, lati, 40233.6).OrderBy(m => m.AvgDistance).ToList();
                    }
                    isFeaturedPListing = _ElasticSearchIndicesPurchase.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedCount);
                }
            }
            else if (Command == "SearchPurchase")
            {
                lstPListing = _ElasticSearchIndicesPurchase.SearchQuery(advSearch, startIndex, limit, out count);
                lstPropertyType = _realEstaeService.GetPurchasePropertyType().ToList();
                lstSubPropertyType = _realEstaeService.GetPurchaseSubPropertyType().ToList();
                if (lstPListing.Count > 0)
                {
                    lati = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[0]);
                    longi = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[1]);
                    nearByLcation = _realEstaeService.GetNearByPurchaseAreaDetails(longi, lati, 40233.6).OrderBy(m => m.AvgDistance).ToList();
                }
                isFeaturedPListing = _ElasticSearchIndicesPurchase.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedCount);
            }
            else if (Command == "SearchRent")
            {
                lstPListing = _ElasticSearchIndicesRental.SearchQuery(advSearch, startIndex, limit, out count);
                lstPropertyType = _realEstaeService.GetRentPropertyType().ToList();
                lstSubPropertyType = _realEstaeService.GetRentSubPropertyType().ToList();
                if (lstPListing.Count > 0)
                {
                    lati = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[0]);
                    longi = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[1]);
                    nearByLcation = _realEstaeService.GetNearByRentAreaDetails(longi, lati, 40233.6).OrderBy(m => m.AvgDistance).ToList();
                }
                isFeaturedPListing = _ElasticSearchIndicesRental.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedCount);
            }

            else if (Command == "SearchOpenHouse")
            {
                //this is required for OpenHouse
                lstPListing = _ElasticSearchIndicesPurchase.SearchQuery(advSearch, startIndex, limit, out count, true);
                lstPropertyType = _realEstaeService.GetPurchasePropertyType().ToList();
                if (lstPListing.Count > 0)
                {
                    lati = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[0]);
                    longi = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[1]);
                    nearByLcation = _realEstaeService.GetNearByPurchaseAreaDetails(longi, lati, 40233.6).OrderBy(m => m.AvgDistance).ToList();
                }

                isFeaturedPListing = _ElasticSearchIndicesPurchase.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedCount);
            }
            else if (Command == "SearchHome")
            {
                lstPListing = _ElasticSearchIndicesNewHome.SearchQuery(advSearch, startIndex, limit, out count);
                foreach (var lst in lstPListing)
                {
                    var obj = lst as NewHome;
                    lst.CommunityName = new[] { obj.CommunityName };

                }
                lstPropertyType = _newHome.GetNewHomePropertyType().ToList();
                lstSubPropertyType = _newHome.GetNewHomeSubPropertyType().ToList();
                if (lstPListing.Count > 0)
                {
                    lati = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[0]);
                    longi = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[1]);
                    nearByLcation = _newHome.GetNearByNewHomeAreaDetails(longi, lati, 40233.6).OrderBy(m => m.AvgDistance).ToList();
                }
                #region Featured
                isFeaturedPListing = _ElasticSearchIndicesNewHome.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedCount);

                #endregion
            }
            else if (Command == "SearchByAgent")
            {
                lstPListing = _ElasticSearchIndicesPurchase.SearchQuery(advSearch, startIndex, limit, out count);
                lstPropertyType = _realEstaeService.GetRentPropertyType().ToList();
                if (lstPListing.Count > 0)
                {
                    lati = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[0]);
                    longi = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[1]);
                    nearByLcation = _realEstaeService.GetNearByRentAreaDetails(longi, lati, 40233.6).OrderBy(m => m.AvgDistance).ToList();
                }
                isFeaturedPListing = _ElasticSearchIndicesPurchase.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedCount);
            }

            ViewBag.SearchValue = advSearch.Location;
            ViewBag.Page = page;
            ViewBag.startIndex = startIndex;
            ViewBag.IsMls = IsMls;
            //if (loadCount == "1")
            //{
            //    foreach (var item in lstPropertyType)
            //    {
            //        foreach (var item1 in lstStr)
            //        {
            //            item.IsSelected = true;
            //        }
            //    }

            //    foreach (var item in lstSubPropertyType)
            //    {
            //        foreach (var item1 in lstSubPropStr)
            //        {
            //            item.IsSelected = true;
            //        }
            //    }
            //}
            //else
            //{
            //    foreach (var item in lstPropertyType)
            //    {
            //        foreach (var item1 in lstStr)
            //        {
            //            if (item.PropertyName == item1)
            //            {
            //                item.IsSelected = true;
            //            }
            //        }
            //    }

            //    foreach (var item in lstSubPropertyType)
            //    {
            //        foreach (var item1 in lstSubPropStr)
            //        {
            //            if (item.SubPropertyName == item1)
            //            {
            //                item.IsSelected = true;
            //            }
            //        }
            //    }
            //}

            if (lstStr.Count() == 1 && lstStr[0] == null)
            {
                foreach (var item in lstPropertyType)
                {
                    item.IsSelected = true;
                }
            }
            else
            {
                foreach (var item in lstPropertyType)
                {
                    foreach (var item1 in lstStr)
                    {
                        if (item.PropertyName == item1)
                        {
                            item.IsSelected = true;
                        }
                    }
                }
            }

            if (lstSubPropStr.Count() == 1 && lstSubPropStr[0] == null)
            {
                foreach (var item in lstSubPropertyType)
                {
                    item.IsSelected = true;
                }
            }
            else
            {
                foreach (var item in lstSubPropertyType)
                {
                    foreach (var item1 in lstSubPropStr)
                    {
                        if (item.SubPropertyName == item1)
                        {
                            item.IsSelected = true;
                        }
                    }
                }
            }
            propertyListingViewModel.PropertyType = lstPropertyType;
            propertyListingViewModel.SubPropertyType = lstSubPropertyType;
            propertyListingViewModel.NearbyAreas = nearByLcation;
            ViewBag.Command = Command;
            ViewBag.SubCommand = SubCommand;
            ViewBag.SubTab = SubTab;
            propertyListingViewModel.Command = ViewBag.Command;
            propertyListingViewModel.SearchValue = searchTerm;
            foreach (var item in lstPListing)
            {
                if (!string.IsNullOrEmpty(item.ProviderName))
                {
                    var offc = _office.GetOfficeDetailsByName(item.ProviderName);
                    if (offc != null && !string.IsNullOrEmpty(offc.OfficeImageUrl))
                    {
                        item.BrokerageLogoUrl = offc.OfficeImageUrl;
                    }
                }


                if (!string.IsNullOrEmpty(item.AgentName))
                {
                    var agent = _iAgent.GetAgentDetailsByName(item.AgentName);

                    if (agent != null)
                    {
                        item.IsAgentFeauterd = agent.IsFeatured;
                    }
                }
            }
            propertyListingViewModel.PropertyListings = new StaticPagedList<PropertyListing>(lstPListing, page, pageSize, count);
            propertyListingViewModel.FeaturedPropertyListings = new StaticPagedList<PropertyListing>(isFeaturedPListing, page, pageSize, isFeaturedCount);
            propertyListingViewModel.advSearch = advSearch;
            ViewBag.Count = count;
            ViewBag.FeaturedCount = isFeaturedCount;
            ViewBag.PropertyCount = lstPropertyType.Count(m => m.IsSelected == true);
            ViewBag.SubPropertyCount = lstSubPropertyType.Count(m => m.IsSelected == true);
            return View(ViewStyle, propertyListingViewModel);
        }

        #endregion

        #region PropertyDetails
        public ActionResult PropertieDetails(string mlsNumber, string Command, string SubCommand, string SelectedRow, string NavigationType, int RecordIndex, int TotalCount, string ListPageUrl)
        {
            PropertyDetails propertyDetails = new PropertyDetails();
            AdvanceSearch advSearch = new AdvanceSearch();
            advSearch = (AdvanceSearch)Session["advSearch"];
            List<string> lst = new List<string>();
            int index = RecordIndex;
            if (Command == "SearchPurchase")
            {
                if (NavigationType == "P")
                {
                    index = RecordIndex - 1;
                    mlsNumber = index < 0 ? mlsNumber : _ElasticSearchIndicesPurchase.GetMlsNumber(advSearch, index);
                }
                else if (NavigationType == "N")
                {
                    index = RecordIndex + 1;
                    mlsNumber = index == TotalCount ? mlsNumber : _ElasticSearchIndicesPurchase.GetMlsNumber(advSearch, index);
                }
                propertyDetails = _realEstaeService.GetPurchaseRecordDetailsByMLSNumber(mlsNumber);
            }
            else if (Command == "SearchRent")
            {

                if (NavigationType == "P")
                {
                    index = RecordIndex - 1;
                    mlsNumber = index < 0 ? mlsNumber : _ElasticSearchIndicesRental.GetMlsNumber(advSearch, index);
                }
                else if (NavigationType == "N")
                {
                    index = RecordIndex + 1;
                    mlsNumber = index == TotalCount ? mlsNumber : _ElasticSearchIndicesRental.GetMlsNumber(advSearch, index);
                }
                propertyDetails = _realEstaeService.GetRentRecordDetailsByMLSNumber(mlsNumber);
            }
            else if (Command == "SearchByAgent")
            {
                if (NavigationType == "P")
                {
                    index = RecordIndex - 1;
                    mlsNumber = index < 0 ? mlsNumber : _ElasticSearchIndicesPurchase.GetMlsNumber(advSearch, index);
                }
                else if (NavigationType == "N")
                {
                    index = RecordIndex + 1;
                    mlsNumber = index == TotalCount ? mlsNumber : _ElasticSearchIndicesPurchase.GetMlsNumber(advSearch, index);
                }
                propertyDetails = _realEstaeService.GetPurchaseRecordDetailsByMLSNumber(mlsNumber);
            }
            else if (Command == "SearchHome")
            {
                if (NavigationType == "P")
                {
                    index = RecordIndex - 1;
                    mlsNumber = index < 0 ? mlsNumber : _ElasticSearchIndicesNewHome.GetMlsNumber(advSearch, index);
                }
                else if (NavigationType == "N")
                {
                    index = RecordIndex + 1;
                    mlsNumber = index == TotalCount ? mlsNumber : _ElasticSearchIndicesNewHome.GetMlsNumber(advSearch, index);
                }
                propertyDetails = _newHome.GetNewHomeRecordDetailsByMLSNumber(mlsNumber);
                //if (propertyDetails.Plans.Count > 0)
                //{
                //    foreach (Plan item in propertyDetails.Plans)
                //    {
                //        /*
                //        if (item.PlanNumber == SelectedRow)
                //        {
                //            propertyDetails.Bedrooms = item.Bedrooms.Text;
                //            propertyDetails.Bathrooms = item.Baths;
                //            propertyDetails.FullBathrooms = item.Baths;
                //            propertyDetails.HalfBathrooms = item.HalfBaths;
                //            propertyDetails.ListPrice = item.BasePrice.Text;
                //            //.ToString("#,#", CultureInfo.InvariantCulture);
                //            propertyDetails.LivingArea = Convert.ToDouble(item.BaseSqft);
                //            List<string> lstStr = new List<string>();
                //            foreach (var item1 in item.PlanAmenity)
                //            {
                //                lstStr.Add(item1.Text + " " + item1.Type);
                //            }
                //            propertyDetails.Appliance = lstStr.ToArray();
                //            propertyDetails.ParkingType = new string[] { item.Garage.Text + " " + item.Garage.Entry };
                //            propertyDetails.NoOfParkingSpace = Convert.ToString(item.Garage.Text);
                //            propertyDetails.ListingDescription = item.Description;
                //            propertyDetails.PropertyType = item.Type;
                //            propertyDetails.ElevationImages = item.PlanImages.ElevationImage == null ? "" : item.PlanImages.ElevationImage.Text;
                //            propertyDetails.VirtualTour = item.PlanImages.VirtualTour;
                //            propertyDetails.PlanViewer = item.PlanImages.PlanViewer;
                //            break;
                //        }
                //         * */
                //    }
                //}
            }
            else
            {
                if (SubCommand == "Purchase")
                {
                    if (NavigationType == "P")
                    {
                        index = RecordIndex - 1;
                        mlsNumber = index < 0 ? mlsNumber : _ElasticSearchIndicesPurchase.GetMlsNumber(advSearch, index);
                    }
                    else if (NavigationType == "N")
                    {
                        index = RecordIndex + 1;
                        mlsNumber = index == TotalCount ? mlsNumber : _ElasticSearchIndicesPurchase.GetMlsNumber(advSearch, index);
                    }
                    propertyDetails = _realEstaeService.GetPurchaseRecordDetailsByMLSNumber(mlsNumber);
                }
                else if (SubCommand == "Rent")
                {
                    if (NavigationType == "P")
                    {
                        index = RecordIndex - 1;
                        mlsNumber = index < 0 ? mlsNumber : _ElasticSearchIndicesRental.GetMlsNumber(advSearch, index);
                    }
                    else if (NavigationType == "N")
                    {
                        index = RecordIndex + 1;
                        mlsNumber = index == TotalCount ? mlsNumber : _ElasticSearchIndicesRental.GetMlsNumber(advSearch, index);
                    }
                    propertyDetails = _realEstaeService.GetRentRecordDetailsByMLSNumber(mlsNumber);
                }
                else
                {
                    if (NavigationType == "P")
                    {
                        index = RecordIndex - 1;
                        mlsNumber = index < 0 ? mlsNumber : _ElasticSearchIndicesNewHome.GetMlsNumber(advSearch, index);
                    }
                    else if (NavigationType == "N")
                    {
                        index = RecordIndex + 1;
                        mlsNumber = index == TotalCount ? mlsNumber : _ElasticSearchIndicesNewHome.GetMlsNumber(advSearch, index);
                    }
                    propertyDetails = _newHome.GetNewHomeRecordDetailsByMLSNumber(mlsNumber);


                }
            }
            ViewBag.PageIndex = index < 0 ? 0 : index;
            ViewBag.MlsNumber = mlsNumber;
            ViewBag.TotalCount = TotalCount;
            ViewBag.Command = Command;
            var agent = NinjectConfig.Get<IAgent>();
            Repositories.Models.Admin.User.User u = agent.IsAgentFeatured(propertyDetails.AgentEmail);

            Office office = _office.GetOfficeDetails(propertyDetails.OfficeId);

            if (office != null)
            {
                if (!string.IsNullOrEmpty(office.OfficeImageUrl))
                {
                    propertyDetails.BrokerageLogoURL = office.OfficeImageUrl;

                }
            }
            propertyDetails.LogoImage = u.LogoImage;
            propertyDetails.WebsiteUrl = u.WebsiteURL;
            ViewBag.IsFeatured = u.IsFeatured;
            ViewBag.OfficePhone = u.OfficePhone;
            ViewBag.PrimaryContactPhone = u.PrimaryContactPhone;
            string url = HttpUtility.UrlDecode(ListPageUrl);
            if (Command != "SearchHome")
            {
                url = url.Replace("searchTerm=" + advSearch.SearchTerm, "searchTerm=" + propertyDetails.AgentName);
                url = url.Replace("IsMls=true", "IsMls=false");
            }
            ViewBag.ListPageUrl = url;
            return View(propertyDetails);
        }

        public ActionResult PropertieDetailsWizard(string mlsNumber, string Command)
        {
            PropertyDetails propertyDetails = new PropertyDetails();
            List<string> lst = new List<string>();
            if (Command == "SearchPurchase")
            {
                propertyDetails = _realEstaeService.GetPurchaseRecordDetailsByMLSNumber(mlsNumber);
            }
            else if (Command == "SearchRent")
            {
                propertyDetails = _realEstaeService.GetRentRecordDetailsByMLSNumber(mlsNumber);
            }
            else if (Command == "SearchByAgent")
            {

                propertyDetails = _realEstaeService.GetPurchaseRecordDetailsByMLSNumber(mlsNumber);
            }
            else if (Command == "SearchHome")
            {

                propertyDetails = _newHome.GetNewHomeRecordDetailsByMLSNumber(mlsNumber);
            }

            //ViewBag.PageIndex = index < 0 ? 0 : index;
            ViewBag.MlsNumber = mlsNumber;
            //ViewBag.TotalCount = TotalCount;
            ViewBag.Command = Command;
            var agent = NinjectConfig.Get<IAgent>();
            Repositories.Models.Admin.User.User u = agent.IsAgentFeatured(propertyDetails.AgentEmail);
            propertyDetails.LogoImage = u.LogoImage;
            propertyDetails.WebsiteUrl = u.WebsiteURL;
            ViewBag.IsFeatured = u.IsFeatured;
            ViewBag.OfficePhone = u.OfficePhone;
            ViewBag.PrimaryContactPhone = u.PrimaryContactPhone;

            return View(propertyDetails);
        }

        #endregion

        #region AdvanceSearch

        [HttpGet]
        public JsonResult AdvanceGetGeoLocation(string searchTerm, string Command, string SearchType, string MinPrice, string MaxPrice, string NoOfBeds, string NoOfBathRooms, string Size, string LotSize, string HomeAge, string PropertyType, string SubCommand, string SubTab, int page = 1, int pageSize = 10, bool IsMls = false)
        {
            List<PropertyListing> lstPListing = new List<PropertyListing>();
            int count = 0;
            int startIndex = (page - 1) * pageSize;
            int limit = pageSize;
            int isFeaturedPCount = 0;
            int isFeaturedRCount = 0;
            int isFeaturedNCount = 0;

            AdvanceSearch advSearch = new AdvanceSearch();
            advSearch = (AdvanceSearch)Session["advSearch"];
            if (Command == "All")
            {
                if (SubCommand == "Rent")
                {
                    if (SubTab == "Featured")
                    {
                        lstPListing = _ElasticSearchIndicesRental.SearchFeaturedQuery(advSearch, startIndex, limit, out count);
                    }
                    else
                    {
                        lstPListing = _ElasticSearchIndicesRental.SearchQuery(advSearch, startIndex, limit, out count);
                    }
                }
                else if (SubCommand == "NewHome")
                {
                    if (SubTab == "Featured")
                    {
                        lstPListing = _ElasticSearchIndicesNewHome.SearchFeaturedQuery(advSearch, startIndex, limit, out count);
                        //foreach (var item in lstPListing)
                        //{
                        //    //item.MlsNumber = item.objId;
                        //}
                    }
                    else
                    {
                        lstPListing = _ElasticSearchIndicesNewHome.SearchQuery(advSearch, startIndex, limit, out count);
                        //foreach (var item in lstPListing)
                        //{
                        //    //item.MlsNumber = item.objId;
                        //}
                    }
                }
                else if (SubCommand == "OpenHouse")
                {
                    if (SubTab == "Featured")
                    {
                        
                        List<PropertyListing> lstIsFeaturedPListing = _ElasticSearchIndicesPurchase.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedPCount);
                        List<PropertyListing> lstIsFeaturedRListing = _ElasticSearchIndicesRental.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedRCount);
                        List<PropertyListing> lstIsFeaturedNListing = _ElasticSearchIndicesNewHome.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedNCount);

                        foreach (PropertyListing item in lstIsFeaturedNListing)
                        {
                            //item.MlsNumber = item.objId;
                            if (item.Plans.Count > 0)
                            {
                                /*
                                item.NoOfBedRooms = item.Plans.Max(x => x.Bedrooms.Text);
                                item.NoOfBathRooms = item.Plans.Max(x => x.Baths);
                                item.NoOfHalfBathRooms = item.Plans.Max(x => x.HalfBaths);
                                item.Price = item.Plans.Max(x => x.BasePrice.Text);
                                item.LivingArea = Convert.ToDouble(item.Plans.Max(x => x.BaseSqft));
                                foreach (Plan item1 in item.Plans)
                                {
                                    item.PropertyType = item1.Type;
                                }
                                 * */
                            }
                            else
                            {
                                item.NoOfBedRooms = 0;
                                item.NoOfBathRooms = 0;
                                item.NoOfHalfBathRooms = 0;
                                item.Price = 0;
                                item.LivingArea = 0;
                                //item.PropertyType = null;
                            }
                        }
                        lstPListing = (lstIsFeaturedPListing == null ? new List<PropertyListing>() : lstIsFeaturedPListing).Concat(lstIsFeaturedPListing == null ? new List<PropertyListing>() : lstIsFeaturedPListing).Concat(lstIsFeaturedNListing == null ? new List<PropertyListing>() : lstIsFeaturedNListing).Distinct().ToList();
                        foreach (var item in lstPListing)
                        {
                            //item.MlsNumber = item.objId;
                        }
                        count = isFeaturedRCount + isFeaturedPCount + isFeaturedNCount;
                    }
                    else
                    {
                        int pcount = 0;
                        int rcount = 0;
                        int ncount = 0;
                        List<PropertyListing> lstPurchase = _ElasticSearchIndicesPurchase.SearchQuery(advSearch, startIndex, limit, out pcount);
                        List<PropertyListing> lstRent = _ElasticSearchIndicesRental.SearchQuery(advSearch, startIndex, limit, out rcount);
                        List<PropertyListing> lstNew = _ElasticSearchIndicesNewHome.SearchQuery(advSearch, startIndex, limit, out ncount);
                        foreach (PropertyListing item in lstNew)
                        {
                            //item.MlsNumber = item.objId;
                            if (item.Plans.Count > 0)
                            {
                                /*
                                item.NoOfBedRooms = item.Plans.Max(x => x.Bedrooms.Text);
                                item.NoOfBathRooms = item.Plans.Max(x => x.Baths);
                                item.NoOfHalfBathRooms = item.Plans.Max(x => x.HalfBaths);
                                item.Price = item.Plans.Max(x => x.BasePrice.Text);
                                item.LivingArea = Convert.ToDouble(item.Plans.Max(x => x.BaseSqft));
                                foreach (Plan item1 in item.Plans)
                                {
                                    item.PropertyType = item1.Type;
                                }
                                 */
                            }
                            else
                            {
                                item.NoOfBedRooms = 0;
                                item.NoOfBathRooms = 0;
                                item.NoOfHalfBathRooms = 0;
                                item.Price = 0;
                                item.LivingArea = 0;
                                //item.PropertyType = null;
                            }
                        }

                        lstPListing = (lstPurchase == null ? new List<PropertyListing>() : lstPurchase).Concat(lstRent == null ? new List<PropertyListing>() : lstRent).Concat(lstNew == null ? new List<PropertyListing>() : lstNew).Distinct().ToList();
                        //foreach (var item in lstPListing)
                        //{
                        //    //item.MlsNumber = item.objId;
                        //}
                        count = pcount + rcount + ncount;
                    }
                }
                else
                {
                    if (SubTab == "Featured")
                    {
                        lstPListing = _ElasticSearchIndicesPurchase.SearchFeaturedQuery(advSearch, startIndex, limit, out count);
                    }
                    else
                    {
                        lstPListing = _ElasticSearchIndicesPurchase.SearchQuery(advSearch, startIndex, limit, out count);
                    }
                }
            }
            else if (Command == "SearchPurchase")
            {
                if (SubTab == "Featured")
                {
                    lstPListing = _ElasticSearchIndicesPurchase.SearchFeaturedQuery(advSearch, startIndex, limit, out count);
                }
                else
                {
                    lstPListing = _ElasticSearchIndicesPurchase.SearchQuery(advSearch, startIndex, limit, out count);
                }
            }
            else if (Command == "SearchRent")
            {
                if (SubTab == "Featured")
                {
                    lstPListing = _ElasticSearchIndicesRental.SearchFeaturedQuery(advSearch, startIndex, limit, out count);
                }
                else
                {
                    lstPListing = _ElasticSearchIndicesRental.SearchQuery(advSearch, startIndex, limit, out count);
                }
            }
            else if (Command == "SearchHome")
            {
                if (SubTab == "Featured")
                {
                    lstPListing = _ElasticSearchIndicesNewHome.SearchFeaturedQuery(advSearch, startIndex, limit, out count);
                    //foreach (var item in lstPListing)
                    //{
                    //    //item.MlsNumber = item.objId;
                    //}
                }
                else
                {
                    lstPListing = _ElasticSearchIndicesNewHome.SearchQuery(advSearch, startIndex, limit, out count);
                    //foreach (var item in lstPListing)
                    //{
                    //    //item.MlsNumber = item.objId;
                    //}
                }


                foreach (var lst in lstPListing)
                {
                    var obj = lst as NewHome;
                    lst.CommunityName = new[] { obj.CommunityName };

                }
            }
            else if (Command == "SearchByAgent")
            {
                if (SubTab == "Featured")
                {
                    lstPListing = _ElasticSearchIndicesPurchase.SearchFeaturedQuery(advSearch, startIndex, limit, out count);
                }
                else
                {
                    lstPListing = _ElasticSearchIndicesPurchase.SearchQuery(advSearch, startIndex, limit, out count);
                }
            }
            else if (Command == "SearchOpenHouse")
            {
                if (SubTab == "Featured")
                {
                    //lstPListing = _ElasticSearchIndicesPurchase.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedCount, true);
                    lstPListing = _ElasticSearchIndicesPurchase.SearchFeaturedQuery(advSearch, startIndex, limit, out count, true);
                }
                else
                {
                    lstPListing = _ElasticSearchIndicesPurchase.SearchQuery(advSearch, startIndex, limit, out count, true);
                }
                //this is required for OpenHouse
                //lstPListing = _ElasticSearchIndicesPurchase.SearchQuery(advSearch, startIndex, limit, out count, true);
                //lstPropertyType = _realEstaeService.GetPurchasePropertyType().ToList();
                //if (lstPListing.Count > 0)
                //{
                //    lati = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[0]);
                //    longi = lstPListing.Average(x => x.GeoLocation == null ? 0 : x.GeoLocation.Coordinates[1]);
                //    nearByLcation = _realEstaeService.GetNearByPurchaseAreaDetails(longi, lati, 40233.6).OrderBy(m => m.AvgDistance).ToList();
                //}

                //isFeaturedPListing = _ElasticSearchIndicesPurchase.SearchFeaturedQuery(advSearch, startIndex, limit, out isFeaturedCount);
            }
            var usersAsIPagedList = new StaticPagedList<PropertyListing>(lstPListing, page, pageSize, count);
            List<Location> lstGeo = new List<Location>();
            foreach (var item in usersAsIPagedList)
            {

                lstGeo.Add(new Location
                {
                    Latitude = item.GeoLocation == null ? 32.714745 : (item.GeoLocation.Coordinates[1] == 0 ? 32.714745 : (item.GeoLocation.Coordinates[1] == item.GeoLocation.Coordinates[0] ? 32.714745 : item.GeoLocation.Coordinates[1])),//(item.GeoLocation == null ? 0 : item.GeoLocation.Coordinates[1]) == 0 ? -117.160469 : (item.GeoLocation == null ? 0 : item.GeoLocation.Coordinates[1]),
                    Longitude = item.GeoLocation == null ? -117.160469 : (item.GeoLocation.Coordinates[0] == 0 ? -117.160469 : (item.GeoLocation.Coordinates[0] == item.GeoLocation.Coordinates[1] ? -117.160469 : item.GeoLocation.Coordinates[0])),
                    FullStreet = item.FullStreetAddress,
                    MlsNumber = item.MlsNumber,
                    CommunityName = item.CommunityName.Count() == 0 ? null : item.CommunityName[0]
                });
            }
            ViewBag.SearchValue = searchTerm;
            ViewBag.Command = Command;
            ViewBag.SubCommand = SubCommand;
            ViewBag.SubTab = SubTab;
            ViewBag.Page = page;
            ViewBag.startIndex = startIndex;
            ViewBag.IsMls = IsMls;
            return Json(lstGeo, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SendMessage
        public JsonResult SendMessage(string Name, string Email, string Phone, string MlsNumber, string Message, string Subject, string BrokerMail, string Command, string SubCommand, string SelectedRow)
        {
            UserContactAndFindDetails userContactAndFindDetails = new UserContactAndFindDetails();
            userContactAndFindDetails.UserName = Name;
            userContactAndFindDetails.Email = Email;
            userContactAndFindDetails.PhoneNo = Phone;
            userContactAndFindDetails.MlsNumber = MlsNumber;

            _userContactDetails.InsertUpdateDetailsAgainstUser(userContactAndFindDetails, Command, SubCommand, SelectedRow);

#if DEBUG
            BrokerMail = "ankit.sarkar@indusnet.co.in,edwindantas@gmail.com,vikas@ntooitive.com";
#endif
            var isSuccess = _mailService.SendMail(new[] { BrokerMail }, Subject, Message,
                new[] { Email });
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public ActionResult AgentBio(string email)
        {
            var agent = NinjectConfig.Get<IAgent>();
            Repositories.Models.Admin.User.User u = agent.IsAgentFeatured(email);
            //ViewBag.AgentDesc = u.AgentDescription;
            return View(u);
        }
        public ActionResult NewHomeDetails(string mlsNumber, string communityName, string Command)
        {

            var planDetails = _newHome.PlanDetails(mlsNumber);
            planDetails.Plans = _newHome.ReleatedPlans(communityName).ToList();
            int i = 0;
            if (planDetails.Homes != null)
            {
                foreach (Home item in planDetails.Homes.Home)
                {
                    item.RowIndex = i;
                    i++;
                }
            }

            var community = _community.GetCommunitiesByName(communityName);
            if (String.IsNullOrEmpty(planDetails.Description))
            {
                planDetails.Description = community == null ? null : community.Description;
            }
            planDetails.communityImageUrl = community == null ? null : community.LogoImage;
            ViewBag.Command = Command;
            ViewBag.SubCommand = "New";
            if (planDetails.GeoPoint == null)
            {
                planDetails.Latitude = 32.714745;
                planDetails.Longitude = -117.160469;
            }
            else if (planDetails.GeoPoint.Coordinates == null)
            {
                planDetails.Latitude = 32.714745;
                planDetails.Longitude = -117.160469;
            }
            else if (planDetails.GeoPoint.Coordinates[0] == 0.0 || planDetails.GeoPoint.Coordinates[1] == 0.0)
            {
                planDetails.Latitude = 32.714745;
                planDetails.Longitude = -117.160469;
            }

            var agent = NinjectConfig.Get<IAgent>();
            Repositories.Models.Admin.User.User u = agent.IsAgentFeatured(planDetails.BuilderEmail);
            planDetails.BuilderLogo = u.LogoImage;
            planDetails.BuilderWebsite = u.WebsiteURL;
            //planDetails.WebsiteUrl = u.WebsiteURL;
            ViewBag.IsFeatured = u.IsFeatured;
            ViewBag.OfficePhone = u.OfficePhone;
            ViewBag.PrimaryContactPhone = u.PrimaryContactPhone;
            return View(planDetails);
        }
        public class Location
        {

            public string FullStreet { get; set; }
            public string StateAndZipCode { get; set; }
            public string MlsNumber { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string CommunityName { get; set; }
        }
    }
}