using MediatR;

namespace WebService
{
    public class DeleteRecurringTransactionCommand : IRequest
    {
        public string UserId { get; set; }
        public string Id { get; set; }
    }
}