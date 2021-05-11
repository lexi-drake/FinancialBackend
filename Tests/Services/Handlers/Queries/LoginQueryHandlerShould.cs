using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class LoginQueryHandlerShould
    {
        private IJwtHelper _jwt = new JwtHelper(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        private Mock<IUserRepository> _repo;
        private LoginQueryHandler _handler;

        public LoginQueryHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();

            _handler = new LoginQueryHandler(logger.Object, _repo.Object, _jwt);
        }
    }
}