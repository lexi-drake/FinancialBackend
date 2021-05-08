using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, IEnumerable<SupportTicketResponse>>
    {
        private ILogger _logger;
        private IUserRepository _repo;

        public GetMessagesQueryHandler(ILogger logger, IUserRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<IEnumerable<SupportTicketResponse>> Handle(GetMessagesQuery query, CancellationToken cancellation)
        {
            var users = await _repo.GetUsersByIdAsync(query.UserId);
            if (!users.Any())
            {
                _logger.Throw($"Unable to find user with id {query.UserId}.");
            }

            var user = users.First();
            if (user.Role.Equals("admin", StringComparison.CurrentCultureIgnoreCase))
            {
                return await GetAllTicketsAsync();
            }
            return await GetUserTicketsAsync(user.Id);
        }

        private async Task<IEnumerable<SupportTicketResponse>> GetAllTicketsAsync() =>
             ToResponse(await _repo.GetSupportTicketsAsync());

        private async Task<IEnumerable<SupportTicketResponse>> GetUserTicketsAsync(string userId) =>
            ToResponse(await _repo.GetSupportTicketsSubmittedByUser(userId));

        private IEnumerable<SupportTicketResponse> ToResponse(IEnumerable<SupportTicket> tickets) =>
            from ticket in tickets
            select new SupportTicketResponse()
            {
                Id = ticket.Id,
                Resolved = ticket.Resolved,
                Messages = from message in ticket.Messages
                           select new MessageResponse()
                           {
                               SentBy = message.SentBy,
                               Subject = message.Subject,
                               Content = message.Content,
                               Opened = message.Opened,
                               CreatedDate = message.CreatedDate
                           },
                CreatedDate = ticket.CreatedDate

            };
    }
}