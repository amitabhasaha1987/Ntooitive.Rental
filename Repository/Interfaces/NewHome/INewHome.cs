using System.Collections.Generic;
using MongoDB.Bson;
using Repositories.Models.Common;
using Repositories.Models.NewHome;
using Repositories.Models.ViewModel;

namespace Repositories.Interfaces.NewHome
{
    public interface INewHome
    {

        #region Added New Section
        IEnumerable<Plan> ReleatedPlans(string communityName);
        Plan PlanDetails(string plainId);
        // Plan GetPlan(string PlanId);
        bool UpdateNewHomePlan(Home home, string planid);

        bool UpdateHome(Home home, string planid);
        #endregion
        #region New Home
        bool InsertBulkNewHomes(List<Plan> homeListings);
        IEnumerable<AutoCompleteDetails> GetNewHomeAddressList(string searchKey);
        PropertyDetails GetNewHomeRecordDetailsByMLSNumber(string mlsNumber);
        IEnumerable<PropertyListing> GetNewHomeRecordList(int skip, int limit, bool IsMls, AdvanceSearch advanceSearch);
        IEnumerable<PropertyTypeCheckBox> GetNewHomePropertyType();
        IEnumerable<SubPropertyTypeCheckBox> GetNewHomeSubPropertyType();
        int GetNewHomeRecordCount(AdvanceSearch advanceSearch, bool IsMls);
        IEnumerable<NearbyArea> GetNearByNewHomeAreaDetails(double latitude, double longitude, double maxDistanceInMiles);
        Plan GetNewHomePropertyDetailsByMLSNumber(string mlsNumber);

        List<Plan> GetNewHomeRecordList(string matchquery, string sortquery, int limit = 0, int skip = 0);
        long GetNewHomeRecordCount(BsonDocument matchDoc);


        List<Plan> GetPlans(string builderId);
        IEnumerable<PropertyListing> GetNewHomeIsFeaturedRecordList(int skip, int limit, bool IsMls, AdvanceSearch advanceSearch);
        int GetNewHomeIsFeaturedRecordCount(AdvanceSearch advanceSearch, bool IsMls);

        IEnumerable<PropertyListing> GetNewHomeOpenHouseRecordList(int skip, int limit, bool IsMls, AdvanceSearch advanceSearch);
        int GetNewHomeOpenHouseRecordCount(AdvanceSearch advanceSearch, bool IsMls);
        #endregion

        ManagePropertyViewModel GetExtraProperty(string mlsid);
        bool SetExtraProperty(ManagePropertyViewModel viewModel);
        bool UpdatePlanImageList(string[] imageList, string planid);
        bool UpdateHomeImageList(string[] imageList, string planid, string homeid);
        int ImageListCount(string BuilderNumber);
        bool InsertNewHome(Plan entities);
        bool UpdateExtraProperty(ManageNewHomePropertyViewModel viewModel);
        bool UpdateNewHomePlanList(Plan propertyViewModel);
        bool UpdateNewHomePlan(Plan plan);
        string GetListedType(string mlsid);
        bool DeleteProperty(string mlsNumber, bool IsDeleted);
        string GetBuilderNo(string planNo);
        void CreateIndex();
        IEnumerable<string> GetPropertyType();
        bool InsertFromFeed(List<Plan> plans);
        bool DeleteNewHome(string planId);
        bool PullImage(string imageurl, string mlsNumber);
        bool PullImageFromHome(string imageurl, string homeid, string planid);
        bool PullHomeFromPlan(string homeid, string planid);
    }
}
