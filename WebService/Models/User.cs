using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebService
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
        public IEnumerable<UsernameChangeData> PreviousUsernames { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class UsernameChangeData
    {
        public string Username { get; set; }
        public DateTime ChangedDate { get; set; }
    }
}