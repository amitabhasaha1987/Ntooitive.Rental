using System;
using System.Collections.Generic;
using System.Linq;
using Core.Repository;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Repositories.Interfaces.Map;
using Repositories.Interfaces.NewHome;
using Repositories.Models.Common;
using Repositories.Models.NewHome;
using Repositories.Models.ViewModel;
using Utility;

namespace Core.Implementation.NewHome
{
    public class NewHomeFeed : Repository<Plan>, INewHome
    {
        private readonly IFetchLatLong _fetchLatLong;

        private new const string CollectionName = "";
        public NewHomeFeed()
            : base(CollectionName)
        {
        }
        public NewHomeFeed(IMongoDatabase database, IFetchLatLong fetchLatLong)
            : base(database, CollectionName)
        {
            _fetchLatLong = fetchLatLong;

        }
        public NewHomeFeed(string connectionString, string databaseName)
            : base(connectionString, databaseName, CollectionName)
        {
        }

        public bool UpdateNewHomePlan(Home home, string planid)
        {
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
            var filter = Builders<Plan>.Filter.Eq("PlanId", planid);
            var update = Builders<Plan>.Update.Push("Homes.Home", home);
            var result = GetCollection().UpdateOneAsync(filter, update).Result;
            return result.ModifiedCount > 0;
        }

        public bool UpdateHome(Home home, string planid)
        {
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
            var plan = new Plan();
            plan = GetCollection().Find(m => m.PlanId == planid).FirstOrDefaultAsync().Result;
            var index = plan.Homes.Home.FindIndex(x => x.Id == home.Id);

            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
            var filter = Builders<Plan>.Filter.Eq("Homes.Home._id", home.Id);
            var update = Builders<Plan>.Update
                .Set("Homes.Home." + index + ".Home_type", home.Home_type)
                .Set("Homes.Home." + index + ".Property_type", home.Property_type)
                .Set("Homes.Home." + index + ".Sales_status", home.Sales_status)
                .Set("Homes.Home." + index + ".Lot", home.Lot)
                .Set("Homes.Home." + index + ".Lot_size", home.Lot_size)
                .Set("Homes.Home." + index + ".Address", home.Address)
                .Set("Homes.Home." + index + ".State", home.State)
                .Set("Homes.Home." + index + ".City", home.City)
                .Set("Homes.Home." + index + ".Zip", home.Zip)
                .Set("Homes.Home." + index + ".County", home.County)
                .Set("Homes.Home." + index + ".Country", home.Country)
                .Set("Homes.Home." + index + ".Latitude", home.Latitude)
                .Set("Homes.Home." + index + ".Longitude", home.Longitude)
                .Set("Homes.Home." + index + ".Term", home.Term)
                .Set("Homes.Home." + index + ".Price", home.Price)
                .Set("Homes.Home." + index + ".Sqft", home.Sqft)
                .Set("Homes.Home." + index + ".Stories", home.Stories)
                .Set("Homes.Home." + index + ".Unit_level", home.Unit_level)
                .Set("Homes.Home." + index + ".Number_of_units", home.Number_of_units)
                .Set("Homes.Home." + index + ".Year_built", home.Year_built)
                .Set("Homes.Home." + index + ".Directions", home.Directions)
                .Set("Homes.Home." + index + ".Master_bedroom_location", home.Master_bedroom_location)
                .Set("Homes.Home." + index + ".Bedrooms", home.Bedrooms)
                 .Set("Homes.Home." + index + ".Half_baths", home.Half_baths)
                .Set("Homes.Home." + index + ".Baths", home.Baths);

            var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;
            //query for select specific home from plan
            //var filter = Builders<Plan>.Filter.Eq("PlanId", planid, "Homes.Home");
            //var update = Builders<Plan>.Update.Push("Homes.Home", home);
            //var result = GetCollection().UpdateOneAsync(filter, update).Result;
            //return result.ModifiedCount > 0;
            //var matchQuery = "{Homes.Home._id : 'home.Id'}";
            //var group = "{_id: null,'CityWithStateAndPostCode':{$addToSet: '$FullAddress'}}";
            //var projectQuery = "{Homes : '$Homes.Home'}";
            //var matchQuery1 = "{Home._id : 'home.Id'}";

            //var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
            //var matchDoc1 = BsonSerializer.Deserialize<BsonDocument>(matchQuery1);
            //var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

            //var aggregate = GetCollection().Aggregate(new AggregateOptions()
            //{
            //    AllowDiskUse = true
            //})
            //.Match(matchDoc)
            //.Match(matchDoc1)
            //.Unwind(m => m.Homes.Home)
            //.Project(projectDoc).ToListAsync().Result.Select(m => BsonSerializer.Deserialize<Home>(m));

            return results.ModifiedCount > 0;
        }

        public IEnumerable<string> GetPropertyType()
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
                var tmpList = new List<string>();
                using (
                    var cursor = GetCollection().DistinctAsync<string>("Homes.Home.Property_type", new BsonDocument()).Result)
                {
                    while (cursor.MoveNextAsync().Result)
                    {
                        tmpList.AddRange(cursor.Current.ToList());
                    }

                    return tmpList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<Plan> ReleatedPlans(string communityName)
        {
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
            return GetCollection().Find(m => m.CommunityName == communityName).ToListAsync().Result;
        }

        public Plan PlanDetails(string plainId)
        {
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
            return GetCollection().Find(m => m.PlanId == plainId).FirstOrDefaultAsync().Result;
        }

        public bool InsertBulkNewHomes(List<Plan> homeListings)
        {
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
            return InsertBulk(homeListings);
        }

        public IEnumerable<AutoCompleteDetails> GetNewHomeAddressList(string searchKey)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

                var matchQuery = "{'FullAddress': {'$regex': '" + searchKey + "', '$options': 'i' }}";
                var group = "{_id: null,'CityWithStateAndPostCode':{$addToSet: '$FullAddress'}}";
                var projectQuery = "{_id : 0,CityWithStateAndPostCode : 1}";

                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
                var groupDoc = BsonSerializer.Deserialize<BsonDocument>(group);
                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

                var aggregate = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
                .Match(matchDoc)
                .Group(groupDoc)
                .Project(projectDoc).ToListAsync().Result.Select(m => BsonSerializer.Deserialize<AutoCompleteDetails>(m));

                return aggregate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PropertyDetails GetNewHomeRecordDetailsByMLSNumber(string mlsNumber)
        {
            try
            {
                //'MlsNumber':1,
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

                var projectQuery1 = " { _id : 0,Home : '$Homes.Home'}";
                var matchQuery = "{'Home._id': '" + mlsNumber + "'}";
                var projectQuery =
                    "{_id : 0,'FullStreetAddress' : '$Home.Address','City':'$Home.City'," +
                    "'StateOrProvince':'$Home.State','PostalCode':'$Home.Zip'," +
                    "'Location':'$Home.Location','BrokerageName':'$Home.Broker_name'," +
                    "'BrokerageEmail':'$Home.Broker_email','ListPrice':'$Home.Price','Bedrooms':'$Home.Bedrooms'," +
                    "'Bathrooms':'$Home.Baths','PropertyType':'$Home.Property_type','HalfBathrooms':'$Home.Half_baths'," +
                    "'LivingArea':'$Home.Sqft','MlsNumber':'$Home._id','BrokeragePhone':'$Home.Broker_phone'," +
                    "'BrokerageWebsite':'$Home.Broker_website','BrokerageLogoURL':'$Home.Broker_logo_url','VirtualTour':'$Home.Home_virtual_tours'," +
                    "'ElevationImages':'$Home.Home_videos','LotSize':{ $cond: { if: '$Home.Lot', then: 0, else: '$Home.Lot' } }," +
                    "'NoOfParkingSpace':'$Home.Garage','Photos':'$Home.Images.Image.Reference','GeoPoint':'$GeoPoint','ListingStatus':{ $cond: { if: '$Home.Is_active', then: 'Active', else: 'De-Active' }}}";


                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);
                var project1Doc = BsonSerializer.Deserialize<BsonDocument>(projectQuery1);

                var aggregate = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
                .Unwind(m => m.Homes.Home)
                .Project(project1Doc)
               .Match(matchDoc)
               .Project(projectDoc);
                var myObj = BsonSerializer.Deserialize<PropertyDetails>(aggregate.FirstOrDefaultAsync().Result);

                //var projectQuery1Geo = " { _id : 0,Home : '$Homes.Home'}";
                var matchQueryGeo = "{'Homes.Home._id': '" + mlsNumber + "'}";
                var projectQueryGeo =
                    "{_id : 0,'GeoPoint':'$GeoPoint'}";


                var matchDocGeo = BsonSerializer.Deserialize<BsonDocument>(matchQueryGeo);
                var projectDocGeo = BsonSerializer.Deserialize<BsonDocument>(projectQueryGeo);
                //var project1DocGeo = BsonSerializer.Deserialize<BsonDocument>(projectQuery1Geo);

                var aggregateGeo = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
                    //.Unwind(m => m.Homes.Home)
                    //.Project(project1DocGeo)
               .Match(matchDocGeo)
               .Project(projectDocGeo);
                var myObjGeo = BsonSerializer.Deserialize<PropertyDetails>(aggregateGeo.FirstOrDefaultAsync().Result);
                myObj.GeoPoint = myObjGeo.GeoPoint;
                return myObj;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Plan> GetNewHomeRecordList(string matchquery, string sortquery, int limit = 0, int skip = 0)
        {
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchquery);
            var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sortquery);


            var aggregate = GetCollection().Aggregate(new AggregateOptions()
            {
                AllowDiskUse = true
            })
                        .Match(matchDoc)
                        .Sort(sortDoc)
                        .Skip(skip)
                        .Limit(limit);
            // .Result;
            return aggregate
                        .ToListAsync().Result;

        }

        public long GetNewHomeRecordCount(BsonDocument matchDoc)
        {
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

            return GetCollection().CountAsync(matchDoc).Result;

        }

        //need to change
        public List<Plan> GetPlans(string builderId)
        {
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
            var tmpList = new List<Plan>();
            using (var cursor = GetCollection()
                .FindAsync(m => m.BuilderNumber == builderId)
                .Result)
            {
                while (cursor.MoveNextAsync().Result)
                {
                    var currentCursor = cursor.Current.FirstOrDefault();
                    if (currentCursor != null)
                    {
                        //tmpList = currentCursor.Plans.Plan;
                    }
                }
                return tmpList;
            }
        }

        public IEnumerable<PropertyListing> GetNewHomeRecordList(int skip, int limit, bool IsMls, AdvanceSearch advanceSearch)
        {
            return new List<PropertyListing>();
            /*
            try
            {
                bool isGeoQuery = false;
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

                var tempList = new HashSet<string>();
                if (advanceSearch.Location.Contains(","))
                {
                    foreach (var searcgString in advanceSearch.Location.Split(','))
                    {
                        foreach (var searchString in searcgString.Split(' '))
                        {
                            if (searchString != "")
                            {
                                tempList.Add(searchString.ToLower().Trim());
                            }
                        }
                    }
                }
                else
                {
                    foreach (var searchString in advanceSearch.Location.Split(' '))
                    {
                        tempList.Add(searchString.ToLower().Trim());
                    }
                }

                var allowedkeywords = '[' + string.Join(",", tempList) + ']';
                allowedkeywords = allowedkeywords.Replace("[", "['");
                allowedkeywords = allowedkeywords.Replace("]", "']");
                allowedkeywords = allowedkeywords.Replace(",", "','");
                if (advanceSearch.NearByDistance > 0)
                {
                    isGeoQuery = true;

                }

                var matchQuery = IsMls
                    ? "{'MlsNumber': {'$regex': '" + advanceSearch.Location + "', '$options': 'i' }}"
                    : !isGeoQuery ? "{'Subdivision.SubAddress.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryStringForNewListing(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                          advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size, advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + "}" :
                          "{" + UtilityClass.CreateQueryStringForNewListing(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                          advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size, advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + "}";

                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);


                var projectQuery =
                    "{_id : 0,'objId':'$_id','FullStreetAddress':'$Subdivision.SubAddress.SubStreet1'," +
                    "'Price':'$Subdivision.PriceLow','Plans':'$Subdivision.Plan'," +
                    "'BrokerageName':'$BrandName','BrokerageLogoUrl':'$BrandLogoMed.Text','ListingParticipantsName':'$ReportingName','DefaultPhoto':'$Subdivision.SubImage.Text'," +
                    "'GeoLocation':'$Subdivision.SubAddress.GeoPoint','State':'$Subdivision.SubAddress.SubState'," +
                    "'PostalCode':'$Subdivision.SubAddress.SubZip','City':'$Subdivision.SubAddress.SubCity'}";


                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);
                IEnumerable<PropertyListing> aggregate;
                var sortBy = advanceSearch.SortBy == "0" ? "{Price : 1}" : "{" + Utility.UtilityClass.GetsortColumn(advanceSearch.SortBy.Split('_')[0]) + ":" + (advanceSearch.SortBy.Split('_')[1] == "Asc" ? 1 : -1) + "}";
                var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sortBy);
                if (isGeoQuery)
                {
                    var geolocation = GetLatLong(allowedkeywords);

                    var geoPoint = new BsonDocument
                    {
                        {"type", "Point"},
                        {"coordinates", new BsonArray(new double[] {geolocation.Longitude, geolocation.Latitude})}
                    };
                    var maxDistanceInMiles = advanceSearch.NearByDistance * 1609.34;

                    var geoNearOptions = new BsonDocument
                    {
                        {"near", geoPoint},
                        {"distanceField", "CalculatedDistance"},
                        {"maxDistance", Convert.ToDouble(maxDistanceInMiles)},
                        {"distanceMultiplier", Convert.ToDouble("0.000621371")},
                        {"query", matchDoc},
                        {"limit", 3000},
                        {"spherical", true},
                    };

                    var stage =
                        new BsonDocumentPipelineStageDefinition<NewHomeListing, NearbyArea>(
                            new BsonDocument { { "$geoNear", geoNearOptions } });
                    aggregate = GetCollection().Aggregate(new AggregateOptions()
                    {
                        AllowDiskUse = true
                    })
                        .AppendStage(stage)
                        .Project(projectDoc)
                        .Sort(sortDoc)
                        .Skip(skip)
                        .Limit(limit)
                        .ToListAsync().Result.Select(m => BsonSerializer.Deserialize<PropertyListing>(m));
                }
                else
                {
                    aggregate = GetCollection().Aggregate(new AggregateOptions()
                    {
                        AllowDiskUse = true
                    })
                        .Match(matchDoc)
                        .Project(projectDoc)
                        .Sort(sortDoc)
                        .Skip(skip)
                        .Limit(limit)
                        .ToListAsync()
                        .Result.Select(m => BsonSerializer.Deserialize<PropertyListing>(m));
                }
                return aggregate;
            }
            catch (Exception ex)
            {
                throw ex;
            }*/
        }

        public IEnumerable<PropertyTypeCheckBox> GetNewHomePropertyType()
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
                var tmpList = new List<string>();
                using (var cursor = GetCollection().DistinctAsync<string>("Style", new BsonDocument()).Result)
                {
                    while (cursor.MoveNextAsync().Result)
                    {
                        tmpList.AddRange(cursor.Current.ToList());
                    }
                    List<PropertyTypeCheckBox> lstPropertyTypeCheckBox = new List<PropertyTypeCheckBox>();
                    foreach (var item in tmpList)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                        PropertyTypeCheckBox propertyTypeCheckBox = new PropertyTypeCheckBox();
                        propertyTypeCheckBox.PropertyName = item;
                        propertyTypeCheckBox.IsSelected = false;
                        lstPropertyTypeCheckBox.Add(propertyTypeCheckBox);
                    }
                    }
                    return lstPropertyTypeCheckBox;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetNewHomeRecordCount(AdvanceSearch advanceSearch, bool IsMls)
        {
            /*
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
                bool isGeoQuery = false;
                var tempList = new HashSet<string>();
                if (advanceSearch.Location.Contains(","))
                {
                    foreach (var searcgString in advanceSearch.Location.Split(','))
                    {
                        foreach (var searchString in searcgString.Split(' '))
                        {
                            if (searchString != "")
                            {
                                tempList.Add(searchString.ToLower().Trim());
                            }
                        }
                    }
                }
                else
                {
                    foreach (var searchString in advanceSearch.Location.Split(' '))
                    {
                        tempList.Add(searchString.ToLower().Trim());
                    }
                }

                var allowedkeywords = '[' + string.Join(",", tempList) + ']';
                allowedkeywords = allowedkeywords.Replace("[", "['");
                allowedkeywords = allowedkeywords.Replace("]", "']");
                allowedkeywords = allowedkeywords.Replace(",", "','");
                if (advanceSearch.NearByDistance > 0)
                {
                    isGeoQuery = true;

                }

                var matchQuery = IsMls
                    ? "{'MlsNumber': {'$regex': '" + advanceSearch.Location + "', '$options': 'i' }}"
                    : !isGeoQuery ? "{'Subdivision.SubAddress.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryStringForNewListing(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                          advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size, advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + "}" :
                          "{" + UtilityClass.CreateQueryStringForNewListing(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                          advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size, advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + "}";

                //var matchQuery = IsMls
                //    ? "{'MlsNumber': {'$regex': '" + advanceSearch.Location + "', '$options': 'i' }}"
                //    : !isGeoQuery ? "{'Subdivision.SalesOffice.Address.AddressArray': { $all : " + allowedkeywords + "}" +
                //      null + "}" :
                //          "{" + null + "}";
                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);


                int aggregate = 0;
                if (isGeoQuery)
                {
                    var geolocation = GetLatLong(allowedkeywords);

                    var geoPoint = new BsonDocument
                    {
                        {"type", "Point"},
                        {"coordinates", new BsonArray(new double[] {geolocation.Longitude, geolocation.Latitude})}
                    };
                    var maxDistanceInMiles = advanceSearch.NearByDistance * 1609.34;

                    var geoNearOptions = new BsonDocument
                    {
                        {"near", geoPoint},
                        {"distanceField", "CalculatedDistance"},
                        {"maxDistance", Convert.ToDouble(maxDistanceInMiles)},
                        {"distanceMultiplier", Convert.ToDouble("0.000621371")},
                        {"query", matchDoc},
                        {"limit", 3000},
                        {"spherical", true},
                    };

                    var stage =
                        new BsonDocumentPipelineStageDefinition<NewHomeListing, BsonDocument>(
                            new BsonDocument { { "$geoNear", geoNearOptions } });
                    aggregate = GetCollection().Aggregate(new AggregateOptions()
                    {
                        AllowDiskUse = true
                    })
                        .AppendStage(stage)
                        .ToListAsync()
                        .Result.Count();
                }
                else
                {
                    aggregate = GetCollection().Aggregate(new AggregateOptions()
                    {
                        AllowDiskUse = true
                    })
                       .Match(matchDoc).ToListAsync().Result.Count();
                }
                return aggregate;
            }
            catch (Exception ex)
            {
                throw ex;
            }*/
            return 0;
        }

        public IEnumerable<NearbyArea> GetNearByNewHomeAreaDetails(double latitude, double longitude, double maxDistanceInMiles)
        {
            /*
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

                BsonDocument geoPoint = new BsonDocument
                {
                    {"type","Point"},
                    {"coordinates",new BsonArray(new double[]{ longitude, latitude })}
                };
                var matchQuery = "{ 'Subdivision.SubAddress.SubCity' : {$ne :''},'Subdivision.SubAddress.SubState' : {$ne :''}}";
                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);

                //Here we are forming the Geo Near Options that consists of Maximum Distance that we have to search till, number of records to be fetched, earth Spherical property to True, calculate the distance from starting point.
                BsonDocument geoNearOptions = new BsonDocument
                {
                    {"near", geoPoint},
                    {"distanceField", "CalculatedDistance"},
                    {"maxDistance", Convert.ToDouble(maxDistanceInMiles)},
                    {"distanceMultiplier",Convert.ToDouble( "0.000621371")},
                    {"query", matchDoc},
                    //{"limit", 3000},             
                    {"spherical", true},
                };

                //MongoDB requires $geoNear as the first stage of a pipeline.
                var stage = new BsonDocumentPipelineStageDefinition<NewHomeListing, NearbyArea>(new BsonDocument { { "$geoNear", geoNearOptions } });

                var projectQuery = "{ _id : 0,CalculatedDistance : '$CalculatedDistance',CityWithState  :  { $concat : [ '$Subdivision.SubAddress.SubCity', ', ', '$Subdivision.SubAddress.SubState']}}";
                var groupQuery = "{_id :'$CityWithState',DistanceArray :  { $addToSet: '$CalculatedDistance' },AvgDistance : {$avg: '$CalculatedDistance'}}}";
                var sortQuery = "{ AvgDistance : -1}";

                var groupDoc = BsonSerializer.Deserialize<BsonDocument>(groupQuery);
                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);
                var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sortQuery);


                var aggregate = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
               .AppendStage(stage)
               .Project(projectDoc)
               .Group(groupDoc)
               .Sort(sortDoc);

                return aggregate.ToListAsync().Result.Select(m => BsonSerializer.Deserialize<NearbyArea>(m));

            }
            catch (Exception)
            {

                throw;
            }
             */
            return new List<NearbyArea>();
        }

        public Plan GetNewHomePropertyDetailsByMLSNumber(string mlsNumber)
        {
            /*
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
                var aggregate = GetCollection().Find<NewHomeListing>(m => m.BuilderNumber == mlsNumber).FirstOrDefaultAsync().Result;
                return aggregate;

            }
            catch (Exception ex)
            {
                throw ex;
            }
             */
            return new Plan();
        }


        #region Utility
        private LatLong GetLatLong(string allowedkeywords)
        {
            var matchQuery = "{'Subdivision.SubAddress.AddressArray': { $in : " + allowedkeywords + "}}";

            var projectQuery =
                "{_id : 0,'Latitude':'$Location.Latitude','Longitude':'$Location.Longitude'}";

            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
            var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

            var aggregate = GetCollection().Aggregate(new AggregateOptions()
            {
                AllowDiskUse = true
            })
                .Match(matchDoc)
                .Project(projectDoc)
                .ToListAsync()
                .Result
                .Select(m => BsonSerializer.Deserialize<LatLong>(m));

            return new LatLong()
            {
                Longitude = aggregate.Average(m => m.Longitude),
                Latitude = aggregate.Average(m => m.Latitude)
            };
        }
        #endregion


        public IEnumerable<PropertyListing> GetNewHomeIsFeaturedRecordList(int skip, int limit, bool IsMls, AdvanceSearch advanceSearch)
        {
            /*
            try
            {
                bool isGeoQuery = false;
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

                var tempList = new HashSet<string>();
                if (advanceSearch.Location.Contains(","))
                {
                    foreach (var searcgString in advanceSearch.Location.Split(','))
                    {
                        foreach (var searchString in searcgString.Split(' '))
                        {
                            if (searchString != "")
                            {
                                tempList.Add(searchString.ToLower().Trim());
                            }
                        }
                    }
                }
                else
                {
                    foreach (var searchString in advanceSearch.Location.Split(' '))
                    {
                        tempList.Add(searchString.ToLower().Trim());
                    }
                }

                var allowedkeywords = '[' + string.Join(",", tempList) + ']';
                allowedkeywords = allowedkeywords.Replace("[", "['");
                allowedkeywords = allowedkeywords.Replace("]", "']");
                allowedkeywords = allowedkeywords.Replace(",", "','");
                if (advanceSearch.NearByDistance > 0)
                {
                    isGeoQuery = true;

                }

                var matchQuery = IsMls
                    ? "{'MlsNumber': {'$regex': '" + advanceSearch.Location + "', '$options': 'i' }}"
                    : !isGeoQuery ? "{'Subdivision.SubAddress.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryStringForNewListing(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                          advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size, advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + ",'ExtProperties.IsFeatured':true}" :
                          "{" + UtilityClass.CreateQueryStringForNewListing(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                          advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size, advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + ",'ExtProperties.IsFeatured':true}";

                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);


                var projectQuery =
                    "{_id : 0,'objId':'$_id','FullStreetAddress':'$Subdivision.SubAddress.SubStreet1'," +
                    "'Price':'$Subdivision.PriceLow','Plans':'$Subdivision.Plan'," +
                    "'BrokerageName':'$BrandName','BrokerageLogoUrl':'$BrandLogoMed.Text','ListingParticipantsName':'$ReportingName','DefaultPhoto':'$Subdivision.SubImage.Text'," +
                    "'GeoLocation':'$Subdivision.SubAddress.GeoPoint','State':'$Subdivision.SubAddress.SubState'," +
                    "'PostalCode':'$Subdivision.SubAddress.SubZip','City':'$Subdivision.SubAddress.SubCity'}";


                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);
                IEnumerable<PropertyListing> aggregate;
                var sortBy = advanceSearch.SortBy == "0" ? "{Price : 1}" : "{" + Utility.UtilityClass.GetsortColumn(advanceSearch.SortBy.Split('_')[0]) + ":" + (advanceSearch.SortBy.Split('_')[1] == "Asc" ? 1 : -1) + "}";
                var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sortBy);
                if (isGeoQuery)
                {
                    var geolocation = GetLatLong(allowedkeywords);

                    var geoPoint = new BsonDocument
                    {
                        {"type", "Point"},
                        {"coordinates", new BsonArray(new double[] {geolocation.Longitude, geolocation.Latitude})}
                    };
                    var maxDistanceInMiles = advanceSearch.NearByDistance * 1609.34;

                    var geoNearOptions = new BsonDocument
                    {
                        {"near", geoPoint},
                        {"distanceField", "CalculatedDistance"},
                        {"maxDistance", Convert.ToDouble(maxDistanceInMiles)},
                        {"distanceMultiplier", Convert.ToDouble("0.000621371")},
                        {"query", matchDoc},
                        {"limit", 3000},
                        {"spherical", true},
                    };

                    var stage =
                        new BsonDocumentPipelineStageDefinition<NewHomeListing, NearbyArea>(
                            new BsonDocument { { "$geoNear", geoNearOptions } });
                    aggregate = GetCollection().Aggregate(new AggregateOptions()
                    {
                        AllowDiskUse = true
                    })
                        .AppendStage(stage)
                        .Project(projectDoc)
                        .Sort(sortDoc)
                        .Skip(skip)
                        .Limit(limit)
                        .ToListAsync().Result.Select(m => BsonSerializer.Deserialize<PropertyListing>(m));
                }
                else
                {
                    aggregate = GetCollection().Aggregate(new AggregateOptions()
                    {
                        AllowDiskUse = true
                    })
                        .Match(matchDoc)
                        .Project(projectDoc)
                        .Sort(sortDoc)
                        .Skip(skip)
                        .Limit(limit)
                        .ToListAsync()
                        .Result.Select(m => BsonSerializer.Deserialize<PropertyListing>(m));
                }
                return aggregate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
             */
            return new List<PropertyListing>();
        }

        public int GetNewHomeIsFeaturedRecordCount(AdvanceSearch advanceSearch, bool IsMls)
        {
            /*
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
                bool isGeoQuery = false;
                var tempList = new HashSet<string>();
                if (advanceSearch.Location.Contains(","))
                {
                    foreach (var searcgString in advanceSearch.Location.Split(','))
                    {
                        foreach (var searchString in searcgString.Split(' '))
                        {
                            if (searchString != "")
                            {
                                tempList.Add(searchString.ToLower().Trim());
                            }
                        }
                    }
                }
                else
                {
                    foreach (var searchString in advanceSearch.Location.Split(' '))
                    {
                        tempList.Add(searchString.ToLower().Trim());
                    }
                }

                var allowedkeywords = '[' + string.Join(",", tempList) + ']';
                allowedkeywords = allowedkeywords.Replace("[", "['");
                allowedkeywords = allowedkeywords.Replace("]", "']");
                allowedkeywords = allowedkeywords.Replace(",", "','");
                if (advanceSearch.NearByDistance > 0)
                {
                    isGeoQuery = true;

                }

                var matchQuery = IsMls
                    ? "{'MlsNumber': {'$regex': '" + advanceSearch.Location + "', '$options': 'i' }}"
                    : !isGeoQuery ? "{'Subdivision.SubAddress.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryStringForNewListing(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                          advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size, advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + ",'ExtProperties.IsFeatured':true}" :
                          "{" + UtilityClass.CreateQueryStringForNewListing(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                          advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size, advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + ",'ExtProperties.IsFeatured':true}";

                //var matchQuery = IsMls
                //    ? "{'MlsNumber': {'$regex': '" + advanceSearch.Location + "', '$options': 'i' }}"
                //    : !isGeoQuery ? "{'Subdivision.SalesOffice.Address.AddressArray': { $all : " + allowedkeywords + "}" +
                //      null + "}" :
                //          "{" + null + "}";
                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);


                int aggregate = 0;
                if (isGeoQuery)
                {
                    var geolocation = GetLatLong(allowedkeywords);

                    var geoPoint = new BsonDocument
                    {
                        {"type", "Point"},
                        {"coordinates", new BsonArray(new double[] {geolocation.Longitude, geolocation.Latitude})}
                    };
                    var maxDistanceInMiles = advanceSearch.NearByDistance * 1609.34;

                    var geoNearOptions = new BsonDocument
                    {
                        {"near", geoPoint},
                        {"distanceField", "CalculatedDistance"},
                        {"maxDistance", Convert.ToDouble(maxDistanceInMiles)},
                        {"distanceMultiplier", Convert.ToDouble("0.000621371")},
                        {"query", matchDoc},
                        {"limit", 3000},
                        {"spherical", true},
                    };

                    var stage =
                        new BsonDocumentPipelineStageDefinition<NewHomeListing, BsonDocument>(
                            new BsonDocument { { "$geoNear", geoNearOptions } });
                    aggregate = GetCollection().Aggregate(new AggregateOptions()
                    {
                        AllowDiskUse = true
                    })
                        .AppendStage(stage)
                        .ToListAsync()
                        .Result.Count();
                }
                else
                {
                    aggregate = GetCollection().Aggregate(new AggregateOptions()
                    {
                        AllowDiskUse = true
                    })
                       .Match(matchDoc).ToListAsync().Result.Count();
                }
                return aggregate;
            }
            catch (Exception ex)
            {
                throw ex;
            }*/
            return 0;
        }


        public IEnumerable<PropertyListing> GetNewHomeOpenHouseRecordList(int skip, int limit, bool IsMls, AdvanceSearch advanceSearch)
        {
            /*
            try
            {
                bool isGeoQuery = false;
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

                var tempList = new HashSet<string>();
                if (advanceSearch.Location.Contains(","))
                {
                    foreach (var searcgString in advanceSearch.Location.Split(','))
                    {
                        foreach (var searchString in searcgString.Split(' '))
                        {
                            if (searchString != "")
                            {
                                tempList.Add(searchString.ToLower().Trim());
                            }
                        }
                    }
                }
                else
                {
                    foreach (var searchString in advanceSearch.Location.Split(' '))
                    {
                        tempList.Add(searchString.ToLower().Trim());
                    }
                }

                var allowedkeywords = '[' + string.Join(",", tempList) + ']';
                allowedkeywords = allowedkeywords.Replace("[", "['");
                allowedkeywords = allowedkeywords.Replace("]", "']");
                allowedkeywords = allowedkeywords.Replace(",", "','");
                if (advanceSearch.NearByDistance > 0)
                {
                    isGeoQuery = true;

                }
                string startDate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
                var matchQuery = IsMls
                    ? "{'MlsNumber': {'$regex': '" + advanceSearch.Location + "', '$options': 'i' }}"
                    : !isGeoQuery ? "{'Subdivision.SubAddress.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryStringForNewListing(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                          advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size, advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + ",'ExtProperties.IsFeatured':true,'ExtProperties.OpenHouseStartDateTime':{$lte:new ISODate('" + startDate + "')},'ExtProperties.OpenHouseEndDateTime':{$gte:new ISODate('" + startDate + "')}}" :
                          "{" + UtilityClass.CreateQueryStringForNewListing(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                          advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size, advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + ",'ExtProperties.IsFeatured':true,'ExtProperties.OpenHouseStartDateTime':{$lte:new ISODate('" + startDate + "')},'ExtProperties.OpenHouseEndDateTime':{$gte:new ISODate('" + startDate + "')}}";

                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);


                var projectQuery =
                    "{_id : 0,'objId':'$_id','FullStreetAddress':'$Subdivision.SubAddress.SubStreet1'," +
                    "'Price':'$Subdivision.PriceLow','Plans':'$Subdivision.Plan'," +
                    "'BrokerageName':'$BrandName','BrokerageLogoUrl':'$BrandLogoMed.Text','ListingParticipantsName':'$ReportingName','DefaultPhoto':'$Subdivision.SubImage.Text'," +
                    "'GeoLocation':'$Subdivision.SubAddress.GeoPoint','State':'$Subdivision.SubAddress.SubState'," +
                    "'PostalCode':'$Subdivision.SubAddress.SubZip','City':'$Subdivision.SubAddress.SubCity'}";


                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);
                IEnumerable<PropertyListing> aggregate;
                var sortBy = advanceSearch.SortBy == "0" ? "{Price : 1}" : "{" + Utility.UtilityClass.GetsortColumn(advanceSearch.SortBy.Split('_')[0]) + ":" + (advanceSearch.SortBy.Split('_')[1] == "Asc" ? 1 : -1) + "}";
                var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sortBy);
                if (isGeoQuery)
                {
                    var geolocation = GetLatLong(allowedkeywords);

                    var geoPoint = new BsonDocument
                    {
                        {"type", "Point"},
                        {"coordinates", new BsonArray(new double[] {geolocation.Longitude, geolocation.Latitude})}
                    };
                    var maxDistanceInMiles = advanceSearch.NearByDistance * 1609.34;

                    var geoNearOptions = new BsonDocument
                    {
                        {"near", geoPoint},
                        {"distanceField", "CalculatedDistance"},
                        {"maxDistance", Convert.ToDouble(maxDistanceInMiles)},
                        {"distanceMultiplier", Convert.ToDouble("0.000621371")},
                        {"query", matchDoc},
                        {"limit", 3000},
                        {"spherical", true},
                    };

                    var stage =
                        new BsonDocumentPipelineStageDefinition<NewHomeListing, NearbyArea>(
                            new BsonDocument { { "$geoNear", geoNearOptions } });
                    aggregate = GetCollection().Aggregate(new AggregateOptions()
                    {
                        AllowDiskUse = true
                    })
                        .AppendStage(stage)
                        .Project(projectDoc)
                        .Sort(sortDoc)
                        .Skip(skip)
                        .Limit(limit)
                        .ToListAsync().Result.Select(m => BsonSerializer.Deserialize<PropertyListing>(m));
                }
                else
                {
                    aggregate = GetCollection().Aggregate(new AggregateOptions()
                    {
                        AllowDiskUse = true
                    })
                        .Match(matchDoc)
                        .Project(projectDoc)
                        .Sort(sortDoc)
                        .Skip(skip)
                        .Limit(limit)
                        .ToListAsync()
                        .Result.Select(m => BsonSerializer.Deserialize<PropertyListing>(m));
                }
                return aggregate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
             */
            return new List<PropertyListing>();
        }

        public int GetNewHomeOpenHouseRecordCount(AdvanceSearch advanceSearch, bool IsMls)
        {
            /*
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
                bool isGeoQuery = false;
                var tempList = new HashSet<string>();
                if (advanceSearch.Location.Contains(","))
                {
                    foreach (var searcgString in advanceSearch.Location.Split(','))
                    {
                        foreach (var searchString in searcgString.Split(' '))
                        {
                            if (searchString != "")
                            {
                                tempList.Add(searchString.ToLower().Trim());
                            }
                        }
                    }
                }
                else
                {
                    foreach (var searchString in advanceSearch.Location.Split(' '))
                    {
                        tempList.Add(searchString.ToLower().Trim());
                    }
                }

                var allowedkeywords = '[' + string.Join(",", tempList) + ']';
                allowedkeywords = allowedkeywords.Replace("[", "['");
                allowedkeywords = allowedkeywords.Replace("]", "']");
                allowedkeywords = allowedkeywords.Replace(",", "','");
                if (advanceSearch.NearByDistance > 0)
                {
                    isGeoQuery = true;

                }
                string startDate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
                var matchQuery = IsMls
                    ? "{'MlsNumber': {'$regex': '" + advanceSearch.Location + "', '$options': 'i' }}"
                    : !isGeoQuery ? "{'Subdivision.SubAddress.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryStringForNewListing(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                          advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size, advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + ",'ExtProperties.IsFeatured':true,'ExtProperties.OpenHouseStartDateTime':{$lte:new ISODate('" + startDate + "')},'ExtProperties.OpenHouseEndDateTime':{$gte:new ISODate('" + startDate + "')}}" :
                          "{" + UtilityClass.CreateQueryStringForNewListing(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                          advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size, advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + ",'ExtProperties.IsFeatured':true,'ExtProperties.OpenHouseStartDateTime':{$lte:new ISODate('" + startDate + "')},'ExtProperties.OpenHouseEndDateTime':{$gte:new ISODate('" + startDate + "')}}";

                //var matchQuery = IsMls
                //    ? "{'MlsNumber': {'$regex': '" + advanceSearch.Location + "', '$options': 'i' }}"
                //    : !isGeoQuery ? "{'Subdivision.SalesOffice.Address.AddressArray': { $all : " + allowedkeywords + "}" +
                //      null + "}" :
                //          "{" + null + "}";
                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);


                int aggregate = 0;
                if (isGeoQuery)
                {
                    var geolocation = GetLatLong(allowedkeywords);

                    var geoPoint = new BsonDocument
                    {
                        {"type", "Point"},
                        {"coordinates", new BsonArray(new double[] {geolocation.Longitude, geolocation.Latitude})}
                    };
                    var maxDistanceInMiles = advanceSearch.NearByDistance * 1609.34;

                    var geoNearOptions = new BsonDocument
                    {
                        {"near", geoPoint},
                        {"distanceField", "CalculatedDistance"},
                        {"maxDistance", Convert.ToDouble(maxDistanceInMiles)},
                        {"distanceMultiplier", Convert.ToDouble("0.000621371")},
                        {"query", matchDoc},
                        {"limit", 3000},
                        {"spherical", true},
                    };

                    var stage =
                        new BsonDocumentPipelineStageDefinition<NewHomeListing, BsonDocument>(
                            new BsonDocument { { "$geoNear", geoNearOptions } });
                    aggregate = GetCollection().Aggregate(new AggregateOptions()
                    {
                        AllowDiskUse = true
                    })
                        .AppendStage(stage)
                        .ToListAsync()
                        .Result.Count();
                }
                else
                {
                    aggregate = GetCollection().Aggregate(new AggregateOptions()
                    {
                        AllowDiskUse = true
                    })
                       .Match(matchDoc).ToListAsync().Result.Count();
                }
                return aggregate;
            }
            catch (Exception ex)
            {
                throw ex;
            }*/
            return 0;
        }

        public ManagePropertyViewModel GetExtraProperty(string builderno)
        {
            /*
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

            var aggregate = GetCollection().Find<NewHomeListing>(m => m.BuilderNumber == builderno).FirstOrDefaultAsync().Result;
            return aggregate.ExtProperties;
             */
            return new ManagePropertyViewModel();
        }

        public bool SetExtraProperty(ManagePropertyViewModel viewModel)
        {
            /*
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

                var filter = Builders<NewHomeListing>.Filter.Eq("BuilderNumber", viewModel.UniqueId);
                var update = Builders<NewHomeListing>.Update
                    .Set("ExtProperties", viewModel);

                var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;

                return results.ModifiedCount > 0;
            }
            catch (Exception)
            {

                throw;
            }*/
            return false;
        }
        public bool UpdateExtraProperty(ManageNewHomePropertyViewModel viewModel)
        {
            /*
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

                var filter = Builders<NewHomeListing>.Filter.Eq("BuilderNumber", viewModel.UniqueId);
                var update = Builders<NewHomeListing>.Update
                    .Set("ExtProperties", viewModel);

                var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;

                return results.ModifiedCount > 0;
            }
            catch (Exception)
            {

                throw;
            }*/
            return false;
        }
        /*
        public bool UpdateImageList(NewHomeListing propertyViewModel)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

                var filter = Builders<NewHomeListing>.Filter.Eq("BuilderNumber", propertyViewModel.BuilderNumber);
                var update = Builders<NewHomeListing>.Update
                    .PushEach("Subdivision.SubImage", propertyViewModel.Subdivision.SubImage);

                var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;

                return results.ModifiedCount > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        public int ImageListCount(string BuilderNumber)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
                var filter = Builders<NewHomeListing>.Filter.Eq("BuilderNumber", BuilderNumber);
                var results = GetCollection().Find(filter).FirstOrDefaultAsync().Result.Subdivision.SubImage;

                return results != null ? results.Count() : 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        */
        public bool InsertNewHome(Plan entities)
        {

            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
                GetCollection().InsertOneAsync(entities).Wait();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }



        /*
        public bool UpdateNewHomePlanList(NewHomeListing propertyViewModel)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

                var filter = Builders<NewHomeListing>.Filter.Eq("BuilderNumber", propertyViewModel.BuilderNumber);
                var update = Builders<NewHomeListing>.Update
                    .PushEach("Subdivision.Plan", propertyViewModel.Subdivision.Plan);

                var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;

                return results.ModifiedCount > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public Plan GetPlan(string builderNumber)
        {
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
            var collection = GetCollection();
            var tmpList = new Plan();
            var jsonQuery = "{'Subdivision.Plan.PlanNumber':'" + builderNumber + "'}";
            var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(jsonQuery);

            using (var cursor = collection.FindAsync(doc).Result)
            {
                while (cursor.MoveNextAsync().Result)
                {
                    tmpList = cursor.Current.FirstOrDefault().Subdivision.Plan.Where(m => m.PlanNumber == builderNumber).FirstOrDefault();
                }

                return tmpList;
            }
        }

        */
        public bool UpdateNewHomePlan(Plan plan)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

                var filter = Builders<Plan>.Filter.Eq("PlanId", plan.PlanId);
                var update = Builders<Plan>.Update
                    .Set("BuilderEmail", plan.BuilderEmail)
                    .Set("BuilderName", plan.BuilderName)
                    .Set("BuilderNumber", plan.BuilderNumber)
                    .Set("Builder_dre_number", plan.Builder_dre_number)
                    .Set("Communityaddress", plan.Communityaddress)
                    .Set("CommunityName", plan.CommunityName)
                    .Set("CommunityNumber", plan.CommunityNumber)
                    .Set("Communitycity", plan.Communitycity)
                    .Set("Communitystate", plan.Communitystate)
                    .Set("Communityzip", plan.Communityzip)
                    .Set("Communityprice_high", plan.Communityprice_high)
                    .Set("Communityprice_low", plan.Communityprice_low)
                    .Set("Communitysqft_high", plan.Communitysqft_high)
                    .Set("Communitysqft_low", plan.Communitysqft_low)
                    .Set("Name", plan.Name)
                    .Set("Number", plan.Number)
                    .Set("Land_price", plan.Land_price)
                    .Set("Base_price", plan.Base_price)
                    .Set("Dining_areas", plan.Dining_areas)
                    .Set("Baths", plan.Baths)
                    .Set("Half_baths", plan.Half_baths)
                    .Set("Master_bedroom_location", plan.Master_bedroom_location)
                    .Set("Bedrooms", plan.Bedrooms)
                    .Set("Garage_entry", plan.Garage_entry)
                    .Set("Garage", plan.Garage)
                    .Set("Garage_detach", plan.Garage_detach)
                    .Set("Style", plan.Style)
                    .Set("Basement", plan.Basement)
                    .Set("Marketing_headline", plan.Marketing_headline)
                    .Set("Description", plan.Description)
                    .Set("Stories", plan.Stories)
                    .Set("Brochure", plan.Brochure)
                    .Set("Plan_amenities", plan.Plan_amenities)
                    .Set("Virtual_tour", plan.Virtual_tour)
                    .Set("Plan_viewer", plan.Plan_viewer)
                    .Set("Baths_low", plan.Baths_low)
                    .Set("Half_baths_low", plan.Half_baths_low)
                    .Set("Bedrooms_low", plan.Bedrooms_low)
                    .Set("Garage_low", plan.Garage_low)
                    ;

                var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;

                return results.ModifiedCount > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }




        public string GetListedType(string mlsid)
        {
            /*
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

            var aggregate = GetCollection().Find<NewHomeListing>(m => m.BuilderNumber == mlsid).FirstOrDefaultAsync().Result;
            return aggregate.ListedBy;
             */
            return string.Empty;
        }


        public bool DeleteProperty(string mlsNumber, bool IsDeleted)
        {
            /*
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

                var filter = Builders<NewHomeListing>.Filter.Eq("BuilderNumber", mlsNumber);
                var update = Builders<NewHomeListing>.Update
                    .Set("ExtProperties.IsDeleted", IsDeleted);

                var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;

                return results.ModifiedCount > 0;
            }
            catch (Exception)
            {

                throw;
            }*/
            return false;
        }


        public string GetBuilderNo(string planNo)
        {
            /*
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
            var collection = GetCollection();
            var tmpList = new NewHomeListing();
            var jsonQuery = "{'Subdivision.Plan.PlanNumber':'" + planNo + "'}";
            var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(jsonQuery);

            using (var cursor = collection.FindAsync(doc).Result)
            {
                while (cursor.MoveNextAsync().Result)
                {
                    tmpList = cursor.Current.FirstOrDefault();
                }

                return tmpList.BuilderNumber;
            }
             * */
            return string.Empty;
        }

        public void CreateIndex()
        {
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

            GetCollection().Indexes.CreateOneAsync(Builders<Plan>.IndexKeys.Combine(
                //Builders<Plan>.IndexKeys.Ascending(_ => _.Leads_email),
                Builders<Plan>.IndexKeys.Ascending(_ => _.FullAddress),
                Builders<Plan>.IndexKeys.Geo2DSphere(_ => _.GeoPoint)));
        }







        public bool UpdatePlanImageList(string[] imageList, string planid)
        {
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

            var filter = Builders<Plan>.Filter.Eq("PlanId", planid);
            var _imageList = new List<Image>();
            foreach (var item in imageList)
            {
                _imageList.Add(new Image { Reference = item });
            }

            var update = Builders<Plan>.Update
                   .PushEach("Images.Image", _imageList);
            var _result = GetCollection().UpdateOneAsync(filter, update).Result;
            return _result.ModifiedCount > 0;
        }

        public bool UpdateHomeImageList(string[] imageList, string planid, string homeid)
        {
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

            var plan = GetCollection().Find(m => m.PlanId == planid).FirstOrDefaultAsync().Result;
            var index = plan.Homes.Home.FindIndex(x => x.Id == homeid);
            var filter = Builders<Plan>.Filter.Eq("Homes.Home._id", homeid);

            var _imageList = new List<Image>();
            foreach (var item in imageList)
            {
                _imageList.Add(new Image { Reference = item });
            }

            var update = Builders<Plan>.Update
                   .PushEach("Homes.Home." + index + ".Images.Image", _imageList);
          
            //    .Set("Homes.Home." + index + ".Home_type", home.Home_type)
            var result = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;
            return result.ModifiedCount > 0;

            //  var index = plan.Homes.Home.FindIndex(x => x.Id == home.Id);

            //base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
            //var filter = Builders<Plan>.Filter.Eq("Homes.Home._id", home.Id);
            //var update = Builders<Plan>.Update
            //    .Set("Homes.Home." + index + ".Home_type", home.Home_type)
        }

        public int ImageListCount(string BuilderNumber)
        {
            throw new NotImplementedException();
        }



        public bool UpdateNewHomePlanList(Plan propertyViewModel)
        {
            throw new NotImplementedException();
        }

        public Plan GetPlan(string builderNumber)
        {
            return new Plan();
        }


        public bool InsertFromFeed(List<Plan> plans)
        {
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
            foreach (var item in plans)
            {
                var filter = Builders<Plan>.Filter.Eq("PlanId", item.PlanId);
                var result = GetCollection().Find<Plan>(filter).FirstOrDefaultAsync().Result;
                if (result == null)
                {
                    this.InsertNewHome(item);
                }
                else
                {
                    /* var updatefilter = Builders<Plan>.Filter.Eq("BuilderNumber", mlsNumber);
                     var update = Builders<Plan>.Update
                         .Set("Base_price", item.Base_price)
                         .Set();*/
                }
            }
            return true;
        }





        public bool DeleteNewHome(string planId)
        {
            base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

            var updateOffice = GetCollection().UpdateOneAsync(
                m => m.PlanId == planId,
                Builders<Repositories.Models.NewHome.Plan>.Update
                    .Set(m => m.IsDeletedByPortal, true)
                    ).Result;

            return true;
        }


        public bool PullImage(string imageurl, string mlsNumber)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
                //var matchDoc = BsonSerializer.Deserialize<BsonDocument>(" { '$pull': { 'Photos.Photo' : { 'MediaURL' : 'http://photos.listhub.net/SANDICOR/150061347/2?lm=20151209T232004' } } }");               
                var filter = Builders<Plan>.Filter.Eq(m => m.PlanId, mlsNumber);
                var update = Builders<Plan>.Update.PullFilter(p => p.Images.Image, f => f.Reference == imageurl);
                var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;
                return results.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool PullImageFromHome(string imageurl, string homeid, string planid)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

                var plan = GetCollection().Find(m => m.PlanId == planid).FirstOrDefaultAsync().Result;
                var index = plan.Homes.Home.FindIndex(x => x.Id == homeid);
                
                var filter = Builders<Plan>.Filter.Eq("Homes.Home._id", homeid);
                var update = BsonSerializer.Deserialize<BsonDocument>(" { '$pull': { 'Homes.Home." + index + ".Images.Image' : { 'Reference' : '" + imageurl + "' } } }");               
                
                var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;
                return results.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public IEnumerable<SubPropertyTypeCheckBox> GetNewHomeSubPropertyType()
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
                var tmpList = new List<string>();
                using (var cursor = GetCollection().DistinctAsync<string>("Homes.Home.Home_type", new BsonDocument()).Result)
                {
                    while (cursor.MoveNextAsync().Result)
                    {
                        tmpList.AddRange(cursor.Current.ToList());
                    }
                    List<SubPropertyTypeCheckBox> lstPropertyTypeCheckBox = new List<SubPropertyTypeCheckBox>();
                    foreach (var item in tmpList)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            SubPropertyTypeCheckBox propertyTypeCheckBox = new SubPropertyTypeCheckBox();
                            propertyTypeCheckBox.SubPropertyName = item.ToUpper();
                            propertyTypeCheckBox.IsSelected = false;
                            lstPropertyTypeCheckBox.Add(propertyTypeCheckBox);
                        }
                    }
                    return lstPropertyTypeCheckBox.GroupBy(x => x.SubPropertyName).Select(y => y.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool PullHomeFromPlan(string homeid, string planid)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);

                var plan = GetCollection().Find(m => m.PlanId == planid).FirstOrDefaultAsync().Result;
                var index = plan.Homes.Home.FindIndex(x => x.Id == homeid);

                var filter = Builders<Plan>.Filter.Eq("Homes.Home._id", homeid);
                var update = BsonSerializer.Deserialize<BsonDocument>(" { '$pull': { 'Homes.Home' : { '_id' : '" + homeid + "' } } }");

                var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;
                return results.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
