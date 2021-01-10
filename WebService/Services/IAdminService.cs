using System.Threading.Tasks;

namespace WebService
{
    public interface IAdminService
    {
        Task<UserRole> AddUserRoleAsync(UserRoleRequest request, string userId);
        Task UpdateUserRoleAsync(UpdateUserRoleRequest request, string userId);
        Task<Frequency> AddFrequencyAsync(FrequencyRequest request, string userId);
        Task<SalaryType> AddSalaryTypeAsync(SalaryTypeRequest request, string userId);
        Task<TransactionType> AddTransactionTypeAsync(TransactionTypeRequest request, string userId);
    }
}