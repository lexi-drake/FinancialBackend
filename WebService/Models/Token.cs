using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebService
{
    public class Token
    {
        public string Jwt { get; set; }
        public string Refresh { get; set; }
    }

    public class RefreshData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Refresh { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}