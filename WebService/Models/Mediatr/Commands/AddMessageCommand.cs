using MediatR;

namespace WebService
{
    public class AddMessageCommand : IRequest
    {
        public Token Token { get; set; }
        public MessageRequest Request { get; set; }
    }
}