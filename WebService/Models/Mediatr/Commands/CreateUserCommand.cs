using MediatR;

namespace WebService
{
    public class CreateUserCommand : IRequest
    {
        public CreateUserRequest Request { get; set; }
    }
}