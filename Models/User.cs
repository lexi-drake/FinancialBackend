using System;

namespace financial_backend
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}