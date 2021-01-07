using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebService
{
    public class AdminService : IAdminService
    {
        private readonly ILogger<AdminService> _logger;
        private ILedgerRepository _ledgerRepo;
        private IUserRepository _userRepo;

        public AdminService(ILogger<AdminService> logger, ILedgerRepository ledgerRepo, IUserRepository userRepo)
        {
            _logger = logger;
            _ledgerRepo = ledgerRepo;
            _userRepo = userRepo;
        }

        public async Task<UserRole> AddUserRoleAsync(UserRoleRequest request, string userId)
        {
            // Validator ensures non-duplicate role.
            return await _userRepo.InsertUserRoleAsync(new UserRole()
            {
                Description = request.Description,
                CreatedBy = userId,
                CreatedDate = DateTime.Now
            });
        }

        public async Task UpdateUserRoleAsync(UpdateUserRoleRequest request)
        {
            // Validator ensures that a user exists with the provided username AND
            // that the role already exists.
            var userId = (await _userRepo.GetUsersByUsernameAsync(request.Username)).First().Id;
            await _userRepo.UpdateUserRoleAsync(userId, request.Role);
        }

        public async Task<Frequency> AddFrequencyAsync(FrequencyRequest request, string userId)
        {
            // Validator ensures non-duplicate frequency.
            return await _ledgerRepo.InsertOneAsync(new Frequency()
            {
                Description = request.Description,
                ApproxTimesPerYear = request.ApproxTimesPerYear,
                CreatedBy = userId,
                CreatedDate = DateTime.Now
            });
        }

        public async Task<SalaryType> AddSalaryTypeAsync(SalaryTypeRequest request, string userId)
        {
            // Validator ensures non-duplicate type.
            return await _ledgerRepo.InsertOneAsync(new SalaryType()
            {
                Description = request.Description,
                CreatedBy = userId,
                CreatedDate = DateTime.Now
            });
        }

        public async Task<TransactionType> AddTransactionTypeAsync(TransactionTypeRequest request, string userId)
        {
            // Validator ensures non-duplicate type.
            return await _ledgerRepo.InsertOneAsync(new TransactionType()
            {
                Description = request.Description,
                CreatedBy = userId,
                CreatedDate = DateTime.Now
            });
        }
    }
}