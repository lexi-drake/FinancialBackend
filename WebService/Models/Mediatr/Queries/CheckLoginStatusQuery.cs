using MediatR;

namespace WebService
{
    public class CheckLoginStatusQuery : IRequest<LoginResponse>
    {
        public Token Token { get; set; }
    }
}