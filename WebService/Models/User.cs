using System;

namespace WebService
{
    public class User
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class UserRole
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}