using MediatR;

namespace WebService
{
    public class AddMessageCommand : IRequest
    {
        public string UserId { get; set; }
        public MessageRequest Request { get; set; }
    }
}