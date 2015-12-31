using Repositories.Interfaces.Admin.Office;
using Repositories.Models.Admin.Office;
using Repositories.Models.DataTable;
using Repositories.Models.ListHub;
using Security.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repositories.Interfaces.Admin.Users;

namespace AdminInterface.Controllers
{
    [RoutePrefix("office")]
    public class OfficeController : Controller
    {
        private readonly IOffice _office;
        private readonly IAgent _agent;

        public OfficeController(IOffice office, IAgent agent)
        {
            _office = office;
            _agent = agent;
        }
        //
        // GET: /Office/
        #region Listing
        [CustomAuthorize(Roles = "Admin")]
        [Route("office-listing")]
        public ActionResult OfficeListing()
        {
            return View();
        }

        [CustomAuthorize(Roles = "Admin")]
        [Route("office-listing-ajax-handler")]
        public ActionResult UserAjaxHandler(JQueryDataTableParamModel param)
        {
            long filterCount = 0;
            var listHubPropertysearchCriteria = new OfficeDataTable()
            {
                isOfficeIdSearchable = Convert.ToBoolean(Request["bSearchable_0"]),
                isNameSearchable = Convert.ToBoolean(Request["bSearchable_1"]),
                isCorporateNameSearchable = Convert.ToBoolean(Request["bSearchable_2"]),
                isBrokerIdSearchable = Convert.ToBoolean(Request["bSearchable_3"]),
                isPhoneNumberSearchable = Convert.ToBoolean(Request["bSearchable_4"]),
                isOfficeEmailSearchable = Convert.ToBoolean(Request["bSearchable_5"]),
                isWebsiteSearchable = Convert.ToBoolean(Request["bSearchable_6"]),

                isOfficeIdSortable = Convert.ToBoolean(Request["bSortable_0"]),
                isNameSortable = Convert.ToBoolean(Request["bSortable_1"]),
                isCorporateNameSortable = Convert.ToBoolean(Request["bSortable_2"]),
                isBrokerIdSortable = Convert.ToBoolean(Request["bSortable_3"]),
                isPhoneNumberSortable = Convert.ToBoolean(Request["bSortable_4"]),
                isOfficeEmailSortable = Convert.ToBoolean(Request["bSortable_5"]),
                isWebsiteSortable = Convert.ToBoolean(Request["bSortable_6"]),


                sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]),
                sortDirection = Request["sSortDir_0"]
            };

            var filteredData = _office.GetDataSet("", param, listHubPropertysearchCriteria, out filterCount, "purchase");

            var totalCount = _office.GetTotalCount("");

            var result = from c in filteredData
                         select
                             new[]
                             {
                                  c.OfficeId,
                         c.OfficeId,
                         c.Name,
                         c.CorporateName,
                         c.BrokerId,
                         c.PhoneNumber,
                         c.OfficeEmail,
                         c.Website,
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


        [CustomAuthorize(Roles = "Admin")]
        [Route("agent-listing-ajax-handler")]
        public ActionResult HomeAjaxHandler(JQueryDataTableParamModel param, string officeId)
        {
            var agentlist = _agent.GetAgents(officeId);
            return View("AgentDetails", agentlist);

        }

        #endregion

        #region Manage
        [CustomAuthorize(Roles = "Admin")]
        [Route("manage/{uniquid}")]
        public ActionResult ManageOffice(string uniquid)
        {
            var manageUser = _office.GetOfficeDetails(uniquid);
            return View(manageUser);
        }
        #endregion

        #region "Delete Office"

        [CustomAuthorize(Roles = "Admin")]
        [Route("delete-office")]
        public JsonResult DeleteOffice(string officeId)
        {
            bool IsDeleted = _office.DeleteOffice(officeId, true);

            return Json(true);
        }

        #endregion

        #region Add
        [CustomAuthorize(Roles = "Admin")]
        [Route("new-office")]
        public ActionResult Add(string officeId)
        {

            NewOffice newOffc = new NewOffice();
            newOffc.StateList = _office.GetStateList().ToList().Where(m => !string.IsNullOrEmpty(m.StateOrProvince)).ToList();
            State st = new State();
            st.StateOrProvince = "Other";
            newOffc.StateList.Add(st);

            newOffc.CityList = _office.GetCityList().ToList();
            Cities ct = new Cities();
            ct.City = "Other";
            newOffc.CityList.Add(ct);

            newOffc.StreetList = _office.GetStreetAddressList().ToList();
            StreetAddress sa = new StreetAddress();
            sa.FullStreetAddress = "Other";
            newOffc.StreetList.Add(sa);

            newOffc.ZipCodeList = _office.GetZipCodeList().ToList();
            ZipCode zc = new ZipCode();
            zc.PostalCode = "Other";
            newOffc.ZipCodeList.Add(zc);
            //In case of update
            if (!String.IsNullOrEmpty(officeId))
            {

                var manageOffice = _office.GetOfficeDetails(officeId);
                //NewOffice objNewOffice = new NewOffice();
                newOffc.Id = manageOffice.Id;
                newOffc.ListedBy = manageOffice.ListedBy;
                newOffc.MainOfficeId = manageOffice.MainOfficeId;
                newOffc.Name = manageOffice.Name;
                newOffc.OfficeCode = manageOffice.OfficeCode;
                newOffc.OfficeEmail = manageOffice.OfficeEmail;
                newOffc.OfficeId = manageOffice.OfficeId;
                newOffc.OfficeImageUrl = manageOffice.OfficeImageUrl;
                newOffc.OfficeKey = manageOffice.OfficeKey;
                newOffc.PhoneNumber = manageOffice.PhoneNumber;
                newOffc.CorporateName = manageOffice.CorporateName;
                newOffc.BrokerId = manageOffice.BrokerId;
                newOffc.MainOfficeId = manageOffice.MainOfficeId;
                newOffc.Website = manageOffice.Website;
                newOffc.Address = new Address()
                {
                    Country = manageOffice.Address.Country,
                    StateOrProvince = manageOffice.Address.StateOrProvince,
                    PostalCode = manageOffice.Address.PostalCode,
                    City = manageOffice.Address.City,
                    FullStreetAddress = manageOffice.Address.FullStreetAddress
                };


                //objNewOffice.StateList = manageOffice.
            }


            return View(newOffc);
        }

        [HttpGet]
        [Route("citylist")]
        public JsonResult CityList(string StateName)
        {
            try
            {
                NewOffice newOffc = new NewOffice();
                newOffc.CityList = _office.GetCityList(StateName).ToList();
                Cities ct = new Cities();
                ct.City = "Other";
                newOffc.CityList.Add(ct);
                return Json(newOffc.CityList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.message = ex.Message;
                throw ex;
            }
        }

        [HttpGet]
        [Route("zipcodelist")]
        public JsonResult ZipCodeList(string CityName)
        {
            try
            {
                NewOffice newOffc = new NewOffice();
                newOffc.ZipCodeList = _office.GetZipCodeList(CityName).ToList();
                ZipCode zc = new ZipCode();
                zc.PostalCode = "Other";
                newOffc.ZipCodeList.Add(zc);
                return Json(newOffc.ZipCodeList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.message = ex.Message;
                throw ex;
            }
        }

        [HttpGet]
        [Route("fullstreetaddresslist")]
        public JsonResult FullStreetAddressList(string CityName)
        {
            try
            {
                NewOffice newOffc = new NewOffice();
                newOffc.StreetList = _office.GetStreetAddressList(CityName).ToList();
                StreetAddress sa = new StreetAddress();
                sa.FullStreetAddress = "Other";
                newOffc.StreetList.Add(sa);
                return Json(newOffc.StreetList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.message = ex.Message;
                throw ex;
            }
        }

        [CustomAuthorize(Roles = "Admin")]
        [Route("add-office")]
        public JsonResult AddOffice(NewOffice newOffc)
        {

            Office offc = new Office();

            offc.Name = newOffc.Name;
            offc.CorporateName = newOffc.CorporateName;
            offc.BrokerId = newOffc.BrokerId;
            offc.PhoneNumber = newOffc.PhoneNumber;
            Address addr = new Address();
            addr.Country = "US";
            addr.City = newOffc.Address.City;
            addr.FullStreetAddress = newOffc.Address.FullStreetAddress;
            addr.StateOrProvince = newOffc.Address.StateOrProvince;
            addr.PostalCode = newOffc.Address.PostalCode;
            offc.Address = addr;
            offc.OfficeEmail = newOffc.OfficeEmail;
            offc.Website = newOffc.Website;
            offc.OfficeDescription = newOffc.OfficeDescription;
            offc.OfficeLogo = newOffc.OfficeLogo;
            offc.IsUpdateByPortal = true;
            offc.OfficeId = newOffc.OfficeId;
            offc.OfficeCode = new OfficeCode { OfficeCodeId = offc.OfficeId };
            offc.OfficeKey = newOffc.OfficeKey;
            offc.MainOfficeId = offc.OfficeId;
            //In case of update checking whether data is present or not in DB so if present then update

            if (_office.Validation(offc))
            {
                var manageOffice = _office.GetOfficeDetails(newOffc.OfficeId);

                if (String.IsNullOrEmpty(newOffc.OfficeId))
                {
                    offc.OfficeId = Utility.UtilityClass.GetUniqueKey();
                    offc.OfficeCode = new OfficeCode { OfficeCodeId = offc.OfficeId };
                    offc.OfficeKey = "3yd-SDRE-" + offc.OfficeId;
                    offc.MainOfficeId = offc.OfficeId;
                    List<Office> offcList = new List<Office>();
                    offcList.Add(offc);
                    bool IsInserted = _office.InsertBulkOffices(offcList);
                    return Json(new { success = IsInserted, ParticipantId = offc.OfficeId });
                }
                else
                {
                    bool IsInserted = _office.UpdateOffice(offc);
                    return Json(new { success = IsInserted, ParticipantId = offc.OfficeId });
                }
            }
            else
            {
                return Json(new { success = false, ParticipantId = offc.OfficeId });
            }
        }
        #endregion
    }
}