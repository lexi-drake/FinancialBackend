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

        public AddMessageCommandHandler(ILogger logger, IUserRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<Unit> Handle(AddMessageCommand command, CancellationToken cancellation)
        {
            _logger.Information($"Adding message with subject {command.Request.Subject} to ticket {command.Request.TicketId}.");

            var users = await _repo.GetUsersByIdAsync(command.UserId);
            if (!users.Any())
            {
                _logger.Throw($"Unable to find user with Id {command.UserId}.");
            }
            var user = users.First();

            var tickets = await _repo.GetSupportTicketsByIdAsync(command.Request.TicketId);
            if (!tickets.Any())
            {
                _logger.Throw($"Unable to find support ticket with Id {command.Request.TicketId}.");
            }

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