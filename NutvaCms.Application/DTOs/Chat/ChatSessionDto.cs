namespace NutvaCms.Application.DTOs.Chat
{
    public class ChatSessionDto
    {
        public int Id { get; set; }
        public int? ChatAdminId { get; set; }
        public string? ChatAdminName { get; set; }
        public string UserIdentifier { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public List<ChatMessageDto> Messages { get; set; } = new();
    }
}
