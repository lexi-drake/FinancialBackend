using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebService
{
    public interface IAdminService
    {
        Task AddUserRoleAsync(UserRoleRequest request, string userId);
        Task UpdateUserRoleAsync(UpdateUserRoleRequest request, string userId);
        Task AddFrequencyAsync(FrequencyRequest request, string userId);
        Task AddSalaryTypeAsync(SalaryTypeRequest request, string userId);
        Task AddTransactionTypeAsync(TransactionTypeRequest request, string userId);
        Task<IEnumerable<SupportTicketResponse>> GetSupportTicketsAsync();
        Task AddMessageAsync(MessageRequest request, string userId);
    }
}