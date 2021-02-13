using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;

namespace WebService
{
    public class AdminService : IAdminService
    {
        private readonly ILogger _logger;
        private ILedgerRepository _ledgerRepo;
        private IUserRepository _userRepo;

        public AdminService(ILogger logger, ILedgerRepository ledgerRepo, IUserRepository userRepo)
        {
            _logger = logger;
            _ledgerRepo = ledgerRepo;
            _userRepo = userRepo;
        }

        // Validator ensures non-duplicate role.
        public async Task AddUserRoleAsync(UserRoleRequest request, string userId) =>
            await _userRepo.InsertUserRoleAsync(new UserRole()
            {
                Description = request.Description,
                CreatedBy = userId,
                CreatedDate = DateTime.Now
            });

        public async Task UpdateUserRoleAsync(UpdateUserRoleRequest request, string userId)
        {
            // Validator ensures that a user exists with the provided username AND
            // that the role already exists.
            var user = (await _userRepo.GetUsersByUsernameAsync(request.Username)).First();
            if (user.Id == userId)
            {
                // Prevent a user from changing their own role.
                return;
            }
            await _userRepo.UpdateUserRoleAsync(user.Id, request.Role);
        }


        // Validator ensures non-duplicate frequency.
        public async Task AddFrequencyAsync(FrequencyRequest request, string userId) =>
            await _ledgerRepo.InsertOneAsync(new Frequency()
            {
                Description = request.Description,
                ApproxTimesPerYear = request.ApproxTimesPerYear,
                CreatedBy = userId,
                CreatedDate = DateTime.Now
            });


        // Validator ensures non-duplicate type.
        public async Task AddSalaryTypeAsync(SalaryTypeRequest request, string userId) =>
             await _ledgerRepo.InsertOneAsync(new SalaryType()
             {
                 Description = request.Description,
                 CreatedBy = userId,
                 CreatedDate = DateTime.Now
             });


        // Validator ensures non-duplicate type.
        public async Task AddTransactionTypeAsync(TransactionTypeRequest request, string userId) =>
             await _ledgerRepo.InsertOneAsync(new TransactionType()
             {
                 Description = request.Description,
                 CreatedBy = userId,
                 CreatedDate = DateTime.Now
             });

        public async Task<IEnumerable<SupportTicketResponse>> GetSupportTicketsAsync() =>
            from ticket in await _userRepo.GetSupportTicketsAsync()
            select new SupportTicketResponse()
            {
                Id = ticket.Id,
                SubmittingUserId = ticket.SubmittingUserId,
                SubmittingUserName = ticket.SubmittingUserName,
                Subject = ticket.Subject,
                Content = ticket.Content,
                Resolved = ticket.Resolved,
                CreatedDate = ticket.CreatedDate
            };

        public async Task AddMessageAsync(MessageRequest request, string userId) =>
            await _userRepo.InsertMessageAsync(new Message()
            {
                TicketId = request.TicketId,
                RecipientId = request.RecipientId,
                SenderId = userId,
                Subject = request.Subject,
                Content = request.Content,
                Opened = false,
                CreatedDate = DateTime.Now
            });
    }
}