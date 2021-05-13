using MediatR;

namespace WebService
{
    public class AddLedgerItemCommand : IRequest
    {
        public string UserId { get; set; }
        public AbstractLedgerItem LedgerItem { get; set; }
    }
}