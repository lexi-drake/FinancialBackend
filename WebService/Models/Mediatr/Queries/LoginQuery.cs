using MediatR;

namespace WebService
{
    public class LoginQuery : IRequest<LoginResponse>
    {
        public LoginRequest Request { get; set; }
    }
}