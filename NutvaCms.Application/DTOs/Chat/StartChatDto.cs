namespace NutvaCms.Application.DTOs.Chat
{
    public class StartChatDto
    {
        public string UserIdentifier { get; set; } = null!;
        public string Text { get; set; } = null!;        // The first message
    }
}
