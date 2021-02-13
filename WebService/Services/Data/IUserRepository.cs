using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebService
{
    public interface IUserRepository
    {
        Task<long> GetUserCountAsync();
        Task<bool> UserExistsWithIdAsync(string userId);
        Task<IEnumerable<User>> GetUsersByIdAsync(string userId);
        Task<IEnumerable<User>> GetUsersByUsernameAsync(string username);
        Task<User> InsertUserAsync(User user);
        Task UpdateUserRoleAsync(string userId, string role);
        Task UpdateUserLastLoggedInAsync(string userId, DateTime lastLoggedIn);
        Task<long> UpdateUsernameAsync(string userId, string username);
        Task<IEnumerable<RefreshData>> GetRefreshDataByUserIdAsync(string userId);
        Task<RefreshData> InsertRefreshDataAsync(RefreshData token);
        Task DeleteRefreshDataByIdAsync(string id);
        Task<IEnumerable<UserRole>> GetUserRolesAsync();
        Task<UserRole> InsertUserRoleAsync(UserRole role);
        Task<bool> SupportTicketExistsWithIdAsync(string id);
        Task<IEnumerable<SupportTicket>> GetSupportTicketsAsync();
        Task<SupportTicket> InsertSupportTicketAsync(SupportTicket ticket);
        Task<IEnumerable<Message>> GetMessagesAsync(string userId);
        Task<Message> InsertMessageAsync(Message message);
    }
}