using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class CheckLoginStatusQueryHandlerShould
    {
        private IJwtHelper _jwt = new JwtHelper(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        private Mock<IUserRepository> _repo;
        private CheckLoginStatusQueryHandler _handler;

        public CheckLoginStatusQueryHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();

            _handler = new CheckLoginStatusQueryHandler(logger.Object, _repo.Object, _jwt);
        }
    }
}