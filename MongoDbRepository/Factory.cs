using System;
using System.Configuration;
using Core.Conventions;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Core
{
    public static class Factory
    {
        private static IMongoDatabase _mongoDbSingleton;

        public static IMongoDatabase CreateMongoDatabase()
        {
          
            if (_mongoDbSingleton == null)
            {

                //Credential For MongoServer
                var credential = MongoCredential.CreateCredential(ConfigurationManager.AppSettings["MongoDatabaseName"], ConfigurationManager.AppSettings["MongoUserName"], ConfigurationManager.AppSettings["MongoPassword"]);
                MongoClientSettings settings;

                //Settings For MongoServer
                if (credential == null)
                {
                    settings = new MongoClientSettings
                    {
                        Server =
                            new MongoServerAddress(ConfigurationManager.AppSettings["MongoServerIP"],
                                Convert.ToInt32(ConfigurationManager.AppSettings["MongoServerPort"])),
                        MaxConnectionPoolSize =
                            Convert.ToInt32(ConfigurationManager.AppSettings["MaxConnectionPoolSize"]),
                        ConnectTimeout =
                            new TimeSpan(0, 0, 0, 0,
                                Convert.ToInt32(ConfigurationManager.AppSettings["ConnectionTimeOutInMiliSecond"])),
                        SocketTimeout =
                            new TimeSpan(0, 0, 0, 0,
                                Convert.ToInt32(ConfigurationManager.AppSettings["ConnectionTimeOutInMiliSecond"])),
                    };
                }
                else
                {
                    settings = new MongoClientSettings
                    {
                        Credentials = new[] { credential },

                        Server =
                            new MongoServerAddress(ConfigurationManager.AppSettings["MongoServerIP"],
                                Convert.ToInt32(ConfigurationManager.AppSettings["MongoServerPort"])),
                        MaxConnectionPoolSize =
                            Convert.ToInt32(ConfigurationManager.AppSettings["MaxConnectionPoolSize"]),
                        ConnectTimeout =
                            new TimeSpan(0, 0, 0, 0,
                                Convert.ToInt32(ConfigurationManager.AppSettings["ConnectionTimeOutInMiliSecond"])),
                        SocketTimeout =
                            new TimeSpan(0, 0, 0, 0,
                                Convert.ToInt32(ConfigurationManager.AppSettings["ConnectionTimeOutInMiliSecond"])),
                    };
                }



                var client = new MongoClient(settings);

                _mongoDbSingleton = client.GetDatabase(ConfigurationManager.AppSettings["MongoDatabaseName"]);

                // Set db conventions
               var conventions = new DbConventions();
               ConventionRegistry.Register("DbConventions", conventions, (type) => true);     
            }

            return _mongoDbSingleton;
        }
    }
}
