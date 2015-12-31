using System;
using System.Collections.Generic;
using System.Linq;
using Core.Repository;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Repositories.Interfaces.Admin.Users;
using Repositories.Models.Admin.User;
using Repositories.Models.DataTable;
using Repositories.Models.ListHub;
using Utility;
using Repositories.Models.Common;

namespace Core.Implementation.Admin.Users
{
    public class UserHandler : Repository<User>, IAgent
    {
        private new const string CollectionName = "";
        public UserHandler()
            : base(CollectionName)
        {
        }
        public UserHandler(IMongoDatabase database)
            : base(database, CollectionName)
        {

        }
        public UserHandler(string connectionString, string databaseName)
            : base(connectionString, databaseName, CollectionName)
        {
        }

        public bool InsertBulkUsers(List<User> users)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            return base.InsertBulk(users);
        }
        public bool UpdateUser(string participantId, string pathToUpdate, bool value)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);

            var updateResult = GetCollection().UpdateOneAsync(
                m => m.ParticipantId == participantId,
                Builders<User>.Update
                    .Set(pathToUpdate, value)
                    ).Result;

            return updateResult.ModifiedCount > 0;
        }
        public bool UpdateUser(User user)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            //var duplicateCheck = GetCollection().Find<User>(m => m.ParticipantId != user.ParticipantId);
            var updateResult = GetCollection().UpdateOneAsync(
                m => m.ParticipantId == user.ParticipantId,
                Builders<User>.Update
                    .Set(m => m.FirstName, user.FirstName)
                    .Set(m => m.LastName, user.LastName)
                    .Set(m => m.PrimaryContactPhone, user.PrimaryContactPhone)
                    .Set(m => m.OfficePhone, user.OfficePhone)
                    .Set(m => m.Email, user.Email)
                    .Set(m => m.WebsiteURL, user.WebsiteURL)
                    .Set(m => m.AgentDescription, user.AgentDescription)
                    .Set(m => m.Role, user.Role)
                    .Set(m => m.Roles, user.Roles)
                    .Set(m => m.OfficeName, user.OfficeName)
                    .Set(m => m.OfficeId, user.OfficeId)
                    .Set(m => m.ProfileImage, user.ProfileImage)
                    .Set(m => m.IsUpdateByPortal, true)
                    ).Result;

            return updateResult.ModifiedCount > 0;
        }

        public bool DeleteUser(string uniquid, bool isDeleted)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);

            var updateResult = GetCollection().UpdateOneAsync(
                m => m.ParticipantId == uniquid,
                Builders<User>.Update
                    .Set(m => m.IsDeletedByPortal, isDeleted)
                //.Set(m => m.IsEmailSend, false)
                //.Set(m => m.DeactivateDate, DateTime.Now)
                    ).Result;

            return updateResult.ModifiedCount > 0;
        }
        public User Login(string email, string password)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            var tmpList = new User();
            using (var cursor = GetCollection()
                .FindAsync(m => m.Email == email && m.Password == HashPassword.Encrypt(password))
                .Result)
            {
                while (cursor.MoveNextAsync().Result)
                {
                    tmpList = cursor.Current.FirstOrDefault();
                }
                return tmpList;
            }
        }

        public User GetAgentDetails(string participantId)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            var tmpList = new User();
            using (var cursor = GetCollection()
                .FindAsync(m => m.ParticipantId == participantId)
                .Result)
            {
                while (cursor.MoveNextAsync().Result)
                {
                    tmpList = cursor.Current.FirstOrDefault();
                }
                return tmpList;
            }
        }

        public User GetAgentDetailsByEmail(string AgentEmail)
        {
            if (!string.IsNullOrEmpty(AgentEmail))
            {


                base.CollectionName = Convert.ToString(DbCollections.UserDetails);
                return GetCollection().Find<User>(x => x.Email.ToLower() == AgentEmail.ToLower()).FirstOrDefaultAsync().Result;
            }
            else
            {
                return new User();
            }
        }
        public bool InitiateRegistration(string participateId, string email)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);

            var updateResult = GetCollection().UpdateOneAsync(
                m => m.ParticipantId == participateId && m.Email == email,
                Builders<User>.Update
                .Set(m => m.IsEmailSend, true)
                .Set(m => m.InitiateDate, DateTime.Now)).Result;

            return updateResult.ModifiedCount > 0;
        }
        public bool SetPassword(string participateId, string email, string hashPassword)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);

            var updateResult = GetCollection().UpdateOneAsync(
                m => m.ParticipantId == participateId && m.Email == email,
                Builders<User>.Update
                .Set(m => m.Password, hashPassword)
                .Set(m => m.IsActive, true)
                .Set(m => m.ActivateDate, DateTime.Now)).Result;

            return updateResult.ModifiedCount > 0;
        }

        public bool DeactiveAgent(string uniquid, bool isActivated)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);

            var updateResult = GetCollection().UpdateOneAsync(
                m => m.ParticipantId == uniquid,
                Builders<User>.Update
                    .Set(m => m.IsActive, isActivated)
                    .Set(m => m.IsEmailSend, false)
                    .Set(m => m.DeactivateDate, DateTime.Now)).Result;

            return updateResult.ModifiedCount > 0;
        }

        public List<User> GetDataSet(string userEmail, JQueryDataTableParamModel dataTableParamModel,
            UserDataTable propertyDataTableSerachCriteria, out long filteredCount, string type = "")
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            var sortQuery = "";

            var matchQuery = "{$or : [{IsDeletedByPortal : {$exists : false} },{IsDeletedByPortal : false}]}";
            if (propertyDataTableSerachCriteria.sortColumnIndex == 0 && propertyDataTableSerachCriteria.isParticipantIdSortable)
            {
                sortQuery = propertyDataTableSerachCriteria.sortDirection == "asc" ? "{ParticipantId : 1}" : "{ParticipantId : -1}";
            }
            if (propertyDataTableSerachCriteria.sortColumnIndex == 1 && propertyDataTableSerachCriteria.isFirstNameSearchable)
            {
                sortQuery = propertyDataTableSerachCriteria.sortDirection == "asc" ? "{FirstName : 1}" : "{FirstName : -1}";
            }
            if (propertyDataTableSerachCriteria.sortColumnIndex == 2 && propertyDataTableSerachCriteria.isLastNameSortable)
            {
                sortQuery = propertyDataTableSerachCriteria.sortDirection == "asc" ? "{LastName : 1}" : "{LastName : -1}";
            }
            if (propertyDataTableSerachCriteria.sortColumnIndex == 3 && propertyDataTableSerachCriteria.isEmailSearchable)
            {
                sortQuery = propertyDataTableSerachCriteria.sortDirection == "asc" ? "{Email : 1}" : "{Email : -1}";
            }
            if (propertyDataTableSerachCriteria.sortColumnIndex == 4 && propertyDataTableSerachCriteria.isPrimaryContactPhoneSortable)
            {
                sortQuery = propertyDataTableSerachCriteria.sortDirection == "asc" ? "{PrimaryContactPhone : 1}" : "{PrimaryContactPhone : -1}";
            }
            if (propertyDataTableSerachCriteria.sortColumnIndex == 5 && propertyDataTableSerachCriteria.isOfficePhoneSortable)
            {
                sortQuery = propertyDataTableSerachCriteria.sortDirection == "asc" ? "{OfficePhone : 1}" : "{OfficePhone : -1}";
            }

            if (!string.IsNullOrEmpty(dataTableParamModel.sSearch))
            {
                var startstr = "{$or: [";
                var endstr = "]}";
                var listOfmatchQuery = new List<string>();

                if (propertyDataTableSerachCriteria.isParticipantIdSearchable)
                {
                    listOfmatchQuery.Add("{'ParticipantId': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (propertyDataTableSerachCriteria.isFirstNameSearchable)
                {
                    listOfmatchQuery.Add("{'FirstName': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (propertyDataTableSerachCriteria.isLastNameSearchable)
                {
                    listOfmatchQuery.Add("{'LastName': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (propertyDataTableSerachCriteria.isEmailSearchable)
                {
                    listOfmatchQuery.Add("{'Email': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (propertyDataTableSerachCriteria.isPrimaryContactPhoneSearchable)
                {
                    listOfmatchQuery.Add("{'PrimaryContactPhone': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
                }
                if (propertyDataTableSerachCriteria.isOfficePhoneSearchable)
                {
                    listOfmatchQuery.Add("{'OfficePhone': {'$regex': '" + dataTableParamModel.sSearch + "', '$options': 'i' }}");
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
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);

            return GetCollection().CountAsync(new BsonDocument()).Result;
        }


        public bool UploadProfileImage(string columnName, string UpdateType, string uniquid, string url)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            bool IsUpdatedByAgent = false;
            if (columnName == "LogoImage" && UpdateType != "Office")
            {
                IsUpdatedByAgent = true;
            }
            var updateResult = GetCollection().UpdateOneAsync(
                m => m.ParticipantId == uniquid,
                Builders<User>.Update
                    .Set(columnName, url)
                    .Set(m => m.IsUpdateByPortal, true)
                    .Set(m => m.IsUpdatedByAgent, IsUpdatedByAgent)).Result;

            return updateResult.ModifiedCount > 0;
        }


        public bool ResetPassword(string userid, string oldpwd, string newpwd)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            var updateResult = GetCollection()
                                .UpdateOneAsync(m =>
                                    m.Id == new ObjectId(userid) &&
                                    m.Password == HashPassword.Encrypt(oldpwd),
                                     Builders<User>.Update
                    .Set(m => m.Password, HashPassword.Encrypt(newpwd))).Result;
            return updateResult.ModifiedCount > 0;
        }

        public bool FeaturedAgent(string uniquid, bool isActivated)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);

            var updateResult = GetCollection().UpdateOneAsync(
                m => m.ParticipantId == uniquid,
                Builders<User>.Update
                    .Set(m => m.IsFeatured, isActivated)).Result;

            return updateResult.ModifiedCount > 0;
        }
        public User IsAgentFeatured(string emailId)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            return GetCollection().Find<User>(m => m.Email == emailId).FirstOrDefaultAsync().Result;
            //if (aggregate == null)
            //{
            //    return false;
            //}
            //else
            //{
            //    return aggregate.IsFeatured;
            //}
        }


        public bool Insert(User user)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            base.Create(user);
            return true;
        }


        public bool UpSertFromFeed(User user)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            var _user = GetCollection().Find<User>(m => m.Email == user.Email).FirstOrDefaultAsync().Result;
            if (_user == null)
            {
                this.Insert(user);
                return true;
            }
            else if (!_user.IsUpdateByPortal)
            {
                user.ParticipantId = _user.ParticipantId;
                user.Id = _user.Id;

                var _result = GetCollection().ReplaceOneAsync(m => m.Id == _user.Id, user).Result;
                return _result.ModifiedCount > 0;
            }
            return false;
        }


        public bool UpdateAgentListHub(User user)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);

            var _user = GetCollection().Find<User>(m => m.Email == user.Email).FirstOrDefaultAsync().Result;
            if (_user == null)
            {
                this.Insert(user);
            }
            else
            {
                // as the values are changed after excute feed in purchase and rent collection, so it is necesseary to change the values in previous form
                if (_user.IsUpdateByPortal)
                {
                    return false;
                }
            }

            return true;
        }



        public List<KeyValuePair<string, string>> GetBuilderList()
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            var startstr = "{$or: [";
            var endstr = "]}";
            var matchQuery = startstr + string.Join(",", "{'Roles' : 'Builder'}") + endstr;
            matchQuery = "{$and: [{$or: [{IsDeletedByPortal: {$exists: false}}, {IsDeletedByPortal: false}]}," + matchQuery + endstr;
            var projectQuery = "{'_id' : 0, 'FirstName' : 1 ,'LastName':1, 'ParticipantId' : 1 }";

            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
            var projecttDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);
            var builderListings = GetCollection().Aggregate(new AggregateOptions()
            {
                AllowDiskUse = true
            })
               .Match(matchDoc)
               .Project(projecttDoc)
               .ToListAsync()
               .Result.Select(m => BsonSerializer.Deserialize<User>(m));

            var result = new List<KeyValuePair<string, string>>();
            foreach (var item in builderListings)
            {
                result.Add(new KeyValuePair<string, string>(item.ParticipantId, item.FirstName + item.LastName));
            }
            return result;
        }

        public List<User> GetAgents(string officeId)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);

            var startstr = "{$or: [";
            var endstr = "]}";
            var matchQuery = startstr + string.Join(",", "{'OfficeId' : '" + officeId + "'}") + endstr;
            matchQuery = "{$and: [{$or: [{IsDeletedByPortal: {$exists: false}}, {IsDeletedByPortal: false}]}," + matchQuery + endstr;

            var projQuery =
                            @"{'_id' : 1, 
                            'ParticipantKey' : 1, 
                            'ParticipantId' : 1, 
                            'FirstName' : 1, 
                            'LastName' : 1, 
                            'Role' : 1, 
                            'PrimaryContactPhone' : 1, 
                            'OfficePhone' : 1, 
                            'Email' : 1, 
                            'WebsiteURL' : 1, 
                            'IsActive' : 1, 
                            'IsEmailSend' : 1, 
                            'ActivateDate' : 1, 
                            'DeactivateDate' : 1, 
                            'InitiateDate' : 1, 
                            'IsCertified' : 1, 
                            'IsFeatured' : 1, 
                            'Roles' : 1, 
                            'AgentDescription' : 1, 
                            'OfficeName' : 1, 
                            'OfficeId' : 1, 
                            'ProfileImage' : 1, 
                            'IsUpdateByPortal' : 1,
                            'IsDeletedByPortal' : 1}";

            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
            var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projQuery);

            var builderListings = GetCollection().Aggregate(new AggregateOptions()
            {
                AllowDiskUse = true
            })
               .Match(matchDoc)
               .Project(projQuery)
               .ToListAsync()
               .Result.Select(m => BsonSerializer.Deserialize<User>(m)).ToList();


            return builderListings;
        }


        public bool CertifiedAgent(string uniquid, bool isActivated)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);

            var updateResult = GetCollection().UpdateOneAsync(
                m => m.ParticipantId == uniquid,
                Builders<User>.Update
                    .Set(m => m.IsCertified, isActivated)).Result;

            return updateResult.ModifiedCount > 0;
        }



        public List<User> GetAgents()
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            return GetCollection().Find(new BsonDocument()).ToListAsync().Result;
        }


        public bool Validation(User user)
        {

            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            var result = GetCollection().Find<User>(m => m.ParticipantId != user.ParticipantId && (m.Email == user.Email)).FirstOrDefaultAsync().Result;
            return result == null ? true : false;
        }

        public List<User> GetCertifiedAgents(int count)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            var matchQuery = @"
                               {
                                  $or: [
                                        {
                                          $and: [ 
          		                                   {'IsCertified' : true},
                                                   {'IsDeletedByPortal' : {$exists : false}}
                                                ]
                                        },
                                        {
                                          $and: [  
                                                   {'IsCertified' : true},
                                                   {'IsDeletedByPortal' : {$exists : true}},
                                                   {'IsDeletedByPortal' : false}
                                                ]
                                        }
                                       ]
                               }
                              ";
            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
            var results = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
                .Match(matchDoc)
                .Limit(count)
                .ToListAsync()
                .Result;


            return results;
        }

        public List<User> GetFeaturedAgents(int count)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            var matchQuery = @"
                               {
                                  $or: [
                                        {
                                          $and: [ 
          		                                   {'IsFeatured' : true},
                                                   {'IsDeletedByPortal' : {$exists : false}}
                                                ]
                                        },
                                        {
                                          $and: [  
                                                   {'IsFeatured' : true},
                                                   {'IsDeletedByPortal' : {$exists : true}},
                                                   {'IsDeletedByPortal' : false}
                                                ]
                                        }
                                       ]
                               }
                              ";
            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
            var results = GetCollection().Aggregate(new AggregateOptions()
            {
                AllowDiskUse = true
            })
                .Match(matchDoc)
                .Limit(count)
                .ToListAsync()
                .Result;


            return results;
        }

        public IEnumerable<AutoCompleteDetails> GetAgentList(string searchKey)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.UserDetails);

                var matchQuery = "{'FullName': {'$regex': '" + searchKey + "', '$options': 'i' }}";
                var project1Query = "{_id : 0,'FirstName' : 1,'FullName':{ $concat: [ '$FirstName', ' ', '$LastName' ] }}";
                var project2Query = "{'FullName':{$cond: [ {$ne: [ '$FullName', null ] }, '$FullName', '$FirstName' ]}}";
                var group = "{_id: null,'CityWithStateAndPostCode':{$addToSet: '$FullName'}}";
                var projectQuery = "{_id : 0,CityWithStateAndPostCode : 1}";
                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);

                var project1Doc = BsonSerializer.Deserialize<BsonDocument>(project1Query);
                var project2Doc = BsonSerializer.Deserialize<BsonDocument>(project2Query);
                var groupDoc = BsonSerializer.Deserialize<BsonDocument>(group);
                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

                var aggregate = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
                
                    .Project(project1Doc)
                    .Project(project2Doc)
                    .Match(matchDoc)
                    .Group(groupDoc)
                    .Project(projectDoc)
                    .ToListAsync()
                    .Result.Select(m => BsonSerializer.Deserialize<AutoCompleteDetails>(m));

                return aggregate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string GetPreviousImageUrl(string columnName, string uniqueId)
        {
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            var matchQuery = "{'ParticipantId':'" + uniqueId + "'}";
            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);

            if (columnName == "LogoImage")
            {
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
            else
            {
                var projectQuery = "{_id : 0,'ProfileImage' : '$ProfileImage'}";
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


        public User GetAgentDetailsByName(string name)
        {
            var names = name.Split(' ');
            var firstName = names[0];
            var lastName = names[1];
            base.CollectionName = Convert.ToString(DbCollections.UserDetails);
            var tmpList = new User();
            using (var cursor = GetCollection()
                .FindAsync(m => m.FirstName == firstName && m.LastName == lastName)
                .Result)
            {
                while (cursor.MoveNextAsync().Result)
                {
                    tmpList = cursor.Current.FirstOrDefault();
                }
                return tmpList;
            }
        }
    }
}
