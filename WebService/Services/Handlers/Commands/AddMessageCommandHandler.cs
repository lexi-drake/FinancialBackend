using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class AddMessageCommandHandler : IRequestHandler<AddMessageCommand>
    {
        private ILogger _logger;
        private IUserRepository _repo;
        private IJwtHelper _jwt;

        public AddMessageCommandHandler(ILogger logger, IUserRepository repo, IJwtHelper jwt)
        {
            _logger = logger;
            _repo = repo;
            _jwt = jwt;
        }

        public async Task<Unit> Handle(AddMessageCommand command, CancellationToken cancellation)
        {
            _logger.Information($"Adding message with subject {command.Request.Subject} to ticket {command.Request.TicketId}.");

            var userId = _jwt.GetUserIdFromToken(command.Token.Jwt);
            if (userId is null)
            {
                _logger.Throw($"Unable to get user id from jwt {command.Token.Jwt}.");
            }
            var role = _jwt.GetRoleFromToken(command.Token.Jwt);

            var tickets = await _repo.GetSupportTicketsByIdAsync(command.Request.TicketId);
            if (!tickets.Any())
            {
                _logger.Throw($"Unable to find support ticket with Id {command.Request.TicketId}.");
            }
            var ticket = tickets.First();

            // Only admins and the user who originally created the ticket are allowed to add
            // a message to the ticket.
            if (!role.Equals("admin", StringComparison.InvariantCultureIgnoreCase) && ticket.SubmittedById != userId)
            {
                _logger.Throw($"User with id {userId} attempted to add a message to ticket {command.Request.TicketId}, but is not allowed.");
            }

            var users = await _repo.GetUsersByIdAsync(userId);
            if (!users.Any())
            {
                _logger.Throw($"Unable to find user with id {userId}.");
            }
            var user = users.First();

            await _repo.AddMessageToSupportTicketAsync(command.Request.TicketId, new Message()
            {
                SentBy = user.Username,
                Subject = command.Request.Subject,
                Content = command.Request.Content,
                Opened = false,
                CreatedDate = DateTime.Now
            });
            return Unit.Value;
        }
    }
}