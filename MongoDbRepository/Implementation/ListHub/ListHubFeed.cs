using System;
using System.Collections.Generic;
using System.Linq;
using Core.Repository;
using KellermanSoftware.CompareNetObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Repositories.Interfaces.ListHub;
using Repositories.Models.Common;
using Repositories.Models.ListHub;
using Repositories.Models.ViewModel;
using Utility;
using Repositories.Models.Admin.User;

namespace Core.Implementation.ListHub
{


    public class ListHubFeed : Repository<ListHubListing>, IListHub
    {
        private new const string CollectionName = "";

        public ListHubFeed()
            : base(CollectionName)
        {
        }

        public ListHubFeed(IMongoDatabase database)
            : base(database, CollectionName)
        {

        }

        public ListHubFeed(string connectionString, string databaseName)
            : base(connectionString, databaseName, CollectionName)
        {
        }

        #region Purchase

        public bool InsertBulkPurchaseCategoryRealestate(List<ListHubListing> entities)
        {
            return InsertBulk(entities);
        }

        public IEnumerable<AutoCompleteDetails> GetPurchaseAddressList(string searchKey)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);

                var matchQuery = "{'Address.FullAddress': {'$regex': '" + searchKey + "', '$options': 'i' }}";
                var group = "{_id: null,'CityWithStateAndPostCode':{$addToSet: '$Address.FullAddress'}}";
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

        public IEnumerable<AutoCompleteDetails> GetPurchaseMlsNumberList(string searchKey)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
                List<string> lst = new List<string>();
                var matchQuery = "{'MlsNumber': {'$regex': '" + searchKey + "', '$options': 'i' }}";
                var group = "{_id: null,'CityWithStateAndPostCode':{$addToSet: {$concat : ['#','$MlsNumber']}}}";
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

        public PropertyDetails GetPurchaseRecordDetailsByMLSNumber(string mlsNumber)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
                var tmpList = new List<PropertyDetails>();

                var matchQuery = "{'MlsNumber':'" + mlsNumber + "'}";
                var projectQuery =
                    "{_id : 0,'FullStreetAddress' : '$Address.FullStreetAddress','City':'$Address.City','StateOrProvince':'$Address.StateOrProvince'," +
                "'PostalCode':'$Address.PostalCode','MlsNumber':1,'ListPrice':'$ListPrice.Text','ListingURL':1,'ProviderName':1,'ProviderURL':1,'ProviderCategory':1," +
                "'LeadRoutingEmail':1,'ListingStatus':1,'ListingDescription':1,'ListingDate':1,'ListingTitle':1,'Bedrooms':1,'Bathrooms':1,'FullBathrooms':1," +
                "'ThreeQuarterBathrooms':1,'HalfBathrooms':1,'OneQuarterBathrooms':1,'PartialBathrooms':1,'LotSize':'$LotSize.Text','PropertyType':'$PropertyType.Text'," +
                "'PropertySubType':'$PropertySubType.Text','ListingKey':1,'ListingCategory':1,'Location':'$Location.Directions','Photos':'$Photos.Photo.MediaURL'," +
                "'Schools':'$Location.Community.Schools.School','Offices':'$Offices.Office.OfficeKey','OfficeId':'$Offices.Office.OfficeId'," +
                "'OfficeCodeId':'$Offices.Office.OfficeCode.OfficeCodeId','OfficeName':'$Offices.Office.Name','CorporateName':'$Offices.Office.CorporateName'," +
                "'BrokerId':'$Offices.Office.BrokerId','MainOfficeId':'$Offices.Office.MainOfficeId','OfficePhoneNumber':'$Offices.Office.PhoneNumber'," +
                "'OfficeFullStreetAddress':'$Offices.Office.Address.FullStreetAddress','OfficeCity':'$Offices.Office.Address.City','VirtualTour':'$VirtualTours.VirtualTour.MediaURL'" +
                "'OfficeState':'$Offices.Office.Address.StateOrProvince','OfficePostalCode':'$Offices.Office.Address.PostalCode','BrokerageName':'$Brokerage.Name'," +
                "'BrokeragePhone':'$Brokerage.Phone','BrokerageEmail':'$Brokerage.Email','BrokerageWebsite':'$Brokerage.WebsiteURL','BrokerageLogoURL':'$Brokerage.LogoURL'," +
                "'BrokerageFullStreetAddress':'$Brokerage.Address.FullAddressStreet','BrokerageCity':'$Brokerage.Address.City'," +
                "'BrokerageState':'$Brokerage.Address.StateOrProvince','BrokeragePostalCode':'$Brokerage.Address.PostalCode'," +
                "'Appliance':'$DetailedCharacteristics.Appliances.Appliance','ArchitectureStyle':'$DetailedCharacteristics.ArchitectureStyle.Text'," +
                "'CoolingSystem':'$DetailedCharacteristics.CoolingSystems.CoolingSystem','ExteriorType':'$DetailedCharacteristics.ExteriorTypes.ExteriorType'," +
                "'HeatingSystem':'$DetailedCharacteristics.HeatingSystems.HeatingSystem','IsNewConstruction':'$DetailedCharacteristics.IsNewConstruction'," +
                "'NoOfFloor':'$DetailedCharacteristics.NumFloors','NoOfParkingSpace':'$DetailedCharacteristics.NumParkingSpaces'," +
                "'ParkingType':'$DetailedCharacteristics.ParkingTypes.ParkingType','RoofTypes':'$DetailedCharacteristics.RoofTypes.RoofType'," +
                "'Rooms':'$DetailedCharacteristics.Rooms.Room','LivingArea':'$LivingArea','IsFeatured':'$ExtProperties.IsFeatured','IsSpotLight':'$ExtProperties.IsSpotlight','GeoPoint':'$Location.GeoPoint'," + "'DateTimeRanges' : '$ExtProperties.DateTimeRanges'" +
                "'AgentEmail':'$ListingParticipants.Participant.Email','WebsiteUrl':'$ListingParticipants.Participant.WebsiteURL','AgentName':{ $concat: [ '$ListingParticipants.Participant.FirstName',' ', '$ListingParticipants.Participant.LastName' ] }}";

                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

                var aggregate = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
               .Match(matchDoc)
               .Project(projectDoc);

                var result = aggregate.FirstOrDefaultAsync().Result;
                var myObj = BsonSerializer.Deserialize<PropertyDetails>(result);
                return myObj;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<PropertyListing> GetPurchaseRecordList(int skip, int limit, bool IsMls,
            AdvanceSearch advanceSearch)
        {
            try
            {
                bool isGeoQuery = false;
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);

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
                    : !isGeoQuery
                        ? "{'Address.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                              advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                              advanceSearch.LotSize,
                              advanceSearch.HomeAge, advanceSearch.SelectedProperty) + "}"
                        : "{" + UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                            advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                            advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + "}";

                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);



                var projectQuery =
                    "{_id : 0,'MlsNumber' : '$MlsNumber','FullStreetAddress':'$Address.FullStreetAddress'," +
                    "'Price':'$ListPrice.Text','NoOfBedRooms':'$Bedrooms','NoOfBathRooms':'$Bathrooms'," +
                    "'NoOfHalfBathRooms':'$HalfBathrooms','PropertyType':{$concat : ['$PropertyType.Text','-','$PropertySubType.Text']}," +
                    "'BrokerageName':'$Brokerage.Name','BrokerageLogoUrl':'$Brokerage.LogoURL','ListingParticipantsName':{$concat : ['$ListingParticipants.Participant.FirstName',' ','$ListingParticipants.Participant.LastName']},'DefaultPhoto':'$Photos.Photo.MediaURL','GeoLocation':'$Location.GeoPoint','State':'$Address.StateOrProvince'," +
                    "'PostalCode':'$Address.PostalCode','City':'$Address.City','LivingArea':'$LivingArea','IsNewConstruction':'$DetailedCharacteristics.IsNewConstruction','IsFeatured':'$ExtProperties.IsFeatured','GeoPoint':'$Location.GeoPoint'}";


                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);
                IEnumerable<PropertyListing> aggregate;
                var sortBy = advanceSearch.SortBy == "0"
                    ? "{Price : 1}"
                    : "{" + Utility.UtilityClass.GetsortColumn(advanceSearch.SortBy.Split('_')[0]) + ":" +
                      (advanceSearch.SortBy.Split('_')[1] == "Asc" ? 1 : -1) + "}";
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
                        new BsonDocumentPipelineStageDefinition<ListHubListing, NearbyArea>(
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
        }

        public IEnumerable<PropertyTypeCheckBox> GetPurchasePropertyType()
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
                var tmpList = new List<string>();
                using (
                    var cursor = GetCollection().DistinctAsync<string>("PropertyType.Text", new BsonDocument()).Result)
                {
                    while (cursor.MoveNextAsync().Result)
                    {
                        tmpList.AddRange(cursor.Current.ToList());
                    }
                    List<PropertyTypeCheckBox> lstPropertyTypeCheckBox = new List<PropertyTypeCheckBox>();
                    foreach (var item in tmpList)
                    {
                        PropertyTypeCheckBox propertyTypeCheckBox = new PropertyTypeCheckBox();
                        propertyTypeCheckBox.PropertyName = item;
                        propertyTypeCheckBox.IsSelected = false;
                        lstPropertyTypeCheckBox.Add(propertyTypeCheckBox);
                    }
                    return lstPropertyTypeCheckBox;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<SubPropertyTypeCheckBox> GetPurchaseSubPropertyType()
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
                var tmpList = new List<string>();
                using (
                    var cursor = GetCollection().DistinctAsync<string>("PropertySubType.Text", new BsonDocument()).Result)
                {
                    while (cursor.MoveNextAsync().Result)
                    {
                        tmpList.AddRange(cursor.Current.ToList());
                    }
                    List<SubPropertyTypeCheckBox> lstPropertyTypeCheckBox = new List<SubPropertyTypeCheckBox>();
                    foreach (var item in tmpList)
                    {
                        SubPropertyTypeCheckBox propertyTypeCheckBox = new SubPropertyTypeCheckBox();
                        propertyTypeCheckBox.SubPropertyName = item;
                        propertyTypeCheckBox.IsSelected = false;
                        lstPropertyTypeCheckBox.Add(propertyTypeCheckBox);
                    }
                    return lstPropertyTypeCheckBox;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int GetPurchaseRecordCount(AdvanceSearch advanceSearch, bool IsMls)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
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
                    : !isGeoQuery
                        ? "{'Address.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                              advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                              advanceSearch.LotSize,
                              advanceSearch.HomeAge, advanceSearch.SelectedProperty) + "}"
                        : "{" + UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                            advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                            advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + "}";

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
                        new BsonDocumentPipelineStageDefinition<ListHubListing, BsonDocument>(
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
            }
        }

        public IEnumerable<NearbyArea> GetNearByPurchaseAreaDetails(double latitude, double longitude,
            double maxDistanceInMiles)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);

                BsonDocument geoPoint = new BsonDocument
                {
                    {"type", "Point"},
                    {"coordinates", new BsonArray(new double[] {longitude, latitude})}
                };
                var matchQuery = "{ 'Address.City' : {$ne :''},'Address.StateOrProvince' : {$ne :''}}";
                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);

                //Here we are forming the Geo Near Options that consists of Maximum Distance that we have to search till, number of records to be fetched, earth Spherical property to True, calculate the distance from starting point.
                BsonDocument geoNearOptions = new BsonDocument
                {
                    {"near", geoPoint},
                    {"distanceField", "CalculatedDistance"},
                    {"maxDistance", Convert.ToDouble(maxDistanceInMiles)},
                    {"distanceMultiplier", Convert.ToDouble("0.000621371")},
                    {"query", matchDoc},
                    {"limit", 3000},
                    {"spherical", true},
                };

                //MongoDB requires $geoNear as the first stage of a pipeline.
                var stage =
                    new BsonDocumentPipelineStageDefinition<ListHubListing, NearbyArea>(new BsonDocument
                    {
                        {"$geoNear", geoNearOptions}
                    });

                var projectQuery =
                    "{ _id : 0,CalculatedDistance : '$CalculatedDistance',CityWithState  :  { $concat : [ '$Address.City', ', ', '$Address.StateOrProvince']}}";
                var groupQuery =
                    "{_id :'$CityWithState',DistanceArray :  { $addToSet: '$CalculatedDistance' },AvgDistance : {$avg: '$CalculatedDistance'}}}";
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
                //.ToListAsync().Result.Select(m => BsonSerializer.Deserialize<NearbyArea>(m));

                return aggregate.ToListAsync().Result.Select(m => BsonSerializer.Deserialize<NearbyArea>(m));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public ListHubListing GetPurchasePropertyDetailsByMLSNumber(string mlsNumber)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
                var tmpList = new List<PropertyDetails>();

                var aggregate =
                    GetCollection().Find<ListHubListing>(m => m.MlsNumber == mlsNumber).FirstOrDefaultAsync().Result;
                return aggregate;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<PropertyListing> GetPurchaseIsFeaturedRecordList(int skip, int limit, bool IsMls,
            AdvanceSearch advanceSearch)
        {
            try
            {
                bool isGeoQuery = false;
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);

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
                    : !isGeoQuery
                        ? "{'Address.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                              advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                              advanceSearch.LotSize,
                              advanceSearch.HomeAge, advanceSearch.SelectedProperty) +
                          ",'ExtProperties.IsFeatured':true}"
                        : "{" + UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                            advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                            advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + ",'ExtProperties.IsFeatured':true}";

                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);



                var projectQuery =
                    "{_id : 0,'MlsNumber' : '$MlsNumber','FullStreetAddress':'$Address.FullStreetAddress'," +
                    "'Price':'$ListPrice.Text','NoOfBedRooms':'$Bedrooms','NoOfBathRooms':'$Bathrooms'," +
                    "'NoOfHalfBathRooms':'$HalfBathrooms','PropertyType':{$concat : ['$PropertyType.Text','-','$PropertySubType.Text']}," +
                    "'BrokerageName':'$Brokerage.Name','BrokerageLogoUrl':'$Brokerage.LogoURL','ListingParticipantsName':{$concat : ['$ListingParticipants.Participant.FirstName',' ','$ListingParticipants.Participant.LastName']},'DefaultPhoto':'$Photos.Photo.MediaURL','GeoLocation':'$Location.GeoPoint','State':'$Address.StateOrProvince'," +
                    "'PostalCode':'$Address.PostalCode','City':'$Address.City','LivingArea':'$LivingArea','IsNewConstruction':'$DetailedCharacteristics.IsNewConstruction','IsFeatured':'$ExtProperties.IsFeatured','GeoPoint':'$Location.GeoPoint'}";


                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);
                IEnumerable<PropertyListing> aggregate;
                var sortBy = advanceSearch.SortBy == "0"
                    ? "{Price : 1}"
                    : "{" + Utility.UtilityClass.GetsortColumn(advanceSearch.SortBy.Split('_')[0]) + ":" +
                      (advanceSearch.SortBy.Split('_')[1] == "Asc" ? 1 : -1) + "}";
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
                        new BsonDocumentPipelineStageDefinition<ListHubListing, NearbyArea>(
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
        }

        public int GetPurchaseIsFeaturedRecordCount(AdvanceSearch advanceSearch, bool IsMls)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
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
                    : !isGeoQuery
                        ? "{'Address.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                              advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                              advanceSearch.LotSize,
                              advanceSearch.HomeAge, advanceSearch.SelectedProperty) +
                          ",'ExtProperties.IsFeatured':true}"
                        : "{" + UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                            advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                            advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + ",'ExtProperties.IsFeatured':true}";

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
                        new BsonDocumentPipelineStageDefinition<ListHubListing, BsonDocument>(
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
            }
        }

        #endregion

        #region Rent

        public bool InsertBulkRentCategoryRealestate(IEnumerable<ListHubListing> entities)
        {
            base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
            return InsertBulk(entities);
        }

        public IEnumerable<AutoCompleteDetails> GetRentAddressList(string searchKey)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
                var matchQuery = "{'Address.FullAddress': {'$regex': '" + searchKey + "', '$options': 'i' }}";

                var group = "{_id: null,'CityWithStateAndPostCode':{$addToSet: '$Address.FullAddress'}}";
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

        public IEnumerable<AutoCompleteDetails> GetRentMlsNumberList(string searchKey)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
                List<string> lst = new List<string>();
                var matchQuery = "{'MlsNumber': {'$regex': '" + searchKey + "', '$options': 'i' }}";
                var group = "{_id: null,'CityWithStateAndPostCode':{$addToSet: {$concat : ['#','$MlsNumber']}}}";
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

        public PropertyDetails GetRentRecordDetailsByMLSNumber(string mlsNumber)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
                var tmpList = new List<PropertyDetails>();

                var matchQuery = "{'MlsNumber':'" + mlsNumber + "'}";
                var projectQuery =
                    "{_id : 0,'FullStreetAddress' : '$Address.FullStreetAddress','City':'$Address.City','StateOrProvince':'$Address.StateOrProvince'," +
                "'PostalCode':'$Address.PostalCode','MlsNumber':1,'ListPrice':'$ListPrice.Text','ListingURL':1,'ProviderName':1,'ProviderURL':1,'ProviderCategory':1," +
                "'LeadRoutingEmail':1,'ListingStatus':1,'ListingDescription':1,'ListingDate':1,'ListingTitle':1,'Bedrooms':1,'Bathrooms':1,'FullBathrooms':1," +
                "'ThreeQuarterBathrooms':1,'HalfBathrooms':1,'OneQuarterBathrooms':1,'PartialBathrooms':1,'LotSize':'$LotSize.Text','PropertyType':'$PropertyType.Text'," +
                "'PropertySubType':'$PropertySubType.Text','ListingKey':1,'ListingCategory':1,'Location':'$Location.Directions','Photos':'$Photos.Photo.MediaURL'," +
                "'Schools':'$Location.Community.Schools.School','Offices':'$Offices.Office.OfficeKey','OfficeId':'$Offices.Office.OfficeId'," +
                "'OfficeCodeId':'$Offices.Office.OfficeCode.OfficeCodeId','OfficeName':'$Offices.Office.Name','CorporateName':'$Offices.Office.CorporateName'," +
                "'BrokerId':'$Offices.Office.BrokerId','MainOfficeId':'$Offices.Office.MainOfficeId','OfficePhoneNumber':'$Offices.Office.PhoneNumber'," +
                "'OfficeFullStreetAddress':'$Offices.Office.Address.FullStreetAddress','OfficeCity':'$Offices.Office.Address.City'," +
                "'OfficeState':'$Offices.Office.Address.StateOrProvince','OfficePostalCode':'$Offices.Office.Address.PostalCode','BrokerageName':'$Brokerage.Name'," +
                "'BrokeragePhone':'$Brokerage.Phone','BrokerageEmail':'$Brokerage.Email','BrokerageWebsite':'$Brokerage.WebsiteURL','BrokerageLogoURL':'$Brokerage.LogoURL'," +
                "'BrokerageFullStreetAddress':'$Brokerage.Address.FullAddressStreet','BrokerageCity':'$Brokerage.Address.City'," +
                "'BrokerageState':'$Brokerage.Address.StateOrProvince','BrokeragePostalCode':'$Brokerage.Address.PostalCode'," +
                "'Appliance':'$DetailedCharacteristics.Appliances.Appliance','ArchitectureStyle':'$DetailedCharacteristics.ArchitectureStyle.Text'," +
                "'CoolingSystem':'$DetailedCharacteristics.CoolingSystems.CoolingSystem','ExteriorType':'$DetailedCharacteristics.ExteriorTypes.ExteriorType'," +
                "'HeatingSystem':'$DetailedCharacteristics.HeatingSystems.HeatingSystem','IsNewConstruction':'$DetailedCharacteristics.IsNewConstruction'," +
                "'NoOfFloor':'$DetailedCharacteristics.NumFloors','NoOfParkingSpace':'$DetailedCharacteristics.NumParkingSpaces'," +
                "'ParkingType':'$DetailedCharacteristics.ParkingTypes.ParkingType','RoofTypes':'$DetailedCharacteristics.RoofTypes.RoofType'," +
                "'Rooms':'$DetailedCharacteristics.Rooms.Room','LivingArea':'$LivingArea','IsFeatured':'$ExtProperties.IsFeatured','IsSpotLight':'$ExtProperties.IsSpotlight','ClassifiedExpireDate':'$ExpireDate','GeoPoint':'$Location.GeoPoint'}";

                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

                var aggregate = GetCollection().Aggregate(new AggregateOptions()
                {
                    AllowDiskUse = true
                })
               .Match(matchDoc)
               .Project(projectDoc);
                var myObj = BsonSerializer.Deserialize<PropertyDetails>(aggregate.FirstOrDefaultAsync().Result);
                return myObj;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<PropertyListing> GetRentRecordList(int skip, int limit, bool IsMls,
            AdvanceSearch advanceSearch)
        {
            try
            {
                bool isGeoQuery = false;
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);

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
                    : !isGeoQuery
                        ? "{'Address.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                              advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                              advanceSearch.LotSize,
                              advanceSearch.HomeAge, advanceSearch.SelectedProperty) + "}"
                        : "{" + UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                            advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                            advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + "}";

                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);



                var projectQuery =
                    "{_id : 0,'MlsNumber' : '$MlsNumber','FullStreetAddress':'$Address.FullStreetAddress'," +
                    "'Price':'$ListPrice.Text','NoOfBedRooms':'$Bedrooms','NoOfBathRooms':'$Bathrooms'," +
                    "'NoOfHalfBathRooms':'$HalfBathrooms','PropertyType':{$concat : ['$PropertyType.Text','-','$PropertySubType.Text']}," +
                    "'BrokerageName':'$Brokerage.Name','BrokerageLogoUrl':'$Brokerage.LogoURL','ListingParticipantsName':{$concat : ['$ListingParticipants.Participant.FirstName',' ','$ListingParticipants.Participant.LastName']},'DefaultPhoto':'$Photos.Photo.MediaURL','GeoLocation':'$Location.GeoPoint','State':'$Address.StateOrProvince'," +
                    "'PostalCode':'$Address.PostalCode','City':'$Address.City','LivingArea':'$LivingArea','IsNewConstruction':'$DetailedCharacteristics.IsNewConstruction'}";

                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);
                IEnumerable<PropertyListing> aggregate;
                var sortBy = advanceSearch.SortBy == "0"
                    ? "{Price : 1}"
                    : "{" + Utility.UtilityClass.GetsortColumn(advanceSearch.SortBy.Split('_')[0]) + ":" +
                      (advanceSearch.SortBy.Split('_')[1] == "Asc" ? 1 : -1) + "}";
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
                        new BsonDocumentPipelineStageDefinition<ListHubListing, NearbyArea>(
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
                        .ToListAsync()
                        .Result
                        .Select(m => BsonSerializer.Deserialize<PropertyListing>(m));
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
        }

        public IEnumerable<PropertyTypeCheckBox> GetRentPropertyType()
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
                var tmpList = new List<string>();
                using (
                    var cursor = GetCollection().DistinctAsync<string>("PropertyType.Text", new BsonDocument()).Result)
                {
                    while (cursor.MoveNextAsync().Result)
                    {
                        tmpList.AddRange(cursor.Current.ToList());
                    }
                    List<PropertyTypeCheckBox> lstPropertyTypeCheckBox = new List<PropertyTypeCheckBox>();
                    foreach (var item in tmpList)
                    {
                        PropertyTypeCheckBox propertyTypeCheckBox = new PropertyTypeCheckBox();
                        propertyTypeCheckBox.PropertyName = item;
                        propertyTypeCheckBox.IsSelected = false;
                        lstPropertyTypeCheckBox.Add(propertyTypeCheckBox);
                    }
                    return lstPropertyTypeCheckBox;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<SubPropertyTypeCheckBox> GetRentSubPropertyType()
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
                var tmpList = new List<string>();
                using (
                    var cursor = GetCollection().DistinctAsync<string>("PropertySubType.Text", new BsonDocument()).Result)
                {
                    while (cursor.MoveNextAsync().Result)
                    {
                        tmpList.AddRange(cursor.Current.ToList());
                    }
                    List<SubPropertyTypeCheckBox> lstPropertyTypeCheckBox = new List<SubPropertyTypeCheckBox>();
                    foreach (var item in tmpList)
                    {
                        SubPropertyTypeCheckBox propertyTypeCheckBox = new SubPropertyTypeCheckBox();
                        propertyTypeCheckBox.SubPropertyName = item;
                        propertyTypeCheckBox.IsSelected = false;
                        lstPropertyTypeCheckBox.Add(propertyTypeCheckBox);
                    }
                    return lstPropertyTypeCheckBox;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int GetRentRecordCount(AdvanceSearch advanceSearch, bool IsMls)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
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
                    : !isGeoQuery
                        ? "{'Address.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                              advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                              advanceSearch.LotSize,
                              advanceSearch.HomeAge, advanceSearch.SelectedProperty) + "}"
                        : "{" + UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                            advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                            advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + "}";

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
                        new BsonDocumentPipelineStageDefinition<ListHubListing, BsonDocument>(
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
            }
        }

        public IEnumerable<NearbyArea> GetNearByRentAreaDetails(double latitude, double longitude,
            double maxDistanceInMiles)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);

                BsonDocument geoPoint = new BsonDocument
                {
                    {"type", "Point"},
                    {"coordinates", new BsonArray(new double[] {longitude, latitude})}
                };
                var matchQuery = "{ 'Address.City' : {$ne :''},'Address.StateOrProvince' : {$ne :''}}";
                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);

                //Here we are forming the Geo Near Options that consists of Maximum Distance that we have to search till, number of records to be fetched, earth Spherical property to True, calculate the distance from starting point.
                BsonDocument geoNearOptions = new BsonDocument
                {
                    {"near", geoPoint},
                    {"distanceField", "CalculatedDistance"},
                    {"maxDistance", Convert.ToDouble(maxDistanceInMiles)},
                    {"distanceMultiplier", Convert.ToDouble("0.000621371")},
                    {"query", matchDoc},
                    {"limit", 3000},
                    {"spherical", true},
                };

                //MongoDB requires $geoNear as the first stage of a pipeline.
                var stage =
                    new BsonDocumentPipelineStageDefinition<ListHubListing, NearbyArea>(new BsonDocument
                    {
                        {"$geoNear", geoNearOptions}
                    });

                var projectQuery =
                    "{ _id : 0,CalculatedDistance : '$CalculatedDistance',CityWithState  :  { $concat : [ '$Address.City', ', ', '$Address.StateOrProvince']}}";
                var groupQuery =
                    "{_id :'$CityWithState',DistanceArray :  { $addToSet: '$CalculatedDistance' },AvgDistance : {$avg: '$CalculatedDistance'}}}";
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
               .Sort(sortDoc).ToListAsync().Result.Select(m => BsonSerializer.Deserialize<NearbyArea>(m));

                return aggregate;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public ListHubListing GetRentPropertyDetailsByMLSNumber(string mlsNumber)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
                var tmpList = new List<PropertyDetails>();

                var aggregate =
                    GetCollection().Find<ListHubListing>(m => m.MlsNumber == mlsNumber).FirstOrDefaultAsync().Result;
                return aggregate;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<PropertyListing> GetRentIsFeaturedRecordList(int skip, int limit, bool IsMls,
            AdvanceSearch advanceSearch)
        {
            try
            {
                bool isGeoQuery = false;
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);

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
                    : !isGeoQuery
                        ? "{'Address.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                              advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                              advanceSearch.LotSize,
                              advanceSearch.HomeAge, advanceSearch.SelectedProperty) +
                          ",'ExtProperties.IsFeatured':true}"
                        : "{" + UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                            advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                            advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + ",'ExtProperties.IsFeatured':true}";

                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);



                var projectQuery =
                    "{_id : 0,'MlsNumber' : '$MlsNumber','FullStreetAddress':'$Address.FullStreetAddress'," +
                    "'Price':'$ListPrice.Text','NoOfBedRooms':'$Bedrooms','NoOfBathRooms':'$Bathrooms'," +
                    "'NoOfHalfBathRooms':'$HalfBathrooms','PropertyType':{$concat : ['$PropertyType.Text','-','$PropertySubType.Text']}," +
                    "'BrokerageName':'$Brokerage.Name','BrokerageLogoUrl':'$Brokerage.LogoURL','ListingParticipantsName':{$concat : ['$ListingParticipants.Participant.FirstName',' ','$ListingParticipants.Participant.LastName']},'DefaultPhoto':'$Photos.Photo.MediaURL','GeoLocation':'$Location.GeoPoint','State':'$Address.StateOrProvince'," +
                    "'PostalCode':'$Address.PostalCode','City':'$Address.City','LivingArea':'$LivingArea','IsNewConstruction':'$DetailedCharacteristics.IsNewConstruction'}";

                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);
                IEnumerable<PropertyListing> aggregate;
                var sortBy = advanceSearch.SortBy == "0"
                    ? "{Price : 1}"
                    : "{" + Utility.UtilityClass.GetsortColumn(advanceSearch.SortBy.Split('_')[0]) + ":" +
                      (advanceSearch.SortBy.Split('_')[1] == "Asc" ? 1 : -1) + "}";
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
                        new BsonDocumentPipelineStageDefinition<ListHubListing, NearbyArea>(
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
                        .ToListAsync()
                        .Result
                        .Select(m => BsonSerializer.Deserialize<PropertyListing>(m));
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
        }

        public int GetRentIsFeaturedRecordCount(AdvanceSearch advanceSearch, bool IsMls)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
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
                    : !isGeoQuery
                        ? "{'Address.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                              advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                              advanceSearch.LotSize,
                              advanceSearch.HomeAge, advanceSearch.SelectedProperty) +
                          ",'ExtProperties.IsFeatured':true}"
                        : "{" + UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                            advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                            advanceSearch.LotSize,
                          advanceSearch.HomeAge, advanceSearch.SelectedProperty) + ",'ExtProperties.IsFeatured':true}";

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
                        new BsonDocumentPipelineStageDefinition<ListHubListing, BsonDocument>(
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
            }
        }


        #endregion

        #region Utility

        private LatLong GetLatLong(string allowedkeywords)
        {
            var matchQuery = "{'Address.AddressArray': { $in : " + allowedkeywords + "}}";

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



        public List<PropertyListing> GetPurchaseListing(string matchquery, string sortquery, int limit = 0, int skip = 0)
        {
            base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchquery);
            var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sortquery);
            var projectQuery =
                "{_id : 0,'MlsNumber' : '$MlsNumber','FullStreetAddress':'$Address.FullStreetAddress'," +
                "'Price':'$ListPrice.Text','NoOfBedRooms':'$Bedrooms','NoOfBathRooms':'$Bathrooms'," +
                "'NoOfHalfBathRooms':'$HalfBathrooms','PropertyType':{$concat : ['$PropertyType.Text','-','$PropertySubType.Text']}," +
                "'BrokerageName':'$Brokerage.Name','BrokerageLogoUrl':'$Brokerage.LogoURL','ListingParticipantsName':{$concat : ['$ListingParticipants.Participant.FirstName',' ','$ListingParticipants.Participant.LastName']},'DefaultPhoto':'$Photos.Photo.MediaURL','GeoLocation':'$Location.GeoPoint','State':'$Address.StateOrProvince'," +
                "'PostalCode':'$Address.PostalCode','City':'$Address.City','LivingArea':'$LivingArea','IsNewConstruction':'$DetailedCharacteristics.IsNewConstruction','IsFeatured' : '$ExtProperties.IsFeatured'}";
            var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

            var aggregate = GetCollection().Aggregate(new AggregateOptions()
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
            return aggregate.ToList();
        }

        public long GetPurchaseRecordCount(string email)
        {
            base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
            if (!string.IsNullOrEmpty(email))
            {
                return GetCollection().CountAsync(m => m.ListingParticipants.Participant.Email == email).Result;
            }
            else
            {
                return GetCollection().CountAsync(new BsonDocument()).Result;

            }
        }

        public long GetPurchaseRecordCount(BsonDocument matchDoc)
        {
            base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
            return GetCollection().CountAsync(matchDoc).Result;
        }

        public List<PropertyListing> GetRentRecordList(string matchquery, string sortquery, int limit = 0, int skip = 0)
        {
            base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchquery);
            var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sortquery);
            var projectQuery =
                "{_id : 0,'MlsNumber' : '$MlsNumber','FullStreetAddress':'$Address.FullStreetAddress'," +
                "'Price':'$ListPrice.Text','NoOfBedRooms':'$Bedrooms','NoOfBathRooms':'$Bathrooms'," +
                "'NoOfHalfBathRooms':'$HalfBathrooms','PropertyType':{$concat : ['$PropertyType.Text','-','$PropertySubType.Text']}," +
                "'BrokerageName':'$Brokerage.Name','BrokerageLogoUrl':'$Brokerage.LogoURL','ListingParticipantsName':{$concat : ['$ListingParticipants.Participant.FirstName',' ','$ListingParticipants.Participant.LastName']},'DefaultPhoto':'$Photos.Photo.MediaURL','GeoLocation':'$Location.GeoPoint','State':'$Address.StateOrProvince'," +
                "'PostalCode':'$Address.PostalCode','City':'$Address.City','LivingArea':'$LivingArea','IsNewConstruction':'$DetailedCharacteristics.IsNewConstruction','IsFeatured' : '$ExtProperties.IsFeatured'}";
            var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

            var aggregate = GetCollection().Aggregate(new AggregateOptions()
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
            return aggregate.ToList();
        }

        public long GetRentRecordCount(BsonDocument matchDoc)
        {
            base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
            return GetCollection().CountAsync(matchDoc).Result;
        }

        public long GetRentRecordCount(string email)
        {
            base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
            if (!string.IsNullOrEmpty(email))
            {
                return GetCollection().CountAsync(m => m.ListingParticipants.Participant.Email == email).Result;
            }
            else
            {
                return GetCollection().CountAsync(new BsonDocument()).Result;

            }

        }

        public bool SetExtraProperty(string type, ManagePropertyViewModel propertyViewModel)
        {
            try
            {
                base.CollectionName =
                    Convert.ToString(type == "purchase"
                        ? DbCollections.PurchaseListHubFeed
                        : DbCollections.RentListHubFeed);

                var filter = Builders<ListHubListing>.Filter.Eq("MlsNumber", propertyViewModel.UniqueId);
                var update = Builders<ListHubListing>.Update
                    .Set("ExtProperties", propertyViewModel);

                var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;

                return results.ModifiedCount > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ManagePropertyViewModel GetExtraProperty(string type, string mlsid)
        {
            base.CollectionName = Convert.ToString(type == "purchase" ? DbCollections.PurchaseListHubFeed : DbCollections.RentListHubFeed);

            var matchQuery = "{'MlsNumber':'" + mlsid + "'}";
            var projectQuery =
                "{_id : 0,'UniqueId' : '$MlsNumber','IsFeatured':'$ExtProperties.IsFeatured','IsPrinted':'$ExtProperties.IsPrinted'," +
                "'IsSpotlight':'$ExtProperties.IsSpotlight','IsDeleted':'$ExtProperties.IsDeleted','Type':'$ExtProperties.Type'," +
                "'PhotosUrl' : '$Photos.Photo.MediaURL'}";

            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
            var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

            var aggregate = GetCollection().Aggregate(new AggregateOptions()
            {
                AllowDiskUse = true
            })
           .Match(matchDoc)
           .Project(projectDoc);

            var result = aggregate.FirstOrDefaultAsync().Result;
            var myObj = BsonSerializer.Deserialize<ManagePropertyViewModel>(result);
            return myObj;
        }

        public string GetListedType(string type, string mlsid)
        {
            base.CollectionName = Convert.ToString(type == "purchase" ? DbCollections.PurchaseListHubFeed : DbCollections.RentListHubFeed);

            var matchQuery = "{'MlsNumber':'" + mlsid + "'}";
            var projectQuery = "{_id : 0,'ListedBy' : '$ListedBy'}";

            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
            var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

            var aggregate = GetCollection().Aggregate(new AggregateOptions()
            {
                AllowDiskUse = true
            })
           .Match(matchDoc)
           .Project(projectDoc);

            var result = aggregate.FirstOrDefaultAsync().Result;
            var myObj = BsonSerializer.Deserialize<BsonDocument>(result);
            var ListedBy = (string)myObj["ListedBy", null];
            return ListedBy;
        }

        public IEnumerable<PropertyListing> GetPurchaseOpenHouseRecordList(int skip, int limit, bool IsMls,
            AdvanceSearch advanceSearch)
        {
            try
            {
                bool isGeoQuery = false;
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);

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
                    : !isGeoQuery
                        ? "{'Address.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                              advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                              advanceSearch.LotSize,
                              advanceSearch.HomeAge, advanceSearch.SelectedProperty) +
                          ",'ExtProperties.IsFeatured':true,'ExtProperties.OpenHouseStartDateTime':{$lte:new ISODate('" +
                          startDate + "')},'ExtProperties.OpenHouseEndDateTime':{$gte:new ISODate('" + startDate +
                          "')}}"
                        : "{" + UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                            advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                            advanceSearch.LotSize,
                            advanceSearch.HomeAge, advanceSearch.SelectedProperty) +
                          ",'ExtProperties.IsFeatured':true,'ExtProperties.OpenHouseStartDateTime':{$lte:new ISODate('" +
                          startDate + "')},'ExtProperties.OpenHouseEndDateTime':{$gte:new ISODate('" + startDate + "')}}";


                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);



                var projectQuery =
                    "{_id : 0,'MlsNumber' : '$MlsNumber','FullStreetAddress':'$Address.FullStreetAddress'," +
                    "'Price':'$ListPrice.Text','NoOfBedRooms':'$Bedrooms','NoOfBathRooms':'$Bathrooms'," +
                    "'NoOfHalfBathRooms':'$HalfBathrooms','PropertyType':{$concat : ['$PropertyType.Text','-','$PropertySubType.Text']}," +
                    "'BrokerageName':'$Brokerage.Name','BrokerageLogoUrl':'$Brokerage.LogoURL','ListingParticipantsName':{$concat : ['$ListingParticipants.Participant.FirstName',' ','$ListingParticipants.Participant.LastName']},'DefaultPhoto':'$Photos.Photo.MediaURL','GeoLocation':'$Location.GeoPoint','State':'$Address.StateOrProvince'," +
                    "'PostalCode':'$Address.PostalCode','City':'$Address.City','LivingArea':'$LivingArea','IsNewConstruction':'$DetailedCharacteristics.IsNewConstruction','IsFeatured':'$ExtProperties.IsFeatured'}";


                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);
                IEnumerable<PropertyListing> aggregate;
                var sortBy = advanceSearch.SortBy == "0"
                    ? "{Price : 1}"
                    : "{" + Utility.UtilityClass.GetsortColumn(advanceSearch.SortBy.Split('_')[0]) + ":" +
                      (advanceSearch.SortBy.Split('_')[1] == "Asc" ? 1 : -1) + "}";
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
                        new BsonDocumentPipelineStageDefinition<ListHubListing, NearbyArea>(
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
        }

        public int GetPurchaseOpenHouseRecordCount(AdvanceSearch advanceSearch, bool IsMls)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
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
                    : !isGeoQuery
                        ? "{'Address.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                              advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                              advanceSearch.LotSize,
                              advanceSearch.HomeAge, advanceSearch.SelectedProperty) +
                          ",'ExtProperties.IsFeatured':true,'ExtProperties.OpenHouseStartDateTime':{$lte:new ISODate('" +
                          startDate + "')},'ExtProperties.OpenHouseEndDateTime':{$gte:new ISODate('" + startDate +
                          "')}}"
                        : "{" + UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                            advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                            advanceSearch.LotSize,
                            advanceSearch.HomeAge, advanceSearch.SelectedProperty) +
                          ",'ExtProperties.IsFeatured':true,'ExtProperties.OpenHouseStartDateTime':{$lte:new ISODate('" +
                          startDate + "')},'ExtProperties.OpenHouseEndDateTime':{$gte:new ISODate('" + startDate + "')}}";

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
                        new BsonDocumentPipelineStageDefinition<ListHubListing, BsonDocument>(
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
            }
        }


        public IEnumerable<PropertyListing> GetRentOpenHouseRecordList(int skip, int limit, bool IsMls,
            AdvanceSearch advanceSearch)
        {
            try
            {
                bool isGeoQuery = false;
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);

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
                    : !isGeoQuery
                        ? "{'Address.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                              advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                              advanceSearch.LotSize,
                              advanceSearch.HomeAge, advanceSearch.SelectedProperty) +
                          ",'ExtProperties.IsFeatured':true,'ExtProperties.OpenHouseStartDateTime':{$lte:new ISODate('" +
                          startDate + "')},'ExtProperties.OpenHouseEndDateTime':{$gte:new ISODate('" + startDate +
                          "')}}"
                        : "{" + UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                            advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                            advanceSearch.LotSize,
                            advanceSearch.HomeAge, advanceSearch.SelectedProperty) +
                          ",'ExtProperties.IsFeatured':true,'ExtProperties.OpenHouseStartDateTime':{$lte:new ISODate('" +
                          startDate + "')},'ExtProperties.OpenHouseEndDateTime':{$gte:new ISODate('" + startDate + "')}}";


                var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);



                var projectQuery =
                    "{_id : 0,'MlsNumber' : '$MlsNumber','FullStreetAddress':'$Address.FullStreetAddress'," +
                    "'Price':'$ListPrice.Text','NoOfBedRooms':'$Bedrooms','NoOfBathRooms':'$Bathrooms'," +
                    "'NoOfHalfBathRooms':'$HalfBathrooms','PropertyType':{$concat : ['$PropertyType.Text','-','$PropertySubType.Text']}," +
                    "'BrokerageName':'$Brokerage.Name','BrokerageLogoUrl':'$Brokerage.LogoURL','ListingParticipantsName':{$concat : ['$ListingParticipants.Participant.FirstName',' ','$ListingParticipants.Participant.LastName']},'DefaultPhoto':'$Photos.Photo.MediaURL','GeoLocation':'$Location.GeoPoint','State':'$Address.StateOrProvince'," +
                    "'PostalCode':'$Address.PostalCode','City':'$Address.City','LivingArea':'$LivingArea','IsNewConstruction':'$DetailedCharacteristics.IsNewConstruction','IsFeatured':'$ExtProperties.IsFeatured'}";


                var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);
                IEnumerable<PropertyListing> aggregate;
                var sortBy = advanceSearch.SortBy == "0"
                    ? "{Price : 1}"
                    : "{" + Utility.UtilityClass.GetsortColumn(advanceSearch.SortBy.Split('_')[0]) + ":" +
                      (advanceSearch.SortBy.Split('_')[1] == "Asc" ? 1 : -1) + "}";
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
                        new BsonDocumentPipelineStageDefinition<ListHubListing, NearbyArea>(
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
        }

        public int GetRentOpenHouseRecordCount(AdvanceSearch advanceSearch, bool IsMls)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
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
                    : !isGeoQuery
                        ? "{'Address.AddressArray': { $all : " + allowedkeywords + "}" +
                      UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                              advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                              advanceSearch.LotSize,
                              advanceSearch.HomeAge, advanceSearch.SelectedProperty) +
                          ",'ExtProperties.IsFeatured':true,'ExtProperties.OpenHouseStartDateTime':{$lte:new ISODate('" +
                          startDate + "')},'ExtProperties.OpenHouseEndDateTime':{$gte:new ISODate('" + startDate +
                          "')}}"
                        : "{" + UtilityClass.CreateQueryString(advanceSearch.MinPrice, advanceSearch.MaxPrice,
                            advanceSearch.NoOfBeds, advanceSearch.NoOfBathroom, advanceSearch.Size,
                            advanceSearch.LotSize,
                            advanceSearch.HomeAge, advanceSearch.SelectedProperty) +
                          ",'ExtProperties.IsFeatured':true,'ExtProperties.OpenHouseStartDateTime':{$lte:new ISODate('" +
                          startDate + "')},'ExtProperties.OpenHouseEndDateTime':{$gte:new ISODate('" + startDate + "')}}";

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
                        new BsonDocumentPipelineStageDefinition<ListHubListing, BsonDocument>(
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
            }
        }


        public bool InsertRealestate(ListHubListing entities, string type)
        {
            try
            {
                if (type == "purchase")
                {
                    base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
                }
                else if (type == "rent")
                {
                    base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
                }
                else
                {
                    base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
                }
                //var filter = Builders<ListHubListing>.Filter.Eq("UniqueNo", entities.UniqueNo);
                //var update = Builders<ListHubListing>.Update
                //    .Set("ExtProperties", entities);

                GetCollection().InsertOneAsync(entities).Wait();

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public IEnumerable<PropertyType> GetPropertyType(string type)
        {
            try
            {
                base.CollectionName = type == "purchase"
                    ? Convert.ToString(DbCollections.PurchaseListHubFeed)
                    : Convert.ToString(DbCollections.RentListHubFeed);
                var tmpList = new List<string>();
                using (
                    var cursor = GetCollection().DistinctAsync<string>("PropertyType.Text", new BsonDocument()).Result)
                {
                    while (cursor.MoveNextAsync().Result)
                    {
                        tmpList.AddRange(cursor.Current.ToList());
                    }
                    List<PropertyType> lstPropertyTypeCheckBox = new List<PropertyType>();
                    foreach (var item in tmpList)
                    {
                        PropertyType propertyTypeCheckBox = new PropertyType();
                        propertyTypeCheckBox.Text = item;
                        lstPropertyTypeCheckBox.Add(propertyTypeCheckBox);
                    }
                    return lstPropertyTypeCheckBox;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<PropertyType> GetPropertyTypeDesc(string type)
        {
            try
            {
                base.CollectionName = type == "purchase"
                    ? Convert.ToString(DbCollections.PurchaseListHubFeed)
                    : Convert.ToString(DbCollections.RentListHubFeed);
                var tmpList = new List<string>();
                using (
                    var cursor =
                        GetCollection()
                            .DistinctAsync<string>("PropertyType.OtherDescription", new BsonDocument())
                            .Result)
                {
                    while (cursor.MoveNextAsync().Result)
                    {
                        tmpList.AddRange(cursor.Current.ToList());
                    }
                    List<PropertyType> lstPropertyTypeCheckBox = new List<PropertyType>();
                    foreach (var item in tmpList)
                    {
                        PropertyType propertyTypeCheckBox = new PropertyType();
                        propertyTypeCheckBox.OtherDescription = item;
                        lstPropertyTypeCheckBox.Add(propertyTypeCheckBox);
                    }
                    return lstPropertyTypeCheckBox;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<PropertySubType> GetSubPropertyType(string type)
        {
            try
            {
                base.CollectionName = type == "purchase"
                    ? Convert.ToString(DbCollections.PurchaseListHubFeed)
                    : Convert.ToString(DbCollections.RentListHubFeed);
                var tmpList = new List<string>();
                using (
                    var cursor =
                        GetCollection().DistinctAsync<string>("PropertySubType.Text", new BsonDocument()).Result)
                {
                    while (cursor.MoveNextAsync().Result)
                    {
                        tmpList.AddRange(cursor.Current.ToList());
                    }
                    List<PropertySubType> lstPropertyTypeCheckBox = new List<PropertySubType>();
                    foreach (var item in tmpList)
                    {
                        PropertySubType propertyTypeCheckBox = new PropertySubType();
                        propertyTypeCheckBox.Text = item;
                        lstPropertyTypeCheckBox.Add(propertyTypeCheckBox);
                    }
                    return lstPropertyTypeCheckBox;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<PropertySubType> GetSubPropertyTypeDesc(string type)
        {
            try
            {
                base.CollectionName = type == "purchase"
                    ? Convert.ToString(DbCollections.PurchaseListHubFeed)
                    : Convert.ToString(DbCollections.RentListHubFeed);
                var tmpList = new List<string>();
                using (
                    var cursor =
                        GetCollection()
                            .DistinctAsync<string>("PropertySubType.OtherDescription", new BsonDocument())
                            .Result)
                {
                    while (cursor.MoveNextAsync().Result)
                    {
                        tmpList.AddRange(cursor.Current.ToList());
                    }
                    List<PropertySubType> lstPropertyTypeCheckBox = new List<PropertySubType>();
                    foreach (var item in tmpList)
                    {
                        PropertySubType propertyTypeCheckBox = new PropertySubType();
                        propertyTypeCheckBox.OtherDescription = item;
                        lstPropertyTypeCheckBox.Add(propertyTypeCheckBox);
                    }
                    return lstPropertyTypeCheckBox;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public IEnumerable<string> GetListingStatus(string type)
        {
            try
            {
                base.CollectionName = type == "purchase"
                    ? Convert.ToString(DbCollections.PurchaseListHubFeed)
                    : Convert.ToString(DbCollections.RentListHubFeed);
                var tmpList = new List<string>();
                using (var cursor = GetCollection().DistinctAsync<string>("ListingStatus", new BsonDocument()).Result)
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

        public bool UpdateImageList(string type, ListHubListing propertyViewModel)
        {
            try
            {
                base.CollectionName =
                    Convert.ToString(type == "purchase"
                        ? DbCollections.PurchaseListHubFeed
                        : DbCollections.RentListHubFeed);

                var filter = Builders<ListHubListing>.Filter.Eq("MlsNumber", propertyViewModel.MlsNumber);
                var update = Builders<ListHubListing>.Update
                    .PushEach("Photos.Photo", propertyViewModel.Photos.Photo);

                var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;

                return results.ModifiedCount > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool DeleteProperty(string type, string mlsNumber, bool isDeleted)
        {
            try
            {
                base.CollectionName = type == "purchase" ? Convert.ToString(DbCollections.PurchaseListHubFeed) : Convert.ToString(DbCollections.RentListHubFeed);

                var filter = Builders<ListHubListing>.Filter.Eq("MlsNumber", mlsNumber);
                var update = Builders<ListHubListing>.Update
                    .Set("ExtProperties.IsDeleted", isDeleted);

                var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;

                return results.ModifiedCount > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void ProcessFeed(List<ListHubListing> hubListings)
        {

            Console.WriteLine("Feed processing started.. ");
            Console.WriteLine(Environment.NewLine);

            foreach (var newlisthublisting in hubListings)
            {
                Console.WriteLine(".");
                var category = Convert.ToString(newlisthublisting.ListingCategory).ToLower();
                if (!newlisthublisting.IsClassified)
                {
                    newlisthublisting.ExpireDate = DateTime.MaxValue.Date;
                }
                //newlisthublisting.Bathrooms = newlisthublisting.FullBathrooms + newlisthublisting.HalfBathrooms + newlisthublisting.ThreeQuarterBathrooms + newlisthublisting.OneQuarterBathrooms;

                var hasAnyDoc = this.GetPurchasePropertyDetailsByMLSNumber(newlisthublisting.MlsNumber);
                // Insert If No previous document found
                if (hasAnyDoc == null)
                {
                    this.InsertRealestate(newlisthublisting, category);
                }
                else if (hasAnyDoc.ExpireDate.HasValue && hasAnyDoc.ExpireDate.Value.Date < DateTime.UtcNow.Date)
                {
                    if (hasAnyDoc.ExtProperties == null)
                    {
                        hasAnyDoc.ExtProperties = new ManagePropertyViewModel
                        {
                            IsDeleted = true
                        };
                    }
                    else
                    {
                        hasAnyDoc.ExtProperties.IsDeleted = true;
                    }
                }

                else
                {
                    // Update if previous document found but w.r.t Modification timestamp we are updating the collection
                    var newlisting = newlisthublisting;
                    var oldlisting = hasAnyDoc;

                    bool checkTimeStamp = false;
                    if (newlisting.IsClassified == false)
                    {
                        var newModifiedDate = Convert.ToDateTime(newlisting.ModificationTimestamp.Text);
                        var oldModifiedDate = Convert.ToDateTime(oldlisting.ModificationTimestamp.Text);
                        checkTimeStamp = newModifiedDate > oldModifiedDate;
                    }

                    if (checkTimeStamp)
                    {

                        var compareObjects = new CompareLogic()
                        {
                            Config = new ComparisonConfig()
                            {
                                CompareChildren = true, //this turns deep compare one, otherwise it's shallow
                                CompareFields = false,
                                CompareReadOnly = true,
                                ComparePrivateFields = false,
                                ComparePrivateProperties = false,
                                CompareProperties = true,
                                MaxDifferences = 10000,
                                IgnoreUnknownObjectTypes = true
                            }
                        };

                        var resultCompare = compareObjects.Compare(newlisting, oldlisting);

                        var diff = resultCompare.Differences;
                        if (diff.Count > 0)
                        {
                            foreach (var chnagedProperty in diff)
                            {
                                if (chnagedProperty.ParentPropertyName != ".Id")
                                {
                                    if (chnagedProperty.PropertyName.ToLower().Contains("addressarray"))
                                    {
                                        if (chnagedProperty.PropertyName.Contains("0"))
                                        {
                                            this.SetUnSetSpecificProperty(category, newlisthublisting.MlsNumber,
                                                chnagedProperty.PropertyName.Remove(0, 1)
                                                    .Replace("[", ".")
                                                    .Replace("]", ""),
                                                newlisting);
                                        }

                                    }
                                    else if (chnagedProperty.PropertyName.ToLower().Contains(".photos.photo"))
                                    {
                                        if (chnagedProperty.PropertyName.ToLower() == ".photos.photo")
                                        {
                                            this.SetUnSetSpecificProperty(category, newlisthublisting.MlsNumber,
                                                chnagedProperty.PropertyName.Remove(0, 1)
                                                    .Replace("[", ".")
                                                    .Replace("]", ""),
                                                newlisting);
                                        }

                                    }
                                    else if (chnagedProperty.PropertyName.ToLower().Contains(".brokerage.address"))
                                    {
                                        if (oldlisting.Brokerage.Address == null)
                                        {
                                            this.SetUnSetSpecificProperty(category, newlisthublisting.MlsNumber,
                                                chnagedProperty.PropertyName.Remove(0, 1)
                                                    .Replace("[", ".")
                                                    .Replace("]", ""),
                                                newlisting, true);
                                        }
                                        else
                                        {

                                            this.SetSpecificProperty(category, newlisthublisting.MlsNumber,
                                                chnagedProperty.PropertyName.Remove(0, 1)
                                                    .Replace("[", ".")
                                                    .Replace("]", ""),
                                                chnagedProperty.Object1Value);
                                        }
                                    }
                                    else
                                    {
                                        if (!chnagedProperty.PropertyName.ToLower().Contains("expiredate")&& !chnagedProperty.PropertyName.ToLower().Contains("extproperties") && !chnagedProperty.PropertyName.ToLower().Contains(".location.geopoint"))
                                        {
                                            this.SetSpecificProperty(category, newlisthublisting.MlsNumber,
                                                chnagedProperty.PropertyName.Remove(0, 1)
                                                    .Replace("[", ".")
                                                    .Replace("]", ""),
                                                chnagedProperty.Object1Value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Feed processing ended.. ");
        }

        public void CreateIndex()
        {
            base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);

            GetCollection().Indexes.CreateOneAsync(Builders<ListHubListing>.IndexKeys.Combine(
                Builders<ListHubListing>.IndexKeys.Ascending(_ => _.Address.FullAddress),
                Builders<ListHubListing>.IndexKeys.Ascending(_ => _.Address.Country),
                Builders<ListHubListing>.IndexKeys.Ascending(_ => _.Address.PostalCode),
                Builders<ListHubListing>.IndexKeys.Ascending(_ => _.Address.StateOrProvince),
                Builders<ListHubListing>.IndexKeys.Ascending(_ => _.Address.City),
                Builders<ListHubListing>.IndexKeys.Ascending(_ => _.Address.FullStreetAddress),
                Builders<ListHubListing>.IndexKeys.Ascending(_ => _.MlsNumber),
                Builders<ListHubListing>.IndexKeys.Geo2DSphere(_ => _.Location.GeoPoint)));

            base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);

            GetCollection().Indexes.CreateOneAsync(Builders<ListHubListing>.IndexKeys.Combine(
                Builders<ListHubListing>.IndexKeys.Ascending(_ => _.Address.FullAddress),
                Builders<ListHubListing>.IndexKeys.Ascending(_ => _.Address.Country),
                Builders<ListHubListing>.IndexKeys.Ascending(_ => _.Address.PostalCode),
                Builders<ListHubListing>.IndexKeys.Ascending(_ => _.Address.StateOrProvince),
                Builders<ListHubListing>.IndexKeys.Ascending(_ => _.Address.City),
                Builders<ListHubListing>.IndexKeys.Ascending(_ => _.Address.FullStreetAddress),
                Builders<ListHubListing>.IndexKeys.Ascending(_ => _.MlsNumber),
                Builders<ListHubListing>.IndexKeys.Geo2DSphere(_ => _.Location.GeoPoint)));


        }


        bool SetSpecificProperty(string type, string uniqueidentifier, string propToUpdate, string value)
        {
            try
            {
                if (value == "(null)")
                {
                    value = null;
                }

                base.CollectionName =
                    Convert.ToString(type == "purchase"
                        ? DbCollections.PurchaseListHubFeed
                        : DbCollections.RentListHubFeed);

                var filter = Builders<ListHubListing>.Filter.Eq("MlsNumber", uniqueidentifier);
                var update = Builders<ListHubListing>.Update
                    .Set(propToUpdate, value);

                var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;

                return results.ModifiedCount > 0;
            }
            catch (Exception)
            {

                return false;
            }
        }
        private bool SetUnSetSpecificProperty(string type,
            string uniqueidentifier, string unsetpropertyname, ListHubListing hubListing, bool onlySet = false)
        {
            try
            {
                base.CollectionName =
                    Convert.ToString(type == "purchase"
                        ? DbCollections.PurchaseListHubFeed
                        : DbCollections.RentListHubFeed);

                var filter = Builders<ListHubListing>.Filter.Eq("MlsNumber", uniqueidentifier);
                if (onlySet)
                {
                    var update = Builders<ListHubListing>.Update.Set(unsetpropertyname,
                          hubListing.Brokerage.Address);
                    var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;

                    return results.ModifiedCount > 0;
                }
                else
                {

                    if (unsetpropertyname == "Photos.Photo")
                    {
                        var update1 = Builders<ListHubListing>.Update.Unset(unsetpropertyname);
                        var tmpResult = GetCollection().UpdateOneAsync(filter, update1, new UpdateOptions()).Result;

                        if (tmpResult.ModifiedCount > 0)
                        {
                            var update = Builders<ListHubListing>.Update.PushEach(unsetpropertyname,
                                hubListing.Photos.Photo);
                            var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;

                            return results.ModifiedCount > 0;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    else if (unsetpropertyname.Contains("Offices.Office.Address.AddressArray"))
                    {
                        unsetpropertyname = unsetpropertyname.Remove(unsetpropertyname.Length - 2, 2);

                        var update1 = Builders<ListHubListing>.Update.Unset(unsetpropertyname);
                        var tmpResult = GetCollection().UpdateOneAsync(filter, update1, new UpdateOptions()).Result;

                        if (tmpResult.ModifiedCount > 0)
                        {
                            var update = Builders<ListHubListing>.Update.PushEach(unsetpropertyname,
                                hubListing.Offices.Office.Address.AddressArray);
                            var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;

                            return results.ModifiedCount > 0;
                        }
                        else
                        {
                            return false;
                        }

                    }
                    else if (unsetpropertyname.Contains("Brokerage.Address.AddressArray"))
                    {
                        unsetpropertyname = unsetpropertyname.Remove(unsetpropertyname.Length - 2, 2);
                        var update1 = Builders<ListHubListing>.Update.Unset(unsetpropertyname);
                        var tmpResult = GetCollection().UpdateOneAsync(filter, update1, new UpdateOptions()).Result;

                        if (tmpResult.ModifiedCount > 0)
                        {
                            var update = Builders<ListHubListing>.Update.PushEach(unsetpropertyname,
                                hubListing.Brokerage.Address.AddressArray);
                            var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;

                            return results.ModifiedCount > 0;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        var update1 = Builders<ListHubListing>.Update.Unset(unsetpropertyname);
                        var tmpResult = GetCollection().UpdateOneAsync(filter, update1, new UpdateOptions()).Result;

                        if (tmpResult.ModifiedCount > 0)
                        {
                            var update = Builders<ListHubListing>.Update.PushEach(unsetpropertyname,
                                hubListing.Address.AddressArray);
                            var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions()).Result;

                            return results.ModifiedCount > 0;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool UpdateForAgent(User user, string type)
        {
            if (type == "purchase")
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
                var filter = Builders<ListHubListing>.Filter.Eq("ListingParticipants.Email", user.Email);
                var update = Builders<ListHubListing>.Update
                                                     .Set("ListingParticipants.ParticipantId", user.ParticipantId)
                                                     .Set("ListingParticipants.ParticipantKey", user.ParticipantKey)
                                                     .Set("ListingParticipants.FirstName", user.FirstName)
                                                     .Set("ListingParticipants.LastName", user.LastName)
                                                     .Set("ListingParticipants.PrimaryContactPhone", user.PrimaryContactPhone)
                                                     .Set("ListingParticipants.OfficePhone", user.OfficePhone)
                                                     .Set("ListingParticipants.WebsiteURL", user.WebsiteURL);
                GetCollection().UpdateManyAsync(filter, update);
            }
            else if (type == "rent")
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
                var filter = Builders<ListHubListing>.Filter.Eq("ListingParticipants.Email", user.Email);
                var update = Builders<ListHubListing>.Update
                                                     .Set("ListingParticipants.ParticipantId", user.ParticipantId)
                                                     .Set("ListingParticipants.ParticipantKey", user.ParticipantKey)
                                                     .Set("ListingParticipants.FirstName", user.FirstName)
                                                     .Set("ListingParticipants.LastName", user.LastName)
                                                     .Set("ListingParticipants.PrimaryContactPhone", user.PrimaryContactPhone)
                                                     .Set("ListingParticipants.OfficePhone", user.OfficePhone)
                                                     .Set("ListingParticipants.WebsiteURL", user.WebsiteURL);
                GetCollection().UpdateManyAsync(filter, update);
            }


            return true;
        }

        public bool MakeBulkFeatured(string fieldbyUpdate, bool value, string uniqueid, string type)
        {
            if (type == "rent")
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);


            }
            else
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
            }

            var filter = Builders<ListHubListing>.Filter.Eq(fieldbyUpdate, uniqueid);
            var update = Builders<ListHubListing>.Update
                .Set(m => m.ExtProperties.IsFeatured, value);
            var res = GetCollection().UpdateManyAsync(filter, update);
            return res.Result.ModifiedCount > 0;
        }

        public bool DeleteListHub(string mlsid, string type)
        {
            if (type == "rent")
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
            }
            else if (type == "purchase")
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
            }

            var filter = Builders<ListHubListing>.Filter.Eq("MlsNumber", mlsid);
            var update = Builders<ListHubListing>.Update.Set(x => x.IsDeletedByPortal, true);

            var result = GetCollection().UpdateOneAsync(filter, update).Result;

            return result.ModifiedCount > 0;
        }

        public bool UpdateListHub(string type, ListHubListing _ListHubListing)
        {
            if (type == "rent")
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
            }
            else
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
            }
            var filter = Builders<ListHubListing>.Filter.Eq("MlsNumber", _ListHubListing.MlsNumber);
            var update = Builders<ListHubListing>.Update
                .Set(m => m.ProviderName, _ListHubListing.ProviderName)
                .Set(m => m.Offices.Office, _ListHubListing.Offices.Office)
                .Set(m => m.CommunityName, _ListHubListing.CommunityName)
                .Set(m => m.ListingParticipants.Participant, _ListHubListing.ListingParticipants.Participant)
                .Set(m => m.ProviderURL, _ListHubListing.ProviderURL)
                .Set(m => m.PropertyType, _ListHubListing.PropertyType)
                .Set(m => m.PropertySubType, _ListHubListing.PropertySubType)
                .Set(m => m.Address, _ListHubListing.Address)
                .Set(m => m.ListPrice, _ListHubListing.ListPrice)
                .Set(m => m.Bedrooms, _ListHubListing.Bedrooms)
                .Set(m => m.FullBathrooms, _ListHubListing.FullBathrooms)
                .Set(m => m.OneQuarterBathrooms, _ListHubListing.OneQuarterBathrooms)
                .Set(m => m.HalfBathrooms, _ListHubListing.HalfBathrooms)
                .Set(m => m.PartialBathrooms, _ListHubListing.PartialBathrooms)
                .Set(m => m.ThreeQuarterBathrooms, _ListHubListing.ThreeQuarterBathrooms)
                .Set(m => m.ListingStatus, _ListHubListing.ListingStatus)
                .Set(m => m.ListingDescription, _ListHubListing.ListingDescription)
                .Set(m => m.LotSize, _ListHubListing.LotSize)
                .Set(m => m.LivingArea, _ListHubListing.LivingArea)
                .Set(m => m.YearBuilt, _ListHubListing.YearBuilt)
                .Set(m => m.ListingTitle, _ListHubListing.ListingTitle)
                .Set(m => m.IsUpdateByPortal, _ListHubListing.IsUpdateByPortal)
                ;
            var res = GetCollection().UpdateOneAsync(filter, update).Result;
            return res.ModifiedCount > 0;

        }

        #region Added for ClassifiedFeed


        public bool InsertRealestate(List<ListHubListing> entities, string type)
        {
            try
            {
                if (type == "purchase")
                {
                    base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
                }
                else if (type == "rent")
                {
                    base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
                }
                else
                {
                    base.CollectionName = Convert.ToString(DbCollections.NewHomesFeed);
                }
                //var filter = Builders<ListHubListing>.Filter.Eq("UniqueNo", entities.UniqueNo);
                //var update = Builders<ListHubListing>.Update
                //    .Set("ExtProperties", entities);

                GetCollection().InsertManyAsync(entities).Wait();

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion





        public List<PropertyDetails> GetClassified(string type, int count)
        {
            if (type == "rent")
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
            }
            else
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
            }
            var matchQuery = @"
                               {
                                  $or: [
                                        {
                                          $and: [ 
          		                                   {'IsClassified' : true},
                                                   {'IsDeletedByPortal' : {$exists : false}}
                                                ]
                                        },
                                        {
                                          $and: [  
                                                   {'IsClassified' : true},
                                                   {'IsDeletedByPortal' : {$exists : true}},
                                                   {'IsDeletedByPortal' : false}
                                                ]
                                        }
                                       ]
                               }
                              ";
            var sortQuery = "{'ExpireDate' : -1}";
            var projectQuery =
                   "{_id : 0,'FullStreetAddress' : '$Address.FullStreetAddress','City':'$Address.City','StateOrProvince':'$Address.StateOrProvince'," +
               "'PostalCode':'$Address.PostalCode','MlsNumber':1,'ListPrice':'$ListPrice.Text','ListingURL':1,'ProviderName':1,'ProviderURL':1,'ProviderCategory':1," +
               "'LeadRoutingEmail':1,'ListingStatus':1,'ListingDescription':1,'ListingDate':1,'ListingTitle':1,'Bedrooms':1,'Bathrooms':1,'FullBathrooms':1," +
               "'ThreeQuarterBathrooms':1,'HalfBathrooms':1,'OneQuarterBathrooms':1,'PartialBathrooms':1,'LotSize':'$LotSize.Text','PropertyType':'$PropertyType.Text'," +
               "'PropertySubType':'$PropertySubType.Text','ListingKey':1,'ListingCategory':1,'Location':'$Location.Directions','Photos':'$Photos.Photo.MediaURL'," +
               "'Schools':'$Location.Community.Schools.School','Offices':'$Offices.Office.OfficeKey','OfficeId':'$Offices.Office.OfficeId'," +
               "'OfficeCodeId':'$Offices.Office.OfficeCode.OfficeCodeId','OfficeName':'$Offices.Office.Name','CorporateName':'$Offices.Office.CorporateName'," +
               "'BrokerId':'$Offices.Office.BrokerId','MainOfficeId':'$Offices.Office.MainOfficeId','OfficePhoneNumber':'$Offices.Office.PhoneNumber'," +
               "'OfficeFullStreetAddress':'$Offices.Office.Address.FullStreetAddress','OfficeCity':'$Offices.Office.Address.City'," +
               "'OfficeState':'$Offices.Office.Address.StateOrProvince','OfficePostalCode':'$Offices.Office.Address.PostalCode','BrokerageName':'$Brokerage.Name'," +
               "'BrokeragePhone':'$Brokerage.Phone','BrokerageEmail':'$Brokerage.Email','BrokerageWebsite':'$Brokerage.WebsiteURL','BrokerageLogoURL':'$Brokerage.LogoURL'," +
               "'BrokerageFullStreetAddress':'$Brokerage.Address.FullAddressStreet','BrokerageCity':'$Brokerage.Address.City'," +
               "'BrokerageState':'$Brokerage.Address.StateOrProvince','BrokeragePostalCode':'$Brokerage.Address.PostalCode'," +
               "'Appliance':'$DetailedCharacteristics.Appliances.Appliance','ArchitectureStyle':'$DetailedCharacteristics.ArchitectureStyle.Text'," +
               "'CoolingSystem':'$DetailedCharacteristics.CoolingSystems.CoolingSystem','ExteriorType':'$DetailedCharacteristics.ExteriorTypes.ExteriorType'," +
               "'HeatingSystem':'$DetailedCharacteristics.HeatingSystems.HeatingSystem','IsNewConstruction':'$DetailedCharacteristics.IsNewConstruction'," +
               "'NoOfFloor':'$DetailedCharacteristics.NumFloors','NoOfParkingSpace':'$DetailedCharacteristics.NumParkingSpaces'," +
               "'ParkingType':'$DetailedCharacteristics.ParkingTypes.ParkingType','RoofTypes':'$DetailedCharacteristics.RoofTypes.RoofType'," +
               "'Rooms':'$DetailedCharacteristics.Rooms.Room','LivingArea':'$LivingArea','IsFeatured':'$ExtProperties.IsFeatured','IsSpotLight':'$ExtProperties.IsSpotlight','ClassifiedExpireDate':'$ExpireDate','GeoPoint':'$Location.GeoPoint'}";

            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
            var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sortQuery);
            var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

            var results = GetCollection().Aggregate(new AggregateOptions()
            {
                AllowDiskUse = true
            })
                .Match(matchDoc)
                .Project(projectDoc)
                .Limit(count)
                .Sort(sortDoc)
                .ToListAsync()
                .Result.Select(m => BsonSerializer.Deserialize<PropertyDetails>(m));


            return results.ToList();
        }

        public PropertyDetails GetPropertyBasedOnCommunity(string type, string communityName)
        {
            if (type == "rent")
            {
                base.CollectionName = Convert.ToString(DbCollections.RentListHubFeed);
            }
            else
            {
                base.CollectionName = Convert.ToString(DbCollections.PurchaseListHubFeed);
            }
            var matchQuery = @"{'CommunityName' : { $regex: '" + communityName + "', $options: 'i' }}";
            var sortQuery = "{'ExpireDate' : -1}";
            var projectQuery =
                  "{_id : 0,'FullStreetAddress' : '$Address.FullStreetAddress','City':'$Address.City','StateOrProvince':'$Address.StateOrProvince'," +
              "'PostalCode':'$Address.PostalCode','MlsNumber':1,'ListPrice':'$ListPrice.Text','ListingURL':1,'ProviderName':1,'ProviderURL':1,'ProviderCategory':1," +
              "'LeadRoutingEmail':1,'ListingStatus':1,'ListingDescription':1,'ListingDate':1,'ListingTitle':1,'Bedrooms':1,'Bathrooms':1,'FullBathrooms':1," +
              "'ThreeQuarterBathrooms':1,'HalfBathrooms':1,'OneQuarterBathrooms':1,'PartialBathrooms':1,'LotSize':'$LotSize.Text','PropertyType':'$PropertyType.Text'," +
              "'PropertySubType':'$PropertySubType.Text','ListingKey':1,'ListingCategory':1,'Location':'$Location.Directions','Photos':'$Photos.Photo.MediaURL'," +
              "'Schools':'$Location.Community.Schools.School','Offices':'$Offices.Office.OfficeKey','OfficeId':'$Offices.Office.OfficeId'," +
              "'OfficeCodeId':'$Offices.Office.OfficeCode.OfficeCodeId','OfficeName':'$Offices.Office.Name','CorporateName':'$Offices.Office.CorporateName'," +
              "'BrokerId':'$Offices.Office.BrokerId','MainOfficeId':'$Offices.Office.MainOfficeId','OfficePhoneNumber':'$Offices.Office.PhoneNumber'," +
              "'OfficeFullStreetAddress':'$Offices.Office.Address.FullStreetAddress','OfficeCity':'$Offices.Office.Address.City'," +
              "'OfficeState':'$Offices.Office.Address.StateOrProvince','OfficePostalCode':'$Offices.Office.Address.PostalCode','BrokerageName':'$Brokerage.Name'," +
              "'BrokeragePhone':'$Brokerage.Phone','BrokerageEmail':'$Brokerage.Email','BrokerageWebsite':'$Brokerage.WebsiteURL','BrokerageLogoURL':'$Brokerage.LogoURL'," +
              "'BrokerageFullStreetAddress':'$Brokerage.Address.FullAddressStreet','BrokerageCity':'$Brokerage.Address.City'," +
              "'BrokerageState':'$Brokerage.Address.StateOrProvince','BrokeragePostalCode':'$Brokerage.Address.PostalCode'," +
              "'Appliance':'$DetailedCharacteristics.Appliances.Appliance','ArchitectureStyle':'$DetailedCharacteristics.ArchitectureStyle.Text'," +
              "'CoolingSystem':'$DetailedCharacteristics.CoolingSystems.CoolingSystem','ExteriorType':'$DetailedCharacteristics.ExteriorTypes.ExteriorType'," +
              "'HeatingSystem':'$DetailedCharacteristics.HeatingSystems.HeatingSystem','IsNewConstruction':'$DetailedCharacteristics.IsNewConstruction'," +
              "'NoOfFloor':'$DetailedCharacteristics.NumFloors','NoOfParkingSpace':'$DetailedCharacteristics.NumParkingSpaces'," +
              "'ParkingType':'$DetailedCharacteristics.ParkingTypes.ParkingType','RoofTypes':'$DetailedCharacteristics.RoofTypes.RoofType'," +
              "'Rooms':'$DetailedCharacteristics.Rooms.Room','LivingArea':'$LivingArea','IsFeatured':'$ExtProperties.IsFeatured','IsSpotLight':'$ExtProperties.IsSpotlight','ClassifiedExpireDate':'$ExpireDate','GeoPoint':'$Location.GeoPoint'}";

            var matchDoc = BsonSerializer.Deserialize<BsonDocument>(matchQuery);
            var sortDoc = BsonSerializer.Deserialize<BsonDocument>(sortQuery);
            var projectDoc = BsonSerializer.Deserialize<BsonDocument>(projectQuery);

            var results = GetCollection().Aggregate(new AggregateOptions()
            {
                AllowDiskUse = true
            })
                 .Match(matchDoc)
                 .Project(projectDoc)
                 .Limit(1)
                 .Sort(sortDoc)
                 .ToListAsync()
                 .Result.Select(m => BsonSerializer.Deserialize<PropertyDetails>(m));


            return results.FirstOrDefault();
        }


        public bool PullImage(string imageurl, string mlsNumber, string Type)
        {
            try
            {
                base.CollectionName = Type == "purchase" ? Convert.ToString(DbCollections.PurchaseListHubFeed) : Convert.ToString(DbCollections.RentListHubFeed);
                var filter = Builders<ListHubListing>.Filter.Eq(m => m.MlsNumber, mlsNumber);
                var update = Builders<ListHubListing>.Update.PullFilter(p => p.Photos.Photo, f => f.MediaURL == imageurl);
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
