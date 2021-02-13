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

        public async Task<long> GetUserCountAsync()
        {
            var filter = Builders<User>.Filter.Eq(x => x.Role, "User");
            return await _db.GetCollection<User>().CountDocumentsAsync(filter);
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

        public async Task UpdateUserLastLoggedInAsync(string userId, DateTime lastLoggedIn)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.LastLoggedIn, lastLoggedIn);

            await _db.GetCollection<User>().UpdateOneAsync(filter, update);
        }

        public async Task<long> UpdateUsernameAsync(string userId, string username)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update.Set(x => x.Username, username);

            var updated = await _db.GetCollection<User>().UpdateOneAsync(filter, update);
            return updated.ModifiedCount;
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

        public async Task<IEnumerable<SupportTicket>> GetSupportTicketsAsync()
        {
            return await _db.FindWithFilterAsync(FilterDefinition<SupportTicket>.Empty);
        }

        public async Task<SupportTicket> InsertSupportTicketAsync(SupportTicket ticket)
        {
            await _db.GetCollection<SupportTicket>().InsertOneAsync(ticket);
            return ticket;
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(string userId)
        {
            var filter = Builders<Message>.Filter.Eq(x => x.RecipientId, userId);
            return await _db.FindWithFilterAsync(filter);
        }

        public async Task<Message> InsertMessageAsync(Message message)
        {
            await _db.GetCollection<Message>().InsertOneAsync(message);
            return message;
        }
    }
}