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
        Task InsertUserAsync(User user);
        Task UpdateUserRoleAsync(string userId, string role);
        Task UpdateUserLastLoggedInAsync(string userId, DateTime lastLoggedIn);
        Task<long> UpdateUsernameAsync(string userId, string username);
        Task<IEnumerable<RefreshData>> GetRefreshDataByUserIdAsync(string userId);
        Task InsertRefreshDataAsync(RefreshData token);
        Task DeleteRefreshDataByIdAsync(string id);
        Task<IEnumerable<UserRole>> GetUserRolesAsync();
        Task InsertUserRoleAsync(UserRole role);
        Task<bool> SupportTicketExistsWithIdAsync(string id);
        Task<IEnumerable<SupportTicket>> GetSupportTicketsAsync();
        Task<IEnumerable<SupportTicket>> GetSupportTicketsSubmittedByUser(string userId);
        Task<IEnumerable<SupportTicket>> GetSupportTicketsByIdAsync(string id);
        Task InsertSupportTicketAsync(SupportTicket ticket);
        Task AddMessageToSupportTicketAsync(string id, Message messages);
        Task UpdateSupportTicketResolvedAsync(string id, bool resolved);
    }
}