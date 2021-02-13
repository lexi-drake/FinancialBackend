namespace WebService
{
    public class MessageResponse
    {
        public string TicketId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool Opened { get; set; }
    }
}