using MongoDB.Driver;
using SimpleMongoRepo.Interfaces;

namespace SimpleMongoRepo
{
    public static class RepositoryFactory<T> where T : IMongoEntity
    {
        public static Repository<T> GetRepository(IMongoDatabase database, string collection)
        {
            return new Repository<T>(database, collection);
        }
    }
}