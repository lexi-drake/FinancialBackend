using System.Collections.Generic;
using MediatR;

namespace WebService
{
    public class GetLedgerItemsQuery<T> : IRequest<IEnumerable<T>> where T : AbstractLedgerItem { }
}