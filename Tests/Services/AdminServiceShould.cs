using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using Xunit;
using Moq;
using WebService;

namespace Tests
{
    public class AdminServiceShould
    {
        private string _validUsername = Guid.NewGuid().ToString();
        private string _userIdFromDbUser = Guid.NewGuid().ToString();
        private string _userIdNotFromDbUser = Guid.NewGuid().ToString();
        private Mock<ILedgerRepository> _ledgerRepo;
        private Mock<IUserRepository> _userRepo;

        private IAdminService _service;

        public AdminServiceShould()
        {
            IEnumerable<User> users = new List<User>() { new User() { Id = _userIdFromDbUser, Username = _validUsername } };
            IEnumerable<SupportTicket> tickets = new List<SupportTicket>()
            {
                 new SupportTicket()
                 {
                     Messages = new List<Message>()
                     {
                         new Message()
                         {
                             SentById = Guid.NewGuid().ToString()
                         }
                     }
                 }
            };

            _ledgerRepo = new Mock<ILedgerRepository>();
            _ledgerRepo.Setup(x => x.InsertOneAsync(It.IsAny<Frequency>()))
                .Returns<Frequency>(frequency =>
                {
                    frequency.Id = Guid.NewGuid().ToString();
                    return Task.FromResult(frequency);
                });
            _ledgerRepo.Setup(x => x.InsertOneAsync(It.IsAny<SalaryType>()))
                .Returns<SalaryType>(type =>
                {
                    type.Id = Guid.NewGuid().ToString();
                    return Task.FromResult(type);
                });
            _ledgerRepo.Setup(x => x.InsertOneAsync(It.IsAny<TransactionType>()))
                .Returns<TransactionType>(type =>
                {
                    type.Id = Guid.NewGuid().ToString();
                    return Task.FromResult(type);
                });

            _userRepo = new Mock<IUserRepository>();
            _userRepo.Setup(x => x.InsertUserRoleAsync(It.IsAny<UserRole>()))
                .Returns<UserRole>(role =>
                {
                    role.Id = Guid.NewGuid().ToString();
                    return Task.FromResult(role);
                });
            _userRepo.Setup(x => x.GetUsersByIdAsync(_userIdFromDbUser))
                .Returns(Task.FromResult(users));
            _userRepo.Setup(x => x.GetUsersByUsernameAsync(_validUsername))
                .Returns(Task.FromResult(users));
            _userRepo.Setup(x => x.GetSupportTicketsAsync())
                .Returns(Task.FromResult(tickets));

            _service = new AdminService(new Mock<ILogger>().Object,
                _ledgerRepo.Object,
                _userRepo.Object);
        }

        [Fact]
        public async Task InsertsUserRole()
        {
            await _service.AddUserRoleAsync(new UserRoleRequest(), _userIdFromDbUser);
            _userRepo.Verify(x => x.InsertUserRoleAsync(It.IsAny<UserRole>()), Times.Once);
        }

        [Fact]
        public async Task DoesNotUpdateRoleForSameUser()
        {
            var request = new UpdateUserRoleRequest()
            {
                Username = _validUsername,
                Role = Guid.NewGuid().ToString()
            };

            await _service.UpdateUserRoleAsync(request, _userIdFromDbUser);
            _userRepo.Verify(x => x.GetUsersByUsernameAsync(_validUsername), Times.Once());
            _userRepo.Verify(x => x.UpdateUserRoleAsync(_userIdFromDbUser, request.Role), Times.Never());
        }

        [Fact]
        public async Task UpdatesRoleForDifferentUser()
        {
            var request = new UpdateUserRoleRequest()
            {
                Username = _validUsername,
                Role = Guid.NewGuid().ToString()
            };

            await _service.UpdateUserRoleAsync(request, _userIdNotFromDbUser);
            _userRepo.Verify(x => x.GetUsersByUsernameAsync(_validUsername), Times.Once());
            _userRepo.Verify(x => x.UpdateUserRoleAsync(_userIdFromDbUser, request.Role), Times.Once());
            _userRepo.Verify(x => x.UpdateUserRoleAsync(_userIdNotFromDbUser, request.Role), Times.Never());
        }

        [Fact]
        public async Task AddsFrequency()
        {
            var userId = Guid.NewGuid().ToString();
            await _service.AddFrequencyAsync(new FrequencyRequest(), userId);
            _ledgerRepo.Verify(x => x.InsertOneAsync<Frequency>(It.IsAny<Frequency>()), Times.Once);
        }

        [Fact]
        public async Task AddSalaryType()
        {
            var userId = Guid.NewGuid().ToString();
            await _service.AddSalaryTypeAsync(new SalaryTypeRequest(), userId);
            _ledgerRepo.Verify(x => x.InsertOneAsync<SalaryType>(It.IsAny<SalaryType>()), Times.Once);
        }

        [Fact]
        public async Task AddTransactionType()
        {
            var userId = Guid.NewGuid().ToString();
            await _service.AddTransactionTypeAsync(new TransactionTypeRequest(), userId);
            _ledgerRepo.Verify(x => x.InsertOneAsync<TransactionType>(It.IsAny<TransactionType>()), Times.Once);
        }

        [Fact]
        public async Task ReturnsTickets()
        {
            var tickets = await _service.GetSupportTicketsAsync();
            Assert.NotNull(tickets);
            Assert.Single(tickets);
        }

        [Fact]
        public async Task InsertsMessage()
        {
            await _service.AddMessageAsync(new MessageRequest(), _userIdFromDbUser);
            _userRepo.Verify(x => x.AddMessageToSupportTicketAsync(It.IsAny<String>(), It.IsAny<Message>()), Times.Once);
        }
    }
}