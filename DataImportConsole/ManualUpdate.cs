using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using AutoMapper;
using Configuration;
using DataImportConsole.PropertySetter;
using Nest;
using Repositories.Interfaces.Admin.Users;
using Repositories.Interfaces.Community;
using Repositories.Interfaces.Downloader;
using Repositories.Interfaces.ElasticSearch;
using Repositories.Interfaces.ListHub;
using Repositories.Interfaces.NewHome;
using Repositories.Models.Admin.User;
using Repositories.Models.ListHub;
using Repositories.Models.NewHome;
//using Configuration = System.Configuration.Configuration;
using Repositories.Interfaces.Admin.Office;
using Repositories.Models.Classified;

namespace DataImportConsole
{
    class ManualUpdate
    {

         void Main(string[] args)
        {
            RunProcess();
            //SerachProcess();
        }

        private static void RunProcess()
        {
            if (!Initialize())
            {
                Console.WriteLine("Error!!! Unable to initialize.");
                Console.ReadKey(true);
                return;
            }

            var elasticSearchindicesforRental = NinjectConfig.Get<IElasticSearchIndices<Rental>>();
            var elasticSearchindicesforPurchase = NinjectConfig.Get<IElasticSearchIndices<Purchase>>();

            //24699
           var allResult =  elasticSearchindicesforPurchase.GetAll();

            foreach (var result in allResult)
            {
               // result.
            }
        }

        private static bool Initialize()
        {
            NinjectConfig.StartScheduler();
            AutoMapperConfiguration.Configure();
            return true;
        }

    }
}
