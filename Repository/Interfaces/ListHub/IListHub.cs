using System.Collections.Generic;
using MongoDB.Bson;
using Repositories.Interfaces.Base;
using Repositories.Models.Common;
using Repositories.Models.ListHub;
using Repositories.Models.ViewModel;
using Repositories.Models.Admin.ListHub;

namespace Repositories.Interfaces.ListHub
{
    public interface IListHub : IRepository<ListHubListing>
    {



        #region Purchase
        bool InsertBulkPurchaseCategoryRealestate(List<ListHubListing> entities);
        IEnumerable<AutoCompleteDetails> GetPurchaseAddressList(string searchKey);
        IEnumerable<AutoCompleteDetails> GetPurchaseMlsNumberList(string searchKey);
        PropertyDetails GetPurchaseRecordDetailsByMLSNumber(string mlsNumber);
        IEnumerable<PropertyListing> GetPurchaseRecordList(int skip, int limit, bool IsMls, AdvanceSearch advanceSearch);
        IEnumerable<PropertyListing> GetPurchaseIsFeaturedRecordList(int skip, int limit, bool IsMls, AdvanceSearch advanceSearch);
        int GetPurchaseIsFeaturedRecordCount(AdvanceSearch advanceSearch, bool IsMls);
        IEnumerable<PropertyTypeCheckBox> GetPurchasePropertyType();
        int GetPurchaseRecordCount(AdvanceSearch advanceSearch, bool IsMls);
        IEnumerable<NearbyArea> GetNearByPurchaseAreaDetails(double latitude, double longitude, double maxDistanceInMiles);
        ListHubListing GetPurchasePropertyDetailsByMLSNumber(string mlsNumber);
        IEnumerable<PropertyListing> GetPurchaseOpenHouseRecordList(int skip, int limit, bool IsMls, AdvanceSearch advanceSearch);
        int GetPurchaseOpenHouseRecordCount(AdvanceSearch advanceSearch, bool IsMls);
        IEnumerable<SubPropertyTypeCheckBox> GetPurchaseSubPropertyType();
        #endregion

        #region Rent
        bool InsertBulkRentCategoryRealestate(IEnumerable<ListHubListing> entities);
        IEnumerable<AutoCompleteDetails> GetRentAddressList(string searchKey);
        IEnumerable<AutoCompleteDetails> GetRentMlsNumberList(string searchKey);
        PropertyDetails GetRentRecordDetailsByMLSNumber(string mlsNumber);
        IEnumerable<PropertyListing> GetRentRecordList(int skip, int limit, bool IsMls, AdvanceSearch advanceSearch);
        IEnumerable<PropertyTypeCheckBox> GetRentPropertyType();
        int GetRentRecordCount(AdvanceSearch advanceSearch, bool IsMls);
        IEnumerable<NearbyArea> GetNearByRentAreaDetails(double latitude, double longitude, double maxDistanceInMiles);
        ListHubListing GetRentPropertyDetailsByMLSNumber(string mlsNumber);
        IEnumerable<PropertyListing> GetRentIsFeaturedRecordList(int skip, int limit, bool IsMls, AdvanceSearch advanceSearch);
        int GetRentIsFeaturedRecordCount(AdvanceSearch advanceSearch, bool IsMls);
        IEnumerable<PropertyListing> GetRentOpenHouseRecordList(int skip, int limit, bool IsMls, AdvanceSearch advanceSearch);
        int GetRentOpenHouseRecordCount(AdvanceSearch advanceSearch, bool IsMls);
        IEnumerable<SubPropertyTypeCheckBox> GetRentSubPropertyType();
        #endregion


        #region Admin Dashboard
        List<PropertyListing> GetPurchaseListing(string matchquery, string sortquery, int limit = 0, int skip = 0);
        long GetPurchaseRecordCount(string email);
        long GetPurchaseRecordCount(BsonDocument matchDoc);
        List<PropertyListing> GetRentRecordList(string matchquery, string sortquery, int limit = 0, int skip = 0);

        long GetRentRecordCount(BsonDocument matchDoc);
        long GetRentRecordCount(string email);

        bool UpdateListHub(string type, ListHubListing _ListHubListing);

        #endregion


        bool SetExtraProperty(string type, ManagePropertyViewModel propertyViewModel);
        ManagePropertyViewModel GetExtraProperty(string type, string mlsid);
        bool InsertRealestate(ListHubListing entities, string type);
        IEnumerable<PropertyType> GetPropertyType(string type);
        IEnumerable<PropertyType> GetPropertyTypeDesc(string type);
        IEnumerable<PropertySubType> GetSubPropertyType(string type);
        IEnumerable<PropertySubType> GetSubPropertyTypeDesc(string type);
        IEnumerable<string> GetListingStatus(string type);
        bool UpdateImageList(string type, ListHubListing propertyViewModel);
        string GetListedType(string type, string mlsid);
        bool DeleteProperty(string type, string mlsNumber, bool isDeleted);
        void ProcessFeed(List<ListHubListing> hubListings);
        void CreateIndex();

        bool InsertRealestate(List<ListHubListing> rentList, string p);

        bool UpdateForAgent(Repositories.Models.Admin.User.User user, string type);

        bool MakeBulkFeatured(string fieldbyUpdate, bool value, string uniqueid, string type);
        bool DeleteListHub(string mlsid, string type);

        List<PropertyDetails> GetClassified(string type, int count); //order by date

        PropertyDetails GetPropertyBasedOnCommunity(string type, string communityName);
        bool PullImage(string imageurl, string mlsNumber,string Type);
    }
}
