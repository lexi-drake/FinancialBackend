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

        public async Task<long> GetUserCountAsync() =>
            await _db.GetCollection<User>().CountDocumentsAsync(Builders<User>.Filter.Eq(x => x.Role, "User"));

        public async Task<bool> UserExistsWithIdAsync(string userId) =>
             await _db.GetCollection<User>().CountDocumentsAsync(Builders<User>.Filter.Eq(x => x.Id, userId)) > 0;

        public async Task<IEnumerable<User>> GetUsersByIdAsync(string userId) =>
            await _db.FindWithFilterAsync(Builders<User>.Filter.Eq(x => x.Id, userId));


        public async Task<IEnumerable<User>> GetUsersByUsernameAsync(string username) =>
            await _db.FindWithFilterAsync(Builders<User>.Filter.Eq(x => x.Username, username));

        public async Task InsertUserAsync(User user) =>
            await _db.GetCollection<User>().InsertOneAsync(user);


        public async Task UpdateUserRoleAsync(string userId, string role) =>
            await _db.GetCollection<User>().UpdateOneAsync(
                Builders<User>.Filter.Eq(x => x.Id, userId),
                Builders<User>.Update.Set(x => x.Role, role));

        public async Task UpdateUserLastLoggedInAsync(string userId, DateTime lastLoggedIn) =>
            await _db.GetCollection<User>().UpdateOneAsync(
                Builders<User>.Filter.Eq(x => x.Id, userId),
                Builders<User>.Update.Set(x => x.LastLoggedIn, lastLoggedIn));

        public async Task<long> UpdateUsernameAsync(string userId, string username)
        {
            var updated = await _db.GetCollection<User>().UpdateOneAsync(
                Builders<User>.Filter.Eq(x => x.Id, userId),
                Builders<User>.Update.Set(x => x.Username, username));
            return updated.ModifiedCount;
        }

        public async Task<IEnumerable<RefreshData>> GetRefreshDataByUserIdAsync(string userId) =>
            await _db.FindWithFilterAsync(Builders<RefreshData>.Filter.Eq(x => x.UserId, userId));

        public async Task InsertRefreshDataAsync(RefreshData data) =>
            await _db.GetCollection<RefreshData>().InsertOneAsync(data);


        public async Task DeleteRefreshDataByIdAsync(string id)
        {
            var filter = Builders<RefreshData>.Filter.Eq(x => x.Id, id);
            await _db.GetCollection<RefreshData>().DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<UserRole>> GetUserRolesAsync() =>
            await _db.FindWithFilterAsync(FilterDefinition<UserRole>.Empty);

        public async Task InsertUserRoleAsync(UserRole role) =>
            await _db.GetCollection<UserRole>().InsertOneAsync(role);

        public async Task<bool> SupportTicketExistsWithIdAsync(string id) =>
            await _db.GetCollection<SupportTicket>().CountDocumentsAsync(Builders<SupportTicket>.Filter.Eq(x => x.Id, id)) > 0;

        public async Task<IEnumerable<SupportTicket>> GetSupportTicketsAsync() =>
            await _db.FindWithFilterAsync(FilterDefinition<SupportTicket>.Empty);

        public async Task<IEnumerable<SupportTicket>> GetSupportTicketsSubmittedByUser(string userId) =>
            await _db.FindWithFilterAsync(Builders<SupportTicket>.Filter.Eq(x => x.SubmittedById, userId));

        public async Task<IEnumerable<SupportTicket>> GetSupportTicketsByIdAsync(string id) =>
            await _db.FindWithFilterAsync(Builders<SupportTicket>.Filter.Eq(x => x.Id, id));

        public async Task InsertSupportTicketAsync(SupportTicket ticket) =>
            await _db.GetCollection<SupportTicket>().InsertOneAsync(ticket);

        public async Task AddMessageToSupportTicketAsync(string id, Message message) =>
            await _db.GetCollection<SupportTicket>().UpdateOneAsync(
                Builders<SupportTicket>.Filter.Eq(x => x.Id, id),
                Builders<SupportTicket>.Update.Push(x => x.Messages, message));

        public async Task UpdateSupportTicketResolvedAsync(string id, bool resolved) =>
            await _db.GetCollection<SupportTicket>().UpdateOneAsync(
                Builders<SupportTicket>.Filter.Eq(x => x.Id, id),
                Builders<SupportTicket>.Update.Set(x => x.Resolved, resolved));
    }
}