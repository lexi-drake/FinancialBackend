using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class AddSupportTicketCommandHandler : IRequestHandler<AddSupportTicketCommand>
    {
        private ILogger _logger;
        private IUserRepository _repo;
        private IJwtHelper _jwt;

        public AddSupportTicketCommandHandler(ILogger logger, IUserRepository repo, IJwtHelper jwt)
        {
            _logger = logger;
            _repo = repo;
            _jwt = jwt;
        }

        public async Task<Unit> Handle(AddSupportTicketCommand command, CancellationToken cancellation)
        {
            _logger.Information($"Adding support ticket with subject {command.Request.Subject}.");

            var userId = _jwt.GetUserIdFromToken(command.Token.Jwt);
            if (userId is null)
            {
                _logger.Throw($"Unable to retrieve user id from jwt {command.Token.Jwt}.");
            }

            var users = await _repo.GetUsersByIdAsync(userId);
            if (!users.Any())
            {
                _logger.Throw($"Unable to find user with id {userId}.");
            }
            var user = users.First();

            await _repo.InsertSupportTicketAsync(new SupportTicket()
            {
                SubmittedById = userId,
                Resolved = false,
                Messages = new List<Message>()
                {
                    new Message()
                    {
                        SentBy = user.Username,
                        Subject = command.Request.Subject,
                        Content = command.Request.Content,
                        Opened=false,
                        CreatedDate = DateTime.Now
                    }
                },
                CreatedDate = DateTime.Now
            });
            return Unit.Value;
        }
    }
}