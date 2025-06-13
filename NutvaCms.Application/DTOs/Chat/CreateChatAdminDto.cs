namespace NutvaCms.Application.DTOs.Chat
{
    public class CreateChatAdminDto
    {
        public required string FullName { get; set; }
        public required long TelegramUserId { get; set; }
    }
}