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
            _userRepo.Setup(x => x.GetUsersByUsernameAsync(_validUsername))
                .Returns(Task.FromResult(users));

            _service = new AdminService(new Mock<ILogger>().Object,
                _ledgerRepo.Object,
                _userRepo.Object);
        }

        [Fact]
        public async Task InsertsUserRole()
        {
            var request = new UserRoleRequest() { Description = Guid.NewGuid().ToString() };

            var role = await _service.AddUserRoleAsync(request, _userIdFromDbUser);
            Assert.NotNull(role);
            Assert.False(string.IsNullOrEmpty(role.Id));
            Assert.Equal(request.Description, role.Description);
            Assert.Equal(_userIdFromDbUser, role.CreatedBy);
            Assert.NotNull(role.CreatedDate);
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
            var request = new FrequencyRequest()
            {
                Description = Guid.NewGuid().ToString(),
                ApproxTimesPerYear = new Random().Next(1, 52)
            };
            var userId = Guid.NewGuid().ToString();

            var frequency = await _service.AddFrequencyAsync(request, userId);
            Assert.NotNull(frequency);
            Assert.Equal(request.Description, frequency.Description);
            Assert.Equal(request.ApproxTimesPerYear, frequency.ApproxTimesPerYear);
            Assert.Equal(userId, frequency.CreatedBy);
        }

        [Fact]
        public async Task AddSalaryType()
        {
            var request = new SalaryTypeRequest()
            {
                Description = Guid.NewGuid().ToString()
            };
            var userId = Guid.NewGuid().ToString();

            var type = await _service.AddSalaryTypeAsync(request, userId);
            Assert.NotNull(type);
            Assert.Equal(request.Description, type.Description);
            Assert.Equal(userId, type.CreatedBy);
        }

        [Fact]
        public async Task AddTransactionType()
        {
            var request = new TransactionTypeRequest()
            {
                Description = Guid.NewGuid().ToString()
            };
            var userId = Guid.NewGuid().ToString();

            var type = await _service.AddTransactionTypeAsync(request, userId);
            Assert.NotNull(type);
            Assert.Equal(request.Description, type.Description);
            Assert.Equal(userId, type.CreatedBy);
        }
    }
}