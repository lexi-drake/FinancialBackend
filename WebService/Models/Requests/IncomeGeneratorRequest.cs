using System.Collections.Generic;

namespace WebService
{
    public class IncomeGeneratorRequest
    {
        public string Description { get; set; }
        public string SalaryTypeId { get; set; }
        public string FrequencyId { get; set; }
        public IEnumerable<RecurringTransactionRequest> RecurringTransactions { get; set; }
    }
}