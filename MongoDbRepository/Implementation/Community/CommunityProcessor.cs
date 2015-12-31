using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Excel;
using Repositories.Interfaces.Community;
using Repositories.Models;
using Repositories.Models.Community;
using Core.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using Utility;
using Repositories.Interfaces.Admin.ListHub;
using MongoDB.Bson.Serialization;

namespace Core.Implementation.Community
{
    public class CommunityProcessor : Repository<Communities>, ICommunityProvider
    {
        private new const string CollectionName = "";


        public CommunityProcessor()
            : base(CollectionName)
        {
        }

        public CommunityProcessor(IMongoDatabase database)
            : base(database, CollectionName)
        {

        }

        public CommunityProcessor(string connectionString, string databaseName)
            : base(connectionString, databaseName, CollectionName)
        {
        }


        public void Process(string filePath)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            excelReader.IsFirstRowAsColumnNames = true;
            var result = excelReader.AsDataSet().Tables[0];


            var communities = (from DataRow row in result.Rows
                               select new Communities
                               {
                                   CommunityName = Convert.ToString(row["Community"]),
                                   Zips = new string[]
                                   {
                                       Convert.ToString(row["Zip 1"]),
                                        Convert.ToString(row["Zip 1"]),
                                        Convert.ToString(row["Zip 2"]),
                                        Convert.ToString(row["Zip 3"]),
                                        Convert.ToString(row["Zip 4"]),
                                        Convert.ToString(row["Zip 5"]),
                                        Convert.ToString(row["Zip 6"]),
                                        Convert.ToString(row["Zip 7"]),
                                        Convert.ToString(row["Zip 8"])
                                   },
                                   City = Convert.ToString(row["City"]),
                                   Region = Convert.ToString(row["Region"])
                               }).ToList();

            excelReader.Close();
            
            /*Check if community exists and then remove it from inserted list*/
            foreach (var item in communities)
            {
                if(this.GetCommunitiesByName(item.CommunityName) != null)
                {
                    communities.Remove(item);
                }
            }
            this.InsertCommunities(communities);
        }

        public void InsertCommunities(List<Communities> communities)
        {
            base.CollectionName = Convert.ToString(DbCollections.Communities);
            base.InsertBulk(communities);
        }


        public string[] GetCommunityName(string postalCode)
        {
            try
            {
                var jsonQuery = "{'Zips' : {$in : ['" + postalCode + "']}}";
                var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(jsonQuery);
                base.CollectionName = Convert.ToString(DbCollections.Communities);
                var tmpList = new List<string>();
                using (var cursor = GetCollection().FindAsync<Communities>(doc).Result)
                {
                    while (cursor.MoveNextAsync().Result)
                    {
                        tmpList.AddRange(cursor.Current.Select(m => m.CommunityName).ToList());
                    }

                    return tmpList.ToArray();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Communities> GetCommunities()
        {
            try
            {
                var jsonQuery = "{}";
                var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(jsonQuery);
                base.CollectionName = Convert.ToString(DbCollections.Communities);
                var tmpList = new List<Communities>();

                using (var cursor = GetCollection().FindAsync<Communities>(doc, new FindOptions<Communities>
                                                                                {

                                                                                    Projection = Builders<Communities>.Projection
                                                                                                .Include(p => p.CommunityName)
                                                                                                .Exclude(p => p.Id)
                                                                                }).Result)
                    {
                        while (cursor.MoveNextAsync().Result)
                        {
                            tmpList.AddRange(cursor.Current.ToList());
                        }

                        return tmpList;
                    }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Repositories.Models.Community.Communities GetCommunities(string communityId)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.Communities);

                var aggregate = GetCollection().Find<Repositories.Models.Community.Communities>(m => m.CommunityId == communityId).FirstOrDefaultAsync().Result;
                return aggregate;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Repositories.Models.Community.Communities GetCommunitiesByName(string communityName)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.Communities);

                var aggregate = GetCollection().Find<Repositories.Models.Community.Communities>(m => m.CommunityName.ToLower() == communityName.ToLower()).FirstOrDefaultAsync().Result;
                return aggregate;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<Communities> GetCommunitiesList()
        {
            try
            {
                var jsonQuery = "{}";
                var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(jsonQuery);
                base.CollectionName = Convert.ToString(DbCollections.Communities);
                var tmpList = new List<Communities>();
                using (var cursor = GetCollection().FindAsync<Communities>(doc).Result)
                {
                    while (cursor.MoveNextAsync().Result)
                    {
                        tmpList.AddRange(cursor.Current);
                    }

                    return tmpList;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<Communities> GetDataSet(string userEmail, Repositories.Models.DataTable.JQueryDataTableParamModel dataTableParamModel, Repositories.Models.Admin.Community.CommunityDataTable serachCriteria, out long filteredCount, string type = "")
        {
            base.CollectionName = Convert.ToString(DbCollections.Communities);
            var sortQuery = "";
            var matchQuery = "{$or : [{IsDeletedByPortal : {$exists : false} },{IsDeletedByPortal : false}]}";
            if (serachCriteria.sortColumnIndex == 0 && serachCriteria.isIdSortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{_id : 1}" : "{_id : -1}";
            }
            if (serachCriteria.sortColumnIndex == 1 && serachCriteria.isNameSortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{CommunityName : 1}" : "{CommunityName : -1}";
            }
            if (serachCriteria.sortColumnIndex == 2 && serachCriteria.isNumberSortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{Number : 1}" : "{Number : -1}";
            }
            if (serachCriteria.sortColumnIndex == 3 && serachCriteria.isAddressSortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{Address : 1}" : "{Address : -1}";
            }
            if (serachCriteria.sortColumnIndex == 4 && serachCriteria.isCitySortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{City : 1}" : "{City : -1}";
            }
            if (serachCriteria.sortColumnIndex == 5 && serachCriteria.isEmailSortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{Email : 1}" : "{Email : -1}";
            }
            if (serachCriteria.sortColumnIndex == 6 && serachCriteria.isPhoneSortable)
            {
                sortQuery = serachCriteria.sortDirection == "asc" ? "{PhoneNo : 1}" : "{PhoneNo : -1}";
            }

            if (!string.IsNullOrEmpty(dataTableParamModel.sSearch))
            {
                var startstr = "{$or: [";
                var endstr = "]}";
                var listOfmatchQuery = new List<string>();

                if (serachCriteria.isIdSearchable)
                {
                    listOfmatchQuery.Add("{'_id': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (serachCriteria.isNameSearchable)
                {
                    listOfmatchQuery.Add("{'CommunityName': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (serachCriteria.isNumberSearchable)
                {
                    listOfmatchQuery.Add("{'Number': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (serachCriteria.isAddressSearchable)
                {
                    listOfmatchQuery.Add("{'Address': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (serachCriteria.isCitySearchable)
                {
                    listOfmatchQuery.Add("{'City': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (serachCriteria.isEmailSearchable)
                {
                    listOfmatchQuery.Add("{'Email': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (serachCriteria.isPhoneSearchable)
                {
                    listOfmatchQuery.Add("{'PhoneNo': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
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
            base.CollectionName = Convert.ToString(DbCollections.Communities);

            return GetCollection().CountAsync(new BsonDocument()).Result;
        }


        public bool InsertFromFeed(List<Communities> communities)
        {
            foreach (var item in communities)
            {
                var community = this.GetCommunitiesByName(item.CommunityName);
                if (community == null)
                {
                    this.Create(item);
                }
            }
            return true;
        }


        public void UpdateCommunities(Communities communities)
        {
            base.CollectionName = Convert.ToString(DbCollections.Communities);
            var delResult = GetCollection().UpdateOneAsync(
                            m => m.CommunityId == communities.CommunityId,
                            Builders<Communities>.Update
                                     .Unset(m => m.Zips)
                                ).Result;

            var updateResult = GetCollection().UpdateOneAsync(
                m => m.CommunityId == communities.CommunityId,
                Builders<Communities>.Update
                    .Set(m => m.CommunityName, communities.CommunityName)
                    .Set(m => m.Address, communities.Address)
                    .Set(m => m.City, communities.City)
                    .Set(m => m.State, communities.State)
                    .Set(m => m.Email, communities.Email)
                    .Set(m => m.WebSite, communities.WebSite)
                    .Set(m => m.PhoneNo, communities.PhoneNo)
                    .Set(m => m.Description, communities.Description)
                    .Set(m => m.LogoImage, communities.LogoImage)
                    .Set(m => m.IsUpdateByPortal, true)
                    .PushEach(m => m.Zips, communities.Zips)
                    ).Result;
        }


        public void DeleteCommunities(string communityId)
        {
            base.CollectionName = Convert.ToString(DbCollections.Communities);
            var updateResult = GetCollection().UpdateOneAsync(
                m => m.CommunityId == communityId,
                Builders<Communities>.Update
                    .Set(m => m.IsDeletedByPortal, true)
                    ).Result;
        }


        public bool Validation(Communities com)
        {
            base.CollectionName = Convert.ToString(DbCollections.Communities);
            var result = GetCollection().Find<Communities>(m => m.CommunityId != com.CommunityId && m.CommunityName == com.CommunityName).FirstOrDefaultAsync().Result;
            return result == null ? true : false;
        }


        public string GetPreviousImageUrl(string communityId)
        {
            base.CollectionName = Convert.ToString(DbCollections.Communities);
            var matchQuery = "{'CommunityId':'" + communityId + "'}";
            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
            var projectQuery = "{_id : 0,'LogoImage' : '$LogoImage'}";
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
