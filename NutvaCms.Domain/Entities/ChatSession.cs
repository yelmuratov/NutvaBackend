namespace NutvaCms.Domain.Entities
{
    public class ChatSession
    {
        public int Id { get; set; }
        public int? ChatAdminId { get; set; }   // nullable for waiting state
        public ChatAdmin? ChatAdmin { get; set; }
        public string UserIdentifier { get; set; } = null!; // e.g., website user ID or connection ID
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }

        public List<ChatMessage> Messages { get; set; } = new();
    }
}
    