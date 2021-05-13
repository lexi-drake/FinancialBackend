using MediatR;

namespace WebService
{
    public class AddRecurringTransactionQuery : IRequest<string>
    {
        // This is a query, instead of a command, because it's
        // used to get transaction ids of recurring transactions
        // when adding an IncomeGenerator.

        public string UserId { get; set; }
        public RecurringTransactionRequest Request { get; set; }
    }
}