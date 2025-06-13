using Microsoft.Extensions.Options;
using Telegram.Bot;
using NutvaCms.Application.Settings;

namespace NutvaCms.Application.Services
{
    public class TelegramService
    {
        private readonly ITelegramBotClient _botClient;

        public TelegramService(IOptions<TelegramSettings> options)
        {
            _botClient = new TelegramBotClient(options.Value.BotToken);
        }

        public async Task SendMessageAsync(long chatId, string text)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: text
            );
        }
    }
}
