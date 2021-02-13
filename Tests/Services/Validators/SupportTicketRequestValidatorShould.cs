using System;
using Xunit;
using WebService;

namespace Tests
{
    public class SupportTicketRequestValidatorShould
    {
        private SupportTicketRequestValidator _validator = new SupportTicketRequestValidator();

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        public void RejectsForEmptySubject(string subject)
        {
            var request = CreateValidRequest();
            request.Subject = subject;

            var result = _validator.Validate(request);
            AssertHelper.FailsWithMessage(result, "'Subject' must not be empty.");
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        public void RejectsForEmptyContent(string content)
        {
            var request = CreateValidRequest();
            request.Content = content;

            var result = _validator.Validate(request);
            AssertHelper.FailsWithMessage(result, "'Content' must not be empty.");
        }

        [Fact]
        public void PassesValidRequest()
        {
            var request = CreateValidRequest();
            var result = _validator.Validate(request);
            Assert.True(result.IsValid);
        }

        private SupportTicketRequest CreateValidRequest()
        {
            return new SupportTicketRequest()
            {
                Subject = Guid.NewGuid().ToString(),
                Content = Guid.NewGuid().ToString()
            };
        }
    }
}