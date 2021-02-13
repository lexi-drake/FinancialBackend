using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebService
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TicketId { get; set; }
        public string RecipientId { get; set; }
        public string SenderId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool Opened { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}