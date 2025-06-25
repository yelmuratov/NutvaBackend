namespace NutvaCms.Application.DTOs.Chat
{
    public class ChatMessageDto
    {
        public string Sender { get; set; } // "admin" or "user"
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
    }
}
