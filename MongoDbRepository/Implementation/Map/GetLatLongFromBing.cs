using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Core.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using Repositories.Interfaces.Map;
using Repositories.Models.Map;
using RestSharp;
using Utility;

namespace Core.Implementation.Map
{
    public class GetLatLongFromBing : Repository<Coordinates>, IFetchLatLong
    {
        private new const string CollectionName = "";


        public GetLatLongFromBing()
            : base(CollectionName)
        {
        }

        public GetLatLongFromBing(IMongoDatabase database)
            : base(database, CollectionName)
        {

        }

        public GetLatLongFromBing(string connectionString, string databaseName)
            : base(connectionString, databaseName, CollectionName)
        {
        }

        public Coordinates GetLatitudeAndLongitude(string address)
        {
            Coordinates objCor = new Coordinates();
            var apiKey = ConfigurationManager.AppSettings["Bing:APIKey"];
            var baseUrl = ConfigurationManager.AppSettings["Bing:BaseUrl"];
           
            var client = new RestClient(baseUrl);

            #region LAT-LONG

          
            if (address.Contains("#") )
            {
                address = address.Replace("#", "");
            }
            if ( address.Contains("&"))
            {
                address = address.Replace("&", "");
            }
            var request = new RestRequest("REST/v1/Locations?q=" + address + "&key=" + apiKey, Method.GET)
            {
                RequestFormat = DataFormat.Json
            };
            var response = client.Execute<BingAddressDetails>(request);

            if (response.Data.statusCode == 200)
            {
                var location = response.Data.resourceSets.FirstOrDefault();
                if (location != null)
                {
                    var resorce = location.resources.FirstOrDefault();
                    if (resorce != null)
                    {
                        var latLong = new LatitudeLongitude
                        {
                            lat = resorce.point.coordinates[0],
                            lng = resorce.point.coordinates[1],
                        };
                        objCor.Coordinate = new double[] { latLong.lat, latLong.lng };
                    }
                    else
                    {
                        var latLong = new LatitudeLongitude
                        {
                            lat = 0,
                            lng = 0,
                        };
                        objCor.Coordinate = new double[] { latLong.lat, latLong.lng };
                    }
                }
            }
            #endregion

            //List<NearByLocation> objLstLoc = new List<NearByLocation>();
            // var EntityCodeList = ConfigurationManager.AppSettings["Bing:EntityCodeList"];

            //foreach (var type in EntityCodeList.Split(','))
            //{
            //    string EntityName = Enum.GetName(typeof(EntiyiType), Convert.ToInt32(type));
            //    objLstLoc = getLocationByType(address, Convert.ToInt32(type));
            //    if (objLstLoc != null)
            //    {
            //        InsertEntityType(address, EntityName, objLstLoc);
            //    }
            //}

            return objCor;
        }

        public List<NearByLocation> getLocationByType(string address, int EntityTypeId)
        {
            List<NearByLocation> objLstLoc = new List<NearByLocation>();
            Coordinates objCor = new Coordinates();
            var apiKey = ConfigurationManager.AppSettings["Bing:APIKey"];
            var TypeBaseUrl = ConfigurationManager.AppSettings["Bing:TypeBaseUrl"];
            var LocationDistance = ConfigurationManager.AppSettings["Bing:LocationDistance"];
            var NoOfItem = ConfigurationManager.AppSettings["Bing:NoOfItem"];

            var client = new RestClient(TypeBaseUrl);

            var request = new RestRequest("REST/v1/data/f22876ec257b474b82fe2ffcb8393150/NavteqNA/NavteqPOIs?spatialFilter=nearby('" + address + "'," + LocationDistance + ")&$filter=EntityTypeID in (" + EntityTypeId + ")&$select=*,__Distance&$format=json&$top=" + NoOfItem + "&key=" + apiKey, Method.GET)
            {
                RequestFormat = DataFormat.Json
            };
            var response = client.Execute<SpatialDataCollection>(request);

            if (response.StatusCode.ToString() == "OK")
            {
                foreach (var item in response.Data.d.results)
                {
                    objLstLoc.Add(item);
                }
            }

            return objLstLoc;
        }

        public bool InsertLatlong(string address, Coordinates fetchedCoordinates)//double[] coordinates), string entityName, List<NearByLocation> entities
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.Coordinates);

                var update = Builders<Coordinates>.Update
                    .Set(m => m.Address, address)
                    .Set(m => m.Coordinate, fetchedCoordinates.Coordinate);

                var results = GetCollection().UpdateOneAsync(m => m.Address == address, update, new UpdateOptions()
                {
                    IsUpsert = true
                }).Result;

                return results.ModifiedCount > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool InsertEntityType(string address, string entityName, List<NearByLocation> entities)//double[] coordinates)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.Coordinates);

                var update = Builders<Coordinates>.Update
                    .Set(m => m.Address, address)
                    .Set(entityName, entities);

                var results = GetCollection().UpdateOneAsync(m => m.Address == address, update, new UpdateOptions()
                {
                    IsUpsert = true
                }).Result;

                return results.ModifiedCount > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public double[] GetLatlong(string address)
        {
            base.CollectionName = Convert.ToString(DbCollections.Coordinates);
            double[] coordinate = null;
            using (var cursor = GetCollection().FindAsync<Coordinates>(m => m.Address == address).Result)
            {
                while (cursor.MoveNextAsync().Result)
                {
                    var firstOrDefault = cursor.Current.FirstOrDefault();
                    if (firstOrDefault != null)
                    {
                        coordinate = firstOrDefault.Coordinate;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return coordinate;
        }
    }
}
