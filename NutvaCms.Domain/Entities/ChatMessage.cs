namespace NutvaCms.Domain.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }

        public Guid ChatSessionId { get; set; }
        public ChatSession Session { get; set; }

        public string Sender { get; set; } // "user" or "admin"
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }

}
