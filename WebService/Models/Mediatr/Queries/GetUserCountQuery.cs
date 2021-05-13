using MediatR;

namespace WebService
{
    public class GetUserCountQuery : IRequest<long> { }
}