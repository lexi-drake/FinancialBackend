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
        private IJwtHelper _jwt;

        public GetMessagesQueryHandler(ILogger logger, IUserRepository repo, IJwtHelper jwt)
        {
            _logger = logger;
            _repo = repo;
            _jwt = jwt;
        }

        public async Task<IEnumerable<SupportTicketResponse>> Handle(GetMessagesQuery query, CancellationToken cancellation)
        {
            var role = _jwt.GetRoleFromToken(query.Token.Jwt);
            if (string.IsNullOrEmpty(role))
            {
                _logger.Throw($"Unable to retrieve role from jwt {query.Token.Jwt}.");
            }

            if (role.Equals("admin", StringComparison.CurrentCultureIgnoreCase))
            {
                return await GetAllTicketsAsync();
            }

            // If a role was retrieved, it's a valid Jwt.
            var userId = _jwt.GetUserIdFromToken(query.Token.Jwt);
            return await GetUserTicketsAsync(userId);
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