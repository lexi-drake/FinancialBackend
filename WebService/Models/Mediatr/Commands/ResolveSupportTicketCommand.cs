using MediatR;

namespace WebService
{
    public class ResolveSupportTicketCommand : IRequest
    {
        public string Id { get; set; }
        public string UserId { get; set; }
    }
}