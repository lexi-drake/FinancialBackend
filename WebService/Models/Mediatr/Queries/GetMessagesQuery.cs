using System.Collections.Generic;
using MediatR;

namespace WebService
{
    public class GetMessagesQuery : IRequest<IEnumerable<SupportTicketResponse>>
    {
        public string UserId { get; set; }
    }
}