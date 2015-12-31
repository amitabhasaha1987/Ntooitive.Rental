using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Repositories.Interfaces;
using System.Web.Script.Serialization;
using Repositories.Models;
using System.Web.UI;

namespace UserInterface.Controllers
{
    public class HomeController : Controller
    {
        readonly IRealEstate _realEstaeService;


        public HomeController(IRealEstate realEstaeService)
        {
            _realEstaeService = realEstaeService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAutoCompleteData(string term)
        {
            var lst = _realEstaeService.GetPurchaseAddressList(term).Select(m => m.CityWithStateAndPostCode.ToList()).FirstOrDefault();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PropertiesListing(string searchTerm)
        {
            var lstRealEstate = _realEstaeService.GetPurchaseRecordList(0, 50, searchTerm,true);
            //GetGeoLocation(searchTerm);
            List<PropertyListing> lstPListing = lstRealEstate.ToList();
            List<string> lstGeo = new List<string>();
            string str = null;
            foreach (PropertyListing item in lstPListing)
            {
                str = str == null ? "<h4>" + item.PropertyType + "</h4>," + item.GeoLocation.Coordinates[0].ToString() + "," + item.GeoLocation.Coordinates[1].ToString() : str + "," + "<h4>" + item.PropertyType + "</h4>," + item.GeoLocation.Coordinates[1].ToString() + "," + item.GeoLocation.Coordinates[0].ToString();
                //lstGeo.Add("<h4>" + item.PropertyType + "</h4>," + item.GeoLocation.Coordinates[1].ToString() + "," + item.GeoLocation.Coordinates[0].ToString());
            }
            //var serializer = new JavaScriptSerializer();
            //var json = serializer.Serialize(lstGeo).Replace('"', ' ');
            //var finalJson = json.Replace("\\u0027", "'");
            ViewBag.JsonValue = str;// "[ [\'Abc\',-77.31534,39.11055] , [\'Abc\',-77.30345,39.10853] ]";
            //ScriptManager.RegisterArrayDeclaration(Page, "markers", finalJson);
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "GoogleMap();", true);
            return View(lstRealEstate);
        }

        //[HttpPost]
        public ActionResult PropertieDetails(string mlsNumber)
        {
            var lstRealEstate = _realEstaeService.GetPurchaseRecordDetailsByMLSNumber(mlsNumber);
            return View(lstRealEstate);
        }

        [HttpGet]
        public JsonResult GetGeoLocation(string name)
        {
            var lstRealEstate = _realEstaeService.GetPurchaseRecordList(0, 50, name,true);
            List<PropertyListing> lstPListing = lstRealEstate.ToList();
            List<RootObject> lstGeo = new List<RootObject>();
            foreach (var item in lstPListing)
            {
                lstGeo.Add(new RootObject
                {
                    Latitude = item.GeoLocation.Coordinates[1],
                    Longitude = item.GeoLocation.Coordinates[0],
                    LocationName = item.PropertyType
                });
            }
            return Json(lstGeo, JsonRequestBehavior.AllowGet);
        }


        public class RootObject
        {
           
            public string LocationName { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
}