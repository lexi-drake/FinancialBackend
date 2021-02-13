using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Moq;
using WebService;

namespace Tests
{
    public class MessageRequestValidatorShould
    {
        private string _ticketIdExists = Guid.NewGuid().ToString();
        private string _ticketIdNotExists = Guid.NewGuid().ToString();
        private string _userIdExists = Guid.NewGuid().ToString();
        private string _userIdNotExists = Guid.NewGuid().ToString();

        private Mock<IUserRepository> _repo;
        private MessageRequestValidator _validator;

        public MessageRequestValidatorShould()
        {
            _repo = new Mock<IUserRepository>();
            _repo.Setup(x => x.SupportTicketExistsWithIdAsync(_ticketIdExists))
                .Returns(Task.FromResult(true));
            _repo.Setup(x => x.SupportTicketExistsWithIdAsync(_ticketIdNotExists))
                .Returns(Task.FromResult(false));
            _repo.Setup(x => x.UserExistsWithIdAsync(_userIdExists))
                .Returns(Task.FromResult(true));
            _repo.Setup(x => x.UserExistsWithIdAsync(_userIdNotExists))
                .Returns(Task.FromResult(false));

            _validator = new MessageRequestValidator(_repo.Object);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task RejectsForEmptyTicketId(string ticketId)
        {
            var request = CreateValidRequest();
            request.TicketId = ticketId;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Ticket Id' must not be empty.");
        }

        [Fact]
        public async Task RejectsForTicketNotExists()
        {
            var request = CreateValidRequest();
            request.TicketId = _ticketIdNotExists;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, $"Ticket not found with id {request.TicketId}.");
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task RejectsForEmptyRecipientId(string recipientId)
        {
            var request = CreateValidRequest();
            request.RecipientId = recipientId;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Recipient Id' must not be empty.");
        }

        [Fact]
        public async Task RejectsForRecipientNotExists()
        {
            var request = CreateValidRequest();
            request.RecipientId = _userIdNotExists;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, $"User not found with id {request.RecipientId}.");
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task RejectsForEmptySubject(string subject)
        {
            var request = CreateValidRequest();
            request.Subject = subject;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Subject' must not be empty.");
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task RejectsForEmptyContent(string content)
        {
            var request = CreateValidRequest();
            request.Content = content;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Content' must not be empty.");
        }

        private MessageRequest CreateValidRequest()
        {
            return new MessageRequest()
            {
                TicketId = _ticketIdExists,
                RecipientId = _userIdExists,
                Subject = Guid.NewGuid().ToString(),
                Content = Guid.NewGuid().ToString()
            };
        }
    }
}