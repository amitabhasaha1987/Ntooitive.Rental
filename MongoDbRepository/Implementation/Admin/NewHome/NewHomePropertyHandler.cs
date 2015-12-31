using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Repositories.Interfaces.Admin.NewHome;
using Repositories.Interfaces.NewHome;
using Repositories.Models.Admin.NewHome;
using Repositories.Models.DataTable;
using Repositories.Models.NewHome;

namespace Core.Implementation.Admin.NewHome
{
  public  class NewHomePropertyHandler : INewHomes
    {
        private readonly INewHome _newHomes;

        public NewHomePropertyHandler(INewHome newHomes)
        {
            _newHomes = newHomes;
        }


        public List<Plan> GetDataSet(string userEmail, JQueryDataTableParamModel dataTableParamModel,
            NewHomesPropertyDataTable serachCriteria, out long filteredCount, string type = "")
        {
            var sortQuery = "";
            var matchQuery = !string.IsNullOrEmpty(userEmail) ? "{$and: [{$or: [{IsDeletedByPortal: {$exists: false}}, {IsDeletedByPortal: false}]},{'BuilderEmail' : '" + userEmail + "'}]}" : "{$or : [{IsDeletedByPortal : {$exists : false} },{IsDeletedByPortal : false}]}";
            //matchQuery = "{$or : [{IsDeletedByPortal : {$exists : false} },{IsDeletedByPortal : false}]}";
            if (serachCriteria.sortColumnIndex == 1 && serachCriteria.isBuilderNoSortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{BuilderNumber : 1}" : "{BuilderNumber : -1}";
            }
            if (serachCriteria.sortColumnIndex == 2 && serachCriteria.isBuilderNameSortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{BuilderName : 1}" : "{BuilderName : -1}";
            }
            if (serachCriteria.sortColumnIndex == 3 && serachCriteria.isPriceHighSortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{'Base_price' : 1}" : "{'Base_price' : -1}";
            }
            if (serachCriteria.sortColumnIndex == 4 && serachCriteria.isPriceLowSortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{'Sqft_low' : 1}" : "{'Sqft_low' : -1}";
            }
            if (serachCriteria.sortColumnIndex == 5 && serachCriteria.isSqFtHighSortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{'Is_active' : 1}" : "{'Is_active' : -1}";
            }
            if (serachCriteria.sortColumnIndex == 6 && serachCriteria.isSqFtLowSortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{'Communityaddress' : 1}" : "{'Communityaddress' : -1}";
            }
            
            if (!string.IsNullOrEmpty(dataTableParamModel.sSearch))
            {

                var startstr = "{$or: [";
                var endstr = "]}";
                var listOfmatchQuery = new List<string>();

                if (serachCriteria.isBuilderNoSearchable)
                {
                    listOfmatchQuery.Add("{'BuilderNumber': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (serachCriteria.isBuilderNameSearchable)
                {
                    listOfmatchQuery.Add("{'BuilderName': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (serachCriteria.isPriceHighSearchable)
                {
                    listOfmatchQuery.Add("{'Base_price': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (serachCriteria.isPriceLowSearchable)
                {
                    listOfmatchQuery.Add("{'Sqft_low': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (serachCriteria.isSqFtHighSearchable)
                {
                    listOfmatchQuery.Add("{'Is_active': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (serachCriteria.isSqFtLowSearchable)
                {
                    listOfmatchQuery.Add("{'Communityaddress': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                
                matchQuery = startstr + string.Join(",", listOfmatchQuery) + endstr;
                matchQuery = "{$and: [{$or: [{IsDeletedByPortal: {$exists: false}}, {IsDeletedByPortal: false}]}," + matchQuery + endstr;
            }
            matchQuery = matchQuery.Replace(@"\", "");

            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);

            var newHomeListings = _newHomes.GetNewHomeRecordList(matchQuery, sortQuery, dataTableParamModel.iDisplayLength,
                dataTableParamModel.iDisplayStart);

            filteredCount = _newHomes.GetNewHomeRecordCount(matchDoc);

            return newHomeListings;
        }

        public long GetTotalCount(string userEmail, string type = "")
        {
            var matchQuery = !string.IsNullOrEmpty(userEmail) ? "{'BuilderEmail' : '" + userEmail + "'}" : "{}";
            matchQuery = matchQuery.Replace(@"\", "");

            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
            return _newHomes.GetNewHomeRecordCount(matchDoc);
        }

      public List<Plan> GetPlans(string builderId)
      {
            return _newHomes.GetPlans(builderId);
      }

    }
}
