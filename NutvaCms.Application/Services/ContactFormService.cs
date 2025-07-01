using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

public class ContactFormService : IContactFormService
{
    private readonly IContactFormRepository _repo;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public ContactFormService(
        IContactFormRepository repo,
        IHttpClientFactory httpClientFactory,
        IConfiguration config)
    {
        _repo = repo;
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    public async Task<bool> SubmitContactFormAsync(ContactFormDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Phone))
            return false;

        var entity = new ContactForm
        {
            Name = dto.Name,
            Phone = dto.Phone,
            Comment = dto.Comment
        };
        await _repo.AddAsync(entity);

        try
        {
            var botToken = _config["Telegram:BotToken"];
            var groupChatId = _config["Telegram:GroupChatId"];
            var botClient = new TelegramBotClient(botToken);

            // 🌐 Universal phone normalization
            string phoneDigits = new string(dto.Phone.Where(char.IsDigit).ToArray());
            string normalizedPhone = dto.Phone.TrimStart().StartsWith("+")
                ? "+" + phoneDigits
                : "+" + phoneDigits.TrimStart('0'); // fallback assumption

            var messageText = new StringBuilder();
            messageText.AppendLine("📝 *New contact form submission*");
            messageText.AppendLine($"👤 *Name:* {dto.Name}");
            messageText.AppendLine($"📞 *Phone:* {normalizedPhone}");
            if (!string.IsNullOrWhiteSpace(dto.Comment))
                messageText.AppendLine($"💬 *Comment:* {dto.Comment}");

            await botClient.SendTextMessageAsync(
                chatId: groupChatId,
                text: messageText.ToString(),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Telegram send error: " + ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        return true;
    }

    public async Task<IEnumerable<ContactFormDto>> GetAllAsync()
    {
        var entities = await _repo.GetAllAsync();
        return entities.Select(e => new ContactFormDto
        {
            Name = e.Name,
            Phone = e.Phone,
            Comment = e.Comment
        });
    }
}
