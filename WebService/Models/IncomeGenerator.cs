using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebService
{
    public class IncomeGenerator
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public string SalaryTypeId { get; set; }
        public string FrequencyId { get; set; }
        public IEnumerable<string> RecurringTransactions { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}