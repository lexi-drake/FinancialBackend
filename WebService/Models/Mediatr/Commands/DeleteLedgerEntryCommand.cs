using MediatR;

namespace WebService
{
    public class DeleteLedgerEntryCommand : IRequest
    {
        public string UserId { get; set; }
        public string Id { get; set; }
    }
}