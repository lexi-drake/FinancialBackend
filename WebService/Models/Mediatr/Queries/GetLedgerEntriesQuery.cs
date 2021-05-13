using System.Collections.Generic;
using MediatR;

namespace WebService
{
    public class GetLedgerEntriesQuery : IRequest<IEnumerable<LedgerEntryResponse>>
    {
        public string UserId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}