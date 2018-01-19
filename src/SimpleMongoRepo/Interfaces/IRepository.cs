using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SimpleMongoRepo.Interfaces
{
    public interface IRepository<T> where T : IMongoEntity
    {
        Task AsyncInsert(T entity);
        void Insert(T entity);
        Task<IEnumerable<T>> AsyncGetAll();
        IEnumerable<T> GetAll();
        List<string> GetAllValuesByPropertyName(string propertyName, List<T> optionalList = null);
        Task<IEnumerable<T>> AsyncGetByField(string fieldName, string fieldValue);
        IFindFluent<T, T> FindBy(Expression<Func<T, bool>> predicate);
        T First(Expression<Func<T, bool>> predicate);
        Task<T> AsyncFirst(Expression<Func<T, bool>> predicate);
        List<T> GetListBy(Expression<Func<T, bool>> predicate);
        T GetById(string id);
        Task<T> AsyncGetById(string id);
        Task<IEnumerable<T>> AsyncGet(int startingFrom, int count);
        Task<bool> AsyncUpdateField(string id, string udateFieldName, string updateFieldValue);
        bool UpdateField(string id, string udateFieldName, string updateFieldValue);
        bool Update(T entity);
        Task<bool> AsyncDeleteById(string id);
        Task<bool> AsyncDeleteBy(Expression<Func<T, bool>> predicate);
        bool DeleteById(string id);
        bool DeleteBy(Expression<Func<T, bool>> predicate);
        long CountBy(Expression<Func<T, bool>> predicate);
        Task<long> AsyncCountBy(Expression<Func<T, bool>> predicate);
        int SumBy(ProjectionDefinition<T, BsonDocument> groupDefinition);
        Task<long> AsyncDeleteAll();
        Task CreateIndexOnNameField(FieldDefinition<T> definition);
        bool Any(Expression<Func<T, bool>> predicate);
    }
}