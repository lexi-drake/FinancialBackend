using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class GetMessagesQueryHandlerShould
    {
        private IJwtHelper _jwt = new JwtHelper(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        private Mock<IUserRepository> _repo;
        private GetMessagesQueryHandler _handler;

        public GetMessagesQueryHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();

            _handler = new GetMessagesQueryHandler(logger.Object, _repo.Object, _jwt);
        }
    }
}