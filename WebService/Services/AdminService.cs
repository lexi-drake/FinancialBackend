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

        public async Task<IEnumerable<SupportTicketResponse>> GetSupportTicketsAsync()
        {
            var tickets = await _userRepo.GetSupportTicketsAsync();

            var userIds = from ticket in tickets
                          from message in ticket.Messages
                          select message.SentById;
            var userInfo = await _userRepo.GetUsernames(userIds);

            return from ticket in tickets
                   select new SupportTicketResponse()
                   {
                       Id = ticket.Id,
                       Resolved = ticket.Resolved,
                       Messages = from message in ticket.Messages
                                  select new MessageResponse()
                                  {
                                      SentBy = userInfo[message.SentById],
                                      Subject = message.Subject,
                                      Content = message.Content,
                                      Opened = message.Opened,
                                      CreatedDate = message.CreatedDate
                                  },
                       CreatedDate = ticket.CreatedDate
                   };
        }

        public async Task ResolveSupportTicketAsync(string ticketId) =>
            await _userRepo.UpdateSupportTicketResolvedAsync(ticketId, true);

        public async Task AddMessageAsync(MessageRequest request, string userId) =>
            await _userRepo.AddMessageToSupportTicketAsync(request.TicketId, new Message()
            {
                SentById = userId,
                Subject = request.Subject,
                Content = request.Content,
                Opened = false,
                CreatedDate = DateTime.Now
            });
    }
}