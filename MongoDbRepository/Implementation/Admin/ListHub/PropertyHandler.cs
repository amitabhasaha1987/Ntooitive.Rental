using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Repositories.Interfaces.Admin.ListHub;
using Repositories.Interfaces.ListHub;
using Repositories.Models.Admin.ListHub;
using Repositories.Models.Common;
using Repositories.Models.DataTable;

namespace Core.Implementation.Admin.ListHub
{
    public class PropertyHandler : IProperties
    {
        private readonly IListHub _listHub;
        public PropertyHandler(IListHub listHub)
        {
            this._listHub = listHub;
        }

        public List<PropertyListing> GetDataSet(string userEmail, JQueryDataTableParamModel dataTableParamModel, ListHubPropertyDataTable serachCriteria, out long filteredCount,string type = "")
        {
            var sortQuery = "";
            var matchQuery = "";
            if (!string.IsNullOrEmpty(userEmail))
            {
                matchQuery = "{'ListingParticipants.Participant.Email' : '" + userEmail + "'}";

            }
            else
            {
                matchQuery = "{}";
            }
      
              var propertyListings = new List<PropertyListing>();
            if (serachCriteria.sortColumnIndex == 2 && serachCriteria.isPriceSortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{Price : 1}" : "{Price : -1}";
            }
            if (serachCriteria.sortColumnIndex == 3 && serachCriteria.isPropertySortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{PropertyType : 1}" : "{PropertyType : -1}";
            }
            if (serachCriteria.sortColumnIndex == 4 && serachCriteria.isLivingArearSortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{LivingArea : 1}" : "{LivingArea : -1}";
            }
              
            if (!string.IsNullOrEmpty(dataTableParamModel.sSearch))
            {
                if (serachCriteria.isMlsSearchable)
                {
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        matchQuery = "{'ListingParticipants.Participant.Email' : '" + userEmail + "','MlsNumber': {'$regex': '" +
                                                         dataTableParamModel.sSearch + "', '$options': 'i' }}";
                    }
                    else
                    {
                        matchQuery = "{'MlsNumber': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}";
                    }
                    
                }
               
            }
            matchQuery = matchQuery.Replace(@"\", "");

            var startstr = "{$or: [";
            var endstr = "]}";
            matchQuery = startstr + matchQuery + endstr;
            matchQuery = "{$and: [{$or: [{IsDeletedByPortal: {$exists: false}}, {IsDeletedByPortal: false}]}," + matchQuery + endstr;

            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
            if (type == "purchase")
            {
                propertyListings = _listHub.GetPurchaseListing(matchQuery, sortQuery, dataTableParamModel.iDisplayLength,
                    dataTableParamModel.iDisplayStart);

                filteredCount = _listHub.GetPurchaseRecordCount(matchDoc);
            }
            else
            {
                propertyListings = _listHub.GetRentRecordList(matchQuery, sortQuery, dataTableParamModel.iDisplayLength,
                   dataTableParamModel.iDisplayStart);

                filteredCount = _listHub.GetRentRecordCount(matchDoc);
            }
          

            return propertyListings;
        }

        public long GetTotalCount(string userEmail, string type = "")
        {
            if (type == "rent")
            {
                return _listHub.GetRentRecordCount(userEmail);
            }
            else
            {
                  return _listHub.GetPurchaseRecordCount(userEmail);
            }
          
        }
    }
}
