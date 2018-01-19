using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SimpleMongoRepo.Interfaces;
using System.Reflection;

namespace SimpleMongoRepo
{
    public class Repository<T> : IRepository<T> where T : IMongoEntity
    {
        private readonly IMongoCollection<T> _collection;

        public Repository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task AsyncInsert(T entity)
        {
            if (string.IsNullOrEmpty(entity.Id)) entity.Id = ObjectId.GenerateNewId().ToString();
            await _collection.InsertOneAsync(entity);
        }

        public void Insert(T entity)
        {
            if (string.IsNullOrEmpty(entity.Id)) entity.Id = ObjectId.GenerateNewId().ToString();
            _collection.InsertOne(entity);
        }

        public async Task<IEnumerable<T>> AsyncGetAll()
        {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public IEnumerable<T> GetAll()
        {
            return _collection.Find(new BsonDocument()).ToList();
        }

        public List<string> GetAllValuesByPropertyName(string propertyName, List<T> optionalList = null)
        {
            var list = optionalList ?? GetAll().ToList();
            return (
                from allResult in list
                let propertyInfo = allResult.GetType().GetProperty(propertyName)
                select propertyInfo?.GetValue(allResult, null)
                into readPropValue
                where readPropValue != null
                select readPropValue.ToString()
                ).ToList();
        }

        public async Task<IEnumerable<T>> AsyncGetByField(string fieldName, string fieldValue)
        {
            var filter = Builders<T>.Filter.Eq(fieldName, fieldValue);
            return (_collection.Count(filter) > 0) ? await _collection.Find(filter).ToListAsync() : new List<T>();
        }

        public IFindFluent<T, T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return (_collection.Count(predicate) > 0) ? _collection.Find(predicate) : null;
        }

        public T First(Expression<Func<T, bool>> predicate)
        {
            return (_collection.Count(predicate) > 0) ? _collection.Find(predicate).First() : default(T);
        }

        public async Task<T> AsyncFirst(Expression<Func<T, bool>> predicate)
        {
            return (_collection.Count(predicate) > 0) ? await _collection.Find(predicate).FirstAsync() : default(T);
        }

        public async Task<T> AsyncGetById(string id)
        {
            return (_collection.Count(c => c.Id == id) > 0) ? await AsyncFirst(c => c.Id == id) : default(T);
        }

        public T GetById(string id)
        {
            return (_collection.Count(c => c.Id == id) > 0) ? First(c => c.Id == id) : default(T);
        }

        public List<T> GetListBy(Expression<Func<T, bool>> predicate)
        {
            return (_collection.Count(predicate) > 0) ? _collection.Find(predicate).ToList() : new List<T>();
        }

        public async Task<IEnumerable<T>> AsyncGet(int startingFrom, int count)
        {
            var result = await _collection.Find(new BsonDocument())
                                               .Skip(startingFrom)
                                               .Limit(count)
                                               .ToListAsync();

            return result;
        }

        public async Task<bool> AsyncUpdateField(string id, string udateFieldName, string updateFieldValue)
        {
            var filter = Builders<T>.Filter.Eq(t => t.Id, id);
            var update = Builders<T>.Update.Set(udateFieldName, updateFieldValue);
            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount != 0;
        }

        public bool UpdateField(string id, string udateFieldName, string updateFieldValue)
        {
            var filter = Builders<T>.Filter.Eq(t => t.Id, id);
            var update = Builders<T>.Update.Set(udateFieldName, updateFieldValue);
            var result = _collection.UpdateOne(filter, update);
            return result.ModifiedCount != 0;
        }

        public bool Update(T entity)
        {
            var filter = Builders<T>.Filter.Eq(t => t.Id, entity.Id);
            var result = _collection.ReplaceOne(filter, entity);
            return result.ModifiedCount != 0;
        }

        public async Task<bool> AsyncDeleteById(string id)
        {
            var filter = Builders<T>.Filter.Eq(t => t.Id, id);
            return await PerformAsyncDelete(filter);
        }

        public async Task<bool> AsyncDeleteBy(Expression<Func<T, bool>> predicate)
        {
            var filter = Builders<T>.Filter.Where(predicate);
            return await PerformAsyncDelete(filter);
        }

        private async Task<bool> PerformAsyncDelete(FilterDefinition<T> filter)
        {
            var result = (_collection.Count(filter) > 0) ? await _collection.DeleteOneAsync(filter) : null;
            if (result == null) return false;
            return result.DeletedCount != 0;
        }

        public bool DeleteById(string id)
        {
            var filter = Builders<T>.Filter.Eq(t => t.Id, id);
            return PerformDelete(filter);
        }

        public bool DeleteBy(Expression<Func<T, bool>> predicate)
        {
            var filter = Builders<T>.Filter.Where(predicate);
            return PerformDelete(filter);
        }

        private bool PerformDelete(FilterDefinition<T> filter)
        {
            var result = (_collection.Count(filter) > 0) ? _collection.DeleteOne(filter) : null;
            if (result == null) return false;
            return result.DeletedCount != 0;
        }

        public async Task<long> AsyncDeleteAll()
        {
            var filter = new BsonDocument();
            var result = await _collection.DeleteManyAsync(filter);
            return result.DeletedCount;
        }

        public long CountBy(Expression<Func<T, bool>> predicate)
        {
            var filter = Builders<T>.Filter.Where(predicate);
            return _collection.Count(filter);
        }

        public int SumBy(ProjectionDefinition<T, BsonDocument> groupDefinition)
        {
            var result = _collection.Aggregate().Group(groupDefinition).Group(new BsonDocument
            {
                {"_id", "_id"},
                {"count", new BsonDocument("$sum", 1)}
            }).First();

            return (int)result["count"];
        }

        public async Task<long> AsyncCountBy(Expression<Func<T, bool>> predicate)
        {
            var filter = Builders<T>.Filter.Where(predicate);
            return await _collection.CountAsync(filter);
        }

        public async Task CreateIndexOnNameField(FieldDefinition<T> definition)
        {
            var keys = Builders<T>.IndexKeys.Ascending(definition);
            await _collection.Indexes.CreateOneAsync(keys);
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return _collection.Count(predicate) > 0;
        }
        public async Task CreateIndexOnCollection(IMongoCollection<BsonDocument> collection, string field)
        {
            var keys = Builders<BsonDocument>.IndexKeys.Ascending(field);
            await collection.Indexes.CreateOneAsync(keys);
        }
    }
}