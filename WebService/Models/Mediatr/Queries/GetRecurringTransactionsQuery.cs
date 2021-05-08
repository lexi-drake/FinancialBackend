using System.Collections.Generic;
using MediatR;

namespace WebService
{
    public class GetRecurringTransactionsQuery : IRequest<IEnumerable<RecurringTransactionResponse>>
    {
        public string UserId { get; set; }
    }
}