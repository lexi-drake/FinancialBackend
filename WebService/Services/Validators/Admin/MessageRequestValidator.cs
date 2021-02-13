using FluentValidation;

namespace WebService
{
    public class MessageRequestValidator : AbstractValidator<MessageRequest>
    {
        private IUserRepository _repo;

        public MessageRequestValidator(IUserRepository repo)
        {
            _repo = repo;

            RuleFor(x => x.TicketId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (id, cancellation) =>
                {
                    return await _repo.SupportTicketExistsWithIdAsync(id);
                }).WithMessage(request => $"Ticket not found with id {request.TicketId}.");
            RuleFor(x => x.RecipientId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (id, cancellation) =>
                {
                    return await _repo.UserExistsWithIdAsync(id);
                }).WithMessage(request => $"User not found with id {request.RecipientId}.");
            RuleFor(x => x.Subject).NotEmpty();
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}