using FluentValidation;

namespace WebService
{
    public class SupportTicketRequestValidator : AbstractValidator<SupportTicketRequest>
    {
        public SupportTicketRequestValidator()
        {
            RuleFor(x => x.Subject).NotEmpty();
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}