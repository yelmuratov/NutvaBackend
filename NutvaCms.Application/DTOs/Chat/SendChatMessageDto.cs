namespace NutvaCms.Application.DTOs.Chat
{
    public class SendChatMessageDto
    {
        public int SessionId { get; set; }
        public string Text { get; set; } = null!;
        public string Sender { get; set; } = null!;
    }
}
