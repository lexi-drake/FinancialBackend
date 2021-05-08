using System.Collections.Generic;
using System.Linq;

namespace WebService
{
    public static class IEnumerableExtensions
    {

        public static IEnumerable<RecurringTransactionResponse> Compile(this IEnumerable<RecurringTransaction> recurringTransactions, IEnumerable<string> ids, IEnumerable<TransactionType> transactionTypes) =>
             from id in ids
             from transaction in recurringTransactions
             where transaction.Id == id
             select RecurringTransactionResponse.FromDBObject(transaction, transactionTypes);
    }
}