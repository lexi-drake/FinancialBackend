using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class AddUserRoleCommandHandler : IRequestHandler<AddUserRoleCommand>
    {
        private ILogger _logger;
        private IUserRepository _repo;

        public AddUserRoleCommandHandler(ILogger logger, IUserRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<Unit> Handle(AddUserRoleCommand command, CancellationToken cancellation)
        {
            _logger.Information($"Adding user role with description {command.Description}.");

            await _repo.InsertUserRoleAsync(new UserRole()
            {
                Description = command.Description,
                CreatedBy = command.CreatedBy,
                CreatedDate = DateTime.Now
            });
            return Unit.Value;
        }
    }
}