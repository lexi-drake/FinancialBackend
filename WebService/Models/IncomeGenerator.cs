using System;
using System.Collections.Generic;

namespace WebService
{
    public class IncomeGenerator
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public string SalaryTypeId { get; set; }
        public string FrequencyId { get; set; }
        public IEnumerable<string> RecurringTransactions { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}