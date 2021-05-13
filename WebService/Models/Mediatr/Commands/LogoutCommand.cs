using MediatR;

namespace WebService
{
    public class LogoutCommand : IRequest
    {
        public Token Token { get; set; }
    }
}