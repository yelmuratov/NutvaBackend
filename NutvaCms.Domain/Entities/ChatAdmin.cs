namespace NutvaCms.Domain.Entities
{
    public class ChatAdmin
    {
        public int Id { get; set; } // Primary Key
        public required string FullName { get; set; }
        public required long TelegramUserId { get; set; }
        public bool IsBusy { get; set; } = false;
        public bool IsOnline { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
