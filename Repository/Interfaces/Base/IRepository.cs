using System;
using System.Collections.Generic;

namespace Repositories.Interfaces.Base
{
    public interface IRepository<T> : IDisposable where T : class
    {
        T Create(T entity);

        T CreateSpecificId(T entity);

        bool Update(T entity);

        bool Delete(string id);

        bool Exists(string id);

        T GetById(string id);

        IEnumerable<T> GetAll();

        bool UpdatedOn(T account, DateTime date);

        bool InsertBulk(IEnumerable<T> entities);
    }
}
