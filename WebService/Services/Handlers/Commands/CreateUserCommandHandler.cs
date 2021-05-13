using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;
using BC = BCrypt.Net.BCrypt;

namespace WebService
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        private const string DEFAULT_USER_ROLE = "User";
        private ILogger _logger;
        private IUserRepository _repo;

        public CreateUserCommandHandler(ILogger logger, IUserRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<Unit> Handle(CreateUserCommand command, CancellationToken cancellation)
        {
            var passwordHash = BC.HashPassword(command.Request.Password);

            // Validator ensures non-duplicate username
            await _repo.InsertUserAsync(new User()
            {
                Role = DEFAULT_USER_ROLE,
                Username = command.Request.Username,
                PasswordHash = passwordHash,
                CreatedDate = DateTime.Now
            });
            return Unit.Value;
        }
    }
}