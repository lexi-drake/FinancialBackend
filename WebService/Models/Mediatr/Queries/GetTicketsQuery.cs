using System.Collections.Generic;
using MediatR;

namespace WebService
{
    public class GetTicketsQuery : IRequest<IEnumerable<SupportTicketResponse>>
    {
        public Token Token { get; set; }
    }
}