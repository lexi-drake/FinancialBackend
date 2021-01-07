using System.Collections.Generic;

namespace WebService
{
    public class IncomeGeneratorResponse
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string SalaryTypeId { get; set; }
        public string FrequencyId { get; set; }
        public IEnumerable<RecurringTransaction> RecurringTransactions { get; set; }
    }
}