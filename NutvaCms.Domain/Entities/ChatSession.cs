namespace NutvaCms.Domain.Entities
{
    public class ChatSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // anonymous user reference (stored in cookie or temporary)
        public string AnonymousId { get; set; }

        // related Telegram Admin
        public int AdminId { get; set; }
        public ChatAdmin Admin { get; set; }

        public bool IsClosed { get; set; } = false;

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }

        public ICollection<ChatMessage> Messages { get; set; }
    }
}
