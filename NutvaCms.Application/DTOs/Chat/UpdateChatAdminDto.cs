namespace NutvaCms.Application.DTOs.Chat
{
    public class UpdateChatAdminDto
    {
        public required string FullName { get; set; }
        public required long TelegramUserId { get; set; }
        public bool IsBusy { get; set; }
    }
}