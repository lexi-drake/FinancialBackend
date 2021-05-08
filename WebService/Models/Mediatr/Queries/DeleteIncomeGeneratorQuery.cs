using System.Collections.Generic;
using MediatR;

namespace WebService
{
    public class DeleteIncomeGeneratorQuery : IRequest<IEnumerable<string>>
    {
        // This is a query, instead of a command, because it's
        // used to get transaction ids of recurring transactions
        // associated with the income generator being deleted.

        public string UserId { get; set; }
        public string Id { get; set; }
    }
}