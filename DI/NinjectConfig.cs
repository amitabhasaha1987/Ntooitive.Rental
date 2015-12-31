using System;
using Core;
using Core.Implementation.Admin.ListHub;
using Core.Implementation.Admin.NewHome;
using Core.Implementation.Admin.Users;
using Core.Implementation.Community;
using Core.Implementation.Downloader;
using Core.Implementation.ElasticSearch;
using Core.Implementation.ListHub;
using Core.Implementation.Mail;
using Core.Implementation.Map;
using Core.Implementation.NewHome;
using Core.Implementation.UserContact;
using MongoDB.Driver;
using Ninject;
using Ninject.Activation;
using Ninject.Web.Common;
using Repositories.Interfaces.Admin.ListHub;
using Repositories.Interfaces.Admin.NewHome;
using Repositories.Interfaces.Admin.Users;
using Repositories.Interfaces.Community;
using Repositories.Interfaces.Downloader;
using Repositories.Interfaces.ElasticSearch;
using Repositories.Interfaces.ListHub;
using Repositories.Interfaces.Mail;
using Repositories.Interfaces.Map;
using Repositories.Interfaces.NewHome;
using Repositories.Interfaces.UserContact;
using Repositories.Models.ListHub;
using Repositories.Interfaces.Admin.Office;
using Core.Implementation.Admin.Office;

namespace Configuration
{
    public static class NinjectConfig
    {
        private static IKernel _kernel;
        /// <summary>
        /// To load specific module per need
        /// </summary>
        /// <typeparam name="T">Interface to get implementaion</typeparam>
        /// <returns>new object of implemented interface</returns>
        public static T Get<T>()
        {
            try
            {
                if (_kernel == null)
                {
                    _kernel = CreateKernel();
                }
                return _kernel.Get<T>();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Some exception occured while trying to get the implementation. Please check the error message : " + ex.Message);
                return default(T);
            }
        }
        /// <summary>
        /// To load all modules for schedulers
        /// </summary>
        public static void StartScheduler()
        {
            CreateKernel();
        }
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);

            RegisterServices(kernel);

            return kernel;
        }
        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IMongoDatabase>().ToMethod(BuildMongoDatabase).InSingletonScope();
            kernel.Bind<ICommunityProvider>().To<CommunityProcessor>();

            kernel.Bind<IFetchLatLong>().To<GetLatLongFromBing>();
            kernel.Bind<IListHub>().To<ListHubFeed>();
            kernel.Bind<INewHome>().To<NewHomeFeed>();
            kernel.Bind<IMailBase>().To<MailgunBase>();
            kernel.Bind<IUserContactDetails>().To<UserContactDetails>();
            kernel.Bind<IProperties>().To<PropertyHandler>();
            kernel.Bind<IAgent>().To<UserHandler>();
            kernel.Bind<IOffice>().To<OfficeHandler>();
            kernel.Bind<INewHomes>().To<NewHomePropertyHandler>();

            kernel.Bind<IElasticSearchSettings>().To<ElasticSearchSettings>();
            kernel.Bind<IElasticSearchIndices<Purchase>>().To<ElasticSearchPurchaseIndices>();

            kernel.Bind<IElasticSearchIndices<Rental>>().To<ElasticSearchRentalIndices>();
            kernel.Bind<IElasticSearchIndices<NewHome>>().To<ElasticSearchNewHomeIndices>();

            kernel.Bind<IProcesser>().To<Processer>();
            kernel.Bind<IDownloader>().To<FileDownloader>();
            kernel.Bind<IFetcher>().To<ListHubDataFetch>();
            kernel.Bind<INewHomeDownloader>().To<NewHomeDateFetch>();

        }
        /// <summary>
        /// Create Singletone DB connetion
        /// </summary>
        /// <param name="arg">Registerd Context of the application</param>
        /// <returns>Returns Database Object</returns>
        private static IMongoDatabase BuildMongoDatabase(IContext arg)
        {
            return Factory.CreateMongoDatabase();
        }

    }
}
