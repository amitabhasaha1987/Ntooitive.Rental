using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace Repositories.Interfaces.ElasticSearch
{
    public interface IElasticSearchSettings
    {
        Lazy<ElasticClient> GetConnection();

        bool CreateSettings();
        bool IndexExists(string indexName);

    }

    public abstract class ElasticSearchConnection
    {
        protected ElasticClient BuildConnection()
        {
            string connectionstring = ConfigurationManager.AppSettings["ElasticSearch:Baseurl"];
            string defaultIndexName = ConfigurationManager.AppSettings["ElasticSearch:Default_Index_Name"];
            var node = new Uri(connectionstring == string.Empty ? "http://localhost:9200/" : connectionstring);

            var settings = new ConnectionSettings(
                node,
                defaultIndex: defaultIndexName
            );

            return new ElasticClient(settings);

        }

    }
}
