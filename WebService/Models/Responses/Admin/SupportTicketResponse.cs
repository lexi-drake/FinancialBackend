using System;
using System.Collections.Generic;

namespace WebService
{
    public class SupportTicketResponse
    {
        public string Id { get; set; }
        public bool Resolved { get; set; }
        public IEnumerable<Message> Messages { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}