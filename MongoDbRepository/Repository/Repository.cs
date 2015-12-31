using System;
using System.Collections.Generic;
using MongoDB.Driver;
using Repositories.Interfaces.Base;
using Repositories.Models.Base;

namespace Core.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : BaseEntity, new()
    {
        protected IMongoDatabase Database;
        public string CollectionName;
       

        public Repository(string collectionName)
        {
            this.Database = Factory.CreateMongoDatabase();
            this.CollectionName = collectionName;
        }

        public Repository(IMongoDatabase database, string collectionName)
        {
            this.Database = database;
            this.CollectionName = collectionName;
        }

        public Repository(string connectionString, string databaseName, string collectionName)
        {
            // Setup the database
            var client = new MongoClient(connectionString);
            //var server = client.GetServer();

            this.Database = client.GetDatabase(databaseName);
            this.CollectionName = collectionName;
        }

        #region ADDED BY INDUSNET TECHNOLOGIES

        /// <summary>
        /// For creating new database instance for current object, collection name can be provide at runtime
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        public Repository(string connectionString, string databaseName)
        {
            // Setup the database
            var client = new MongoClient(connectionString);

            this.Database = client.GetDatabase(databaseName);
        }

        /// <summary>
        /// Set a new database instance at runtime for current object
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        public void SetDatabaseInstance(string connectionString, string databaseName, string collectionName)
        {
            // Setup the database
            var client = new MongoClient(connectionString);
            this.Database = client.GetDatabase(databaseName);
            this.CollectionName = collectionName;
        }

        #endregion
        protected virtual IMongoCollection<T> GetCollection()
        {
            return Database.GetCollection<T>(CollectionName);
        }

        public virtual T Create(T entity)
        {
            GetCollection().InsertOneAsync(entity).Wait();
            return entity;
        }

        public virtual T CreateSpecificId(T entity)
        {
            //Todo
            throw new NotImplementedException();
        }

        public virtual bool Update(T entity)
        {
            //Todo
            throw new NotImplementedException();
        }

        public virtual bool Delete(string id)
        {
            //Todo
            throw new NotImplementedException();
        }

        public virtual bool Exists(string id)
        {
            //Todo
            throw new NotImplementedException();
        }

        public virtual T GetById(string id)
        {
            //Todo
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            //Todo
            throw new NotImplementedException();
        }

        public bool UpdatedOn(T entity, DateTime date)
        {
            //Todo
            throw new NotImplementedException();
        }

        public bool InsertBulk(IEnumerable<T> entities)
        {
            try
            {
                GetCollection().InsertManyAsync(entities).Wait();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public virtual void Dispose() { }


    }
}
