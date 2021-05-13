using MediatR;

namespace WebService
{
    public class RefreshTokenQuery : IRequest<Token>
    {
        public Token Token;
    }
}