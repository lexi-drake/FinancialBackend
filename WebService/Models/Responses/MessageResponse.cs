using System;

namespace WebService
{
    public class MessageResponse
    {
        // Username
        public string SentBy { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool Opened { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}