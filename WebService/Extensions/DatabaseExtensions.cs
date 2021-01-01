using System;
using System.Linq;
using MongoDB.Driver;

namespace WebService
{
    public static class DatabaseExtensions
    {
        public static IMongoCollection<T> GetCollection<T>(this IMongoDatabase database)
        {
            return database.GetCollection<T>(typeof(T).GetClassName());
        }

        private static string GetClassName(this Type type)
        {
            return type.ToString().Split('.').Last();

        }
    }
}