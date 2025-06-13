namespace NutvaCms.Application.DTOs.Chat
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public string Sender { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime SentAt { get; set; }
    }
}
