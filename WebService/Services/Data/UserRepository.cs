using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace WebService
{
    public class UserRepository : IUserRepository
    {
        private IMongoDatabase _db;

        public UserRepository(string connectionString, string database)
        {
            _db = new MongoClient(connectionString).GetDatabase(database);
        }

        public async Task<IEnumerable<User>> GetUsersByIdAsync(string userId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            return await GetUsersWithFilter(filter);
        }

        public async Task<IEnumerable<User>> GetUsersByUsernameAsync(string username)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Username, username);
            return await GetUsersWithFilter(filter);
        }

        private async Task<IEnumerable<User>> GetUsersWithFilter(FilterDefinition<User> filter)
        {
            var cursor = await _db.GetCollection<User>().FindAsync<User>(filter);
            return await cursor.ToListAsync();
        }

        public async Task<User> InsertUserAsync(User user)
        {
            await _db.GetCollection<User>().InsertOneAsync(user);
            return user;
        }

        public async Task<IEnumerable<RefreshData>> GetRefreshDataByUserIdAsync(string userId)
        {
            var filter = Builders<RefreshData>.Filter.Eq(x => x.UserId, userId);

            var cursor = await _db.GetCollection<RefreshData>().FindAsync<RefreshData>(filter);
            return await cursor.ToListAsync();
        }

        public async Task<RefreshData> InsertRefreshDataAsync(RefreshData data)
        {
            await _db.GetCollection<RefreshData>().InsertOneAsync(data);
            return data;
        }

        public async Task DeleteRefreshDataByIdAsync(string id)
        {
            var filter = Builders<RefreshData>.Filter.Eq(x => x.Id, id);
            await _db.GetCollection<RefreshData>().DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<UserRole>> GetUserRolesAsync()
        {
            var filter = FilterDefinition<UserRole>.Empty;

            var cursor = await _db.GetCollection<UserRole>().FindAsync<UserRole>(filter);
            return await cursor.ToListAsync();
        }

        public async Task<UserRole> InsertUserRoleAsync(UserRole role)
        {
            await _db.GetCollection<UserRole>().InsertOneAsync(role);
            return role;
        }
    }
}