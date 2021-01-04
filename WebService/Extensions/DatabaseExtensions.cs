using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public static async Task<IEnumerable<T>> FindWithFilterAsync<T>(this IMongoDatabase database, FilterDefinition<T> filter)
        {
            var cursor = await database.GetCollection<T>().FindAsync(filter);
            return await cursor.ToListAsync();
        }
    }
}