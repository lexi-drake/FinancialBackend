namespace WebService
{
    public class MessageRequest
    {
        public string TicketId { get; set; }
        public string RecipientId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}