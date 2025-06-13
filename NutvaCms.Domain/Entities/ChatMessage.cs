namespace NutvaCms.Domain.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int ChatSessionId { get; set; }
        public ChatSession ChatSession { get; set; } = null!;
        public string Sender { get; set; } = null!; // "user" or "admin"
        public string Text { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
