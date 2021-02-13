using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebService
{
    public class SupportTicket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string SubmittingUserId { get; set; }
        public string SubmittingUserName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool Resolved { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}