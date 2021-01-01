using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebService
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersByIdAsync(string userId);
        Task<IEnumerable<User>> GetUsersByUsernameAsync(string username);
        Task<User> InsertUserAsync(User user);
        Task<IEnumerable<RefreshData>> GetRefreshDataByUserIdAsync(string userId);
        Task<RefreshData> InsertRefreshDataAsync(RefreshData token);
        Task DeleteRefreshDataByIdAsync(string id);
        Task<IEnumerable<UserRole>> GetUserRolesAsync();
        Task<UserRole> InsertUserRoleAsync(UserRole role);
    }
}