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
            return await _db.FindWithFilterAsync(filter);
        }

        public async Task<IEnumerable<User>> GetUsersByUsernameAsync(string username)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Username, username);
            return await _db.FindWithFilterAsync(filter);
        }

        public async Task<User> InsertUserAsync(User user)
        {
            await _db.GetCollection<User>().InsertOneAsync(user);
            return user;
        }

        public async Task UpdateUserRoleAsync(string userId, string role)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.Role, role);

            await _db.GetCollection<User>().UpdateOneAsync(filter, update);
        }

        public async Task<IEnumerable<RefreshData>> GetRefreshDataByUserIdAsync(string userId)
        {
            var filter = Builders<RefreshData>.Filter.Eq(x => x.UserId, userId);
            return await _db.FindWithFilterAsync(filter);
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
            return await _db.FindWithFilterAsync(FilterDefinition<UserRole>.Empty);
        }

        public async Task<UserRole> InsertUserRoleAsync(UserRole role)
        {
            await _db.GetCollection<UserRole>().InsertOneAsync(role);
            return role;
        }
    }
}