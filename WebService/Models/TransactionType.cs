using System;

namespace WebService
{
    public class TransactionType
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}