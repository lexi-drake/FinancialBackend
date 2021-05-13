using MediatR;

namespace WebService
{
    public class AddSupportTicketCommand : IRequest
    {
        public SupportTicketRequest Request { get; set; }
        public Token Token { get; set; }
    }
}