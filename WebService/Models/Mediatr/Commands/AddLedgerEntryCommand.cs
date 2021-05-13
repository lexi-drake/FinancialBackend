using MediatR;

namespace WebService
{
    public class AddLedgerEntryCommand : IRequest
    {
        public string UserId { get; set; }
        public LedgerEntryRequest Request { get; set; }
    }
}