namespace NutvaCms.Application.DTOs.Chat
{
    public class ChatAdminDto
    {
        public int Id { get; set; }
        public long TelegramUserId { get; set; }
        public string? FullName { get; set; }
        public bool IsBusy { get; set; }
        public bool IsOnline { get; set; }
    }
}
