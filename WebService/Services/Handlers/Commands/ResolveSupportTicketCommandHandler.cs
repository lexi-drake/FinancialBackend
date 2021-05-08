using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class ResolveSupportTicketCommandHandler : IRequestHandler<ResolveSupportTicketCommand>
    {
        private ILogger _logger;
        private IUserRepository _repo;

        public ResolveSupportTicketCommandHandler(ILogger logger, IUserRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<Unit> Handle(ResolveSupportTicketCommand command, CancellationToken cancellation)
        {
            _logger.Information($"Resolving support ticket with Id {command.Id}.");

            var users = await _repo.GetUsersByIdAsync(command.UserId);
            if (!users.Any())
            {
                _logger.Throw($"Unable to find user with Id {command.UserId}.");
            }

            var user = users.First();
            if (!user.Role.Equals("admin", StringComparison.InvariantCultureIgnoreCase))
            {
                var tickets = await _repo.GetSupportTicketsByIdAsync(command.Id);
                if (!tickets.Any())
                {
                    _logger.Throw($"Unable to find support ticket with Id {command.Id}.");
                }

                var ticket = tickets.First();
                if (ticket.SubmittedById != command.UserId)
                {
                    _logger.Throw($"User {command.UserId} cannot resolve submit ticket with Id {command.Id}.");
                }
            }

            await _repo.UpdateSupportTicketResolvedAsync(command.Id, true);
            return Unit.Value;
        }
    }
}