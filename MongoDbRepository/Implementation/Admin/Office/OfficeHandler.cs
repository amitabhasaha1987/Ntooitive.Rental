using Core.Repository;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Repositories.Interfaces.Admin.Office;
using Repositories.Models.Admin.Office;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Core.Implementation.Admin.Office
{
    public class OfficeHandler : Repository<Repositories.Models.ListHub.Office>, IOffice
    {
        private new const string CollectionName = "";
        public OfficeHandler()
            : base(CollectionName)
        {
        }
        public OfficeHandler(IMongoDatabase database)
            : base(database, CollectionName)
        {

        }
        public OfficeHandler(string connectionString, string databaseName)
            : base(connectionString, databaseName, CollectionName)
        {
        }
        public bool InsertBulkOffices(List<Repositories.Models.ListHub.Office> users)
        {
            base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);
            return base.InsertBulk(users);
        }

        public bool UpdateOffice(Repositories.Models.ListHub.Office office)
        {
            base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);

            var updateResult = GetCollection().UpdateOneAsync(
                m => m.OfficeId == office.OfficeId,
                Builders<Repositories.Models.ListHub.Office>.Update
                    .Set(m => m.Name, office.Name)
                    .Set(m => m.CorporateName, office.CorporateName)
                    .Set(m => m.BrokerId, office.BrokerId)
                    .Set(m => m.MainOfficeId, office.MainOfficeId)
                    .Set(m => m.PhoneNumber, office.PhoneNumber)
                    .Set(m => m.Address.StateOrProvince, office.Address.StateOrProvince)
                    .Set(m => m.Address.City, office.Address.City)
                    .Set(m => m.Address.PostalCode, office.Address.PostalCode)
                    .Set(m => m.Address.FullStreetAddress, office.Address.FullStreetAddress)
                    .Set(m => m.Website, office.Website)
                    .Set(m => m.OfficeEmail, office.OfficeEmail)
                    .Set(m => m.OfficeDescription, office.OfficeDescription)
                    .Set(m => m.OfficeLogo, office.OfficeLogo)
                    .Set(m => m.IsUpdateByPortal, true)
                    ).Result;

            return updateResult.ModifiedCount > 0;
        }

        public bool DeleteOffice(string uniquid, bool isDeleted)
        {
            base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);

            var updateOffice = GetCollection().UpdateOneAsync(
                m => m.OfficeId == uniquid,
                Builders<Repositories.Models.ListHub.Office>.Update
                    .Set(m => m.IsDeletedByPortal, isDeleted)
                    ).Result;

            return updateOffice.ModifiedCount > 0;
        }
        public Repositories.Models.ListHub.Office GetOfficeDetails(string officeId)
        {
            base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);
            var tmpList = new Repositories.Models.ListHub.Office();
            using (var cursor = GetCollection()
                .FindAsync(m => m.OfficeId == officeId)
                .Result)
            {
                while (cursor.MoveNextAsync().Result)
                {
                    tmpList = cursor.Current.FirstOrDefault();
                }
                return tmpList;
            }
        }

        public List<Repositories.Models.ListHub.Office> GetDataSet(string userEmail, Repositories.Models.DataTable.JQueryDataTableParamModel dataTableParamModel,
            Repositories.Models.Admin.Office.OfficeDataTable propertyDataTableSerachCriteria, out long filteredCount, string type = "")
        {
            base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);
            var sortQuery = "";
            //var matchQuery = "{}";
            var matchQuery = "{$or : [{IsDeletedByPortal : {$exists : false} },{IsDeletedByPortal : false}]}";
            if (propertyDataTableSerachCriteria.sortColumnIndex == 0 && propertyDataTableSerachCriteria.isOfficeIdSortable)
            {
                sortQuery = propertyDataTableSerachCriteria.sortDirection == "asc" ? "{OfficeId : 1}" : "{OfficeId : -1}";
            }
            if (propertyDataTableSerachCriteria.sortColumnIndex == 1 && propertyDataTableSerachCriteria.isNameSearchable)
            {
                sortQuery = propertyDataTableSerachCriteria.sortDirection == "asc" ? "{Name : 1}" : "{Name : -1}";
            }
            if (propertyDataTableSerachCriteria.sortColumnIndex == 2 && propertyDataTableSerachCriteria.isCorporateNameSortable)
            {
                sortQuery = propertyDataTableSerachCriteria.sortDirection == "asc" ? "{CorporateName : 1}" : "{CorporateName : -1}";
            }
            if (propertyDataTableSerachCriteria.sortColumnIndex == 3 && propertyDataTableSerachCriteria.isBrokerIdSortable)
            {
                sortQuery = propertyDataTableSerachCriteria.sortDirection == "asc" ? "{BrokerId : 1}" : "{BrokerId : -1}";
            }
            if (propertyDataTableSerachCriteria.sortColumnIndex == 4 && propertyDataTableSerachCriteria.isOfficeEmailSearchable)
            {
                sortQuery = propertyDataTableSerachCriteria.sortDirection == "asc" ? "{OfficeEmail : 1}" : "{OfficeEmail : -1}";
            }
            if (propertyDataTableSerachCriteria.sortColumnIndex == 5 && propertyDataTableSerachCriteria.isWebsiteSortable)
            {
                sortQuery = propertyDataTableSerachCriteria.sortDirection == "asc" ? "{Website : 1}" : "{Website : -1}";
            }

            if (!string.IsNullOrEmpty(dataTableParamModel.sSearch))
            {
                var startstr = "{$or: [";
                var endstr = "]}";
                var listOfmatchQuery = new List<string>();

                if (propertyDataTableSerachCriteria.isOfficeIdSearchable)
                {
                    listOfmatchQuery.Add("{'OfficeId': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (propertyDataTableSerachCriteria.isNameSearchable)
                {
                    listOfmatchQuery.Add("{'Name': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (propertyDataTableSerachCriteria.isCorporateNameSearchable)
                {
                    listOfmatchQuery.Add("{'CorporateName': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (propertyDataTableSerachCriteria.isBrokerIdSearchable)
                {
                    listOfmatchQuery.Add("{'BrokerId': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (propertyDataTableSerachCriteria.isPhoneNumberSearchable)
                {
                    listOfmatchQuery.Add("{'PhoneNumber': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (propertyDataTableSerachCriteria.isOfficeEmailSearchable)
                {
                    listOfmatchQuery.Add("{'OfficeEmail': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (propertyDataTableSerachCriteria.isWebsiteSearchable)
                {
                    listOfmatchQuery.Add("{'Website': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                matchQuery = startstr + string.Join(",", listOfmatchQuery) + endstr;
                matchQuery = "{$and: [{$or: [{IsDeletedByPortal: {$exists: false}}, {IsDeletedByPortal: false}]}," + matchQuery + endstr;
            }


            matchQuery = matchQuery.Replace(@"\", "");
            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
            var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sortQuery);

            var userListings = GetCollection().Aggregate(new AggregateOptions()
            {
                AllowDiskUse = true
            })
                .Match(matchDoc)
                .Sort(sortDoc)
                .Skip(dataTableParamModel.iDisplayStart)
                .Limit(dataTableParamModel.iDisplayLength)
                .ToListAsync()
                .Result.ToList();

            filteredCount = GetCollection().CountAsync(matchDoc).Result;
            return userListings;
        }

        public long GetTotalCount(string userEmail, string type = "")
        {
            base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);

            return GetCollection().CountAsync(new BsonDocument()).Result;
        }


        public IEnumerable<Repositories.Models.Admin.Office.State> GetStateList()
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);

                var group = "{_id : '$Address.StateOrProvince',Count : {$sum :1}}";
                var projectQuery = "{_id : 0,StateOrProvince : '$_id',StateCount : '$Count'}";
                var sort = "{SateOrProvince : 1}";

                var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sort);
                var groupDoc = BsonSerializer.Deserialize<BsonDocument>(group);
                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

                var aggregate = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
                .Group(groupDoc)
                .Project(projectDoc)
                .Sort(sortDoc)
                .ToListAsync().Result.Select(m => BsonSerializer.Deserialize<State>(m));

                return aggregate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<Repositories.Models.Admin.Office.Cities> GetCityList()
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);

                var group = "{_id : '$Address.City',Count : {$sum :1}}";
                var projectQuery = "{_id : 0,City : '$_id',CityCount : '$Count'}";
                var sort = "{City : 1}";

                var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sort);
                var groupDoc = BsonSerializer.Deserialize<BsonDocument>(group);
                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

                var aggregate = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
                .Group(groupDoc)
                .Project(projectDoc)
                .Sort(sortDoc)
                .ToListAsync().Result.Select(m => BsonSerializer.Deserialize<Cities>(m));

                return aggregate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<Repositories.Models.Admin.Office.Cities> GetCityList(string StateName)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);

                var matchQuery = "{'Address.StateOrProvince': '" + StateName + "'}";
                var group = "{_id : '$Address.City',Count : {$sum :1}}";
                var projectQuery = "{_id : 0,City : '$_id',CityCount : '$Count'}";
                var sort = "{City : 1}";

                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
                var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sort);
                var groupDoc = BsonSerializer.Deserialize<BsonDocument>(group);
                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

                var aggregate = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
                .Match(matchDoc)
                .Group(groupDoc)
                .Project(projectDoc)
                .Sort(sortDoc)
                .ToListAsync().Result.Select(m => BsonSerializer.Deserialize<Cities>(m));

                return aggregate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<Repositories.Models.Admin.Office.ZipCode> GetZipCodeList()
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);

                var group = "{_id : '$Address.PostalCode',Count : {$sum :1}}";
                var projectQuery = "{_id : 0,PostalCode : '$_id',PostalCodeCount : '$Count'}";
                var sort = "{City : 1}";

                var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sort);
                var groupDoc = BsonSerializer.Deserialize<BsonDocument>(group);
                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

                var aggregate = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
                .Group(groupDoc)
                .Project(projectDoc)
                .Sort(sortDoc)
                .ToListAsync().Result.Select(m => BsonSerializer.Deserialize<ZipCode>(m));

                return aggregate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<Repositories.Models.Admin.Office.ZipCode> GetZipCodeList(string CityName)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);

                var matchQuery = "{'Address.City': '" + CityName + "'}";
                var group = "{_id : '$Address.PostalCode',Count : {$sum :1}}";
                var projectQuery = "{_id : 0,PostalCode : '$_id',PostalCodeCount : '$Count'}";
                var sort = "{City : 1}";

                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
                var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sort);
                var groupDoc = BsonSerializer.Deserialize<BsonDocument>(group);
                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

                var aggregate = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
                .Match(matchDoc)
                .Group(groupDoc)
                .Project(projectDoc)
                .Sort(sortDoc)
                .ToListAsync().Result.Select(m => BsonSerializer.Deserialize<ZipCode>(m));

                return aggregate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<Repositories.Models.Admin.Office.StreetAddress> GetStreetAddressList()
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);

                var group = "{_id : '$Address.FullStreetAddress',Count : {$sum :1}}";
                var projectQuery = "{_id : 0,FullStreetAddress : '$_id',FullStreetAddressCount : '$Count'}";
                var sort = "{City : 1}";

                var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sort);
                var groupDoc = BsonSerializer.Deserialize<BsonDocument>(group);
                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

                var aggregate = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
                .Group(groupDoc)
                .Project(projectDoc)
                .Sort(sortDoc)
                .ToListAsync().Result.Select(m => BsonSerializer.Deserialize<StreetAddress>(m));

                return aggregate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<Repositories.Models.Admin.Office.StreetAddress> GetStreetAddressList(string CityName)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);

                var matchQuery = "{'Address.City': '" + CityName + "'}";
                var group = "{_id : '$Address.FullStreetAddress',Count : {$sum :1}}";
                var projectQuery = "{_id : 0,FullStreetAddress : '$_id',FullStreetAddressCount : '$Count'}";
                var sort = "{City : 1}";

                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
                var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sort);
                var groupDoc = BsonSerializer.Deserialize<BsonDocument>(group);
                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

                var aggregate = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
                .Match(matchDoc)
                .Group(groupDoc)
                .Project(projectDoc)
                .Sort(sortDoc)
                .ToListAsync().Result.Select(m => BsonSerializer.Deserialize<StreetAddress>(m));

                return aggregate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool UploadProfileImage(string uniquid, string url)
        {
            base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);

            var updateResult = GetCollection().UpdateOneAsync(
                m => m.OfficeId == uniquid,
                Builders<Repositories.Models.ListHub.Office>.Update
                    .Set(m => m.OfficeImageUrl, url)
                    .Set(m => m.IsUpdateByPortal, true)).Result;

            return updateResult.ModifiedCount > 0;
        }


        public List<Repositories.Models.ListHub.Office> GetOffice()
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);
                var startstr = "{$or: [";
                var endstr = "]}";
                var matchQuery = startstr + string.Join(",", "{}") + endstr;
                matchQuery = "{$and: [{$or: [{IsDeletedByPortal: {$exists: false}}, {IsDeletedByPortal: false}]}," + matchQuery + endstr;
                var projectQuery = "{'_id' : 0, 'OfficeId' : 1 , 'CorporateName' : 1 }";

                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
                var projecttDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);
                var builderListings = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
                   .Match(matchDoc)
                   .Project(projecttDoc)
                   .ToListAsync()
                   .Result.Select(m => BsonSerializer.Deserialize<Repositories.Models.ListHub.Office>(m));

                var result = new List<Repositories.Models.ListHub.Office>();
                foreach (var item in builderListings)
                {
                    result.Add(new Repositories.Models.ListHub.Office { CorporateName = item.CorporateName, OfficeId = item.OfficeId });
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public Repositories.Models.ListHub.Office GetOfficeDetailsByName(string officeName)
        {
            base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);
            return GetCollection().Find(m => m.Name.ToLower() == officeName.ToLower()).FirstOrDefaultAsync().Result;

        }


        public bool Validation(Repositories.Models.ListHub.Office offc)
        {
            base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);
            var result = GetCollection().Find<Repositories.Models.ListHub.Office>(m => m.OfficeId != offc.OfficeId && m.OfficeEmail == offc.OfficeEmail).FirstOrDefaultAsync().Result;
            return result == null ? true : false;
        }


        public string GetPreviousImageUrl(string uniqueId)
        {
            base.CollectionName = Convert.ToString(DbCollections.OfficeDetails);
            var matchQuery = "{'OfficeId':'" + uniqueId + "'}";
            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
            var projectQuery = "{_id : 0,'OfficeImageUrl' : '$OfficeImageUrl'}";
            var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);
            var aggregate = GetCollection().Aggregate(new AggregateOptions()
            {
                AllowDiskUse = true
            })
           .Match(matchDoc)
           .Project(projectDoc);

            var result = aggregate.FirstOrDefaultAsync().Result;
            if (result.ElementCount == 0)
            {
                return string.Empty;
            }
            else
            {
                return (string)result[0];
            }
        }
    }
}
