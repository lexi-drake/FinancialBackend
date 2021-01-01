using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebService
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(string connectionString, string database)
        {

        }

        public async Task<IEnumerable<User>> GetUsersByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetUsersByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<User> InsertUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RefreshData>> GetRefreshDataByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<RefreshData> InsertRefreshDataAsync(RefreshData data)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteRefreshDataByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserRole>> GetUserRolesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserRole>> GetUserRolesByIdAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<UserRole> InsertUserRoleAsync(UserRole role)
        {
            throw new NotImplementedException();
        }
    }
}