using System.Text.RegularExpressions;
using MongoDB.Driver;

namespace SimpleMongoRepo
{
    public static class DatabaseFactory
    {
        private static IMongoClient CreateClient(string connectionString)
        {
            return new MongoClient(connectionString);
        }

        public static IMongoDatabase Create(string connectionString)
        {
            var client = CreateClient(connectionString);
            var dbName = Regex.Match(connectionString, @"(\w*)\?").Groups[1].ToString();
            return client.GetDatabase(dbName);
        }
    }
}