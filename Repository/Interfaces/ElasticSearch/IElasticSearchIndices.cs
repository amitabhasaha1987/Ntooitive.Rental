using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Repositories.Models.ListHub;
using Repositories.Models.Common;

namespace Repositories.Interfaces.ElasticSearch
{
    public interface IElasticSearchIndices<T> where T : class
    {
        T Get(string Id);

        bool RemoveIndex(string mlsid);

        bool CreateIndex(T entity);
        void CreateBulkIndex(List<T> entites);
        bool UpdateIndex(T entity);
        dynamic SearchQuery(AdvanceSearch advSearch, int startIndex, int limit, out int totalcount, bool IsOpenHouse = false);
        dynamic SearchFeaturedQuery(AdvanceSearch advSearch, int startIndex, int limit, out int totalcount, bool IsOpenHouse = false);
        dynamic GetMlsNumber(AdvanceSearch advSearch, int index);
 List<T> GetAll();

    }
}
