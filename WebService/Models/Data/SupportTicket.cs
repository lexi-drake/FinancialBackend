using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebService
{
    public class SupportTicket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string SubmittedById { get; set; }
        public bool Resolved { get; set; }
        public IEnumerable<Message> Messages { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class Message
    {
        // Username
        public string SentBy { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool Opened { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}