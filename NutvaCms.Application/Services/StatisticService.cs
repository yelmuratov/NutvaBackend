using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;

namespace NutvaCms.Application.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IStatisticRepository _repo;
        private readonly IConfiguration _config;
        private readonly IProductService _productService;

        public StatisticService(
            IStatisticRepository repo,
            IConfiguration config,
            IProductService productService
        )
        {
            _repo = repo;
            _config = config;
            _productService = productService;
        }

        public Task TrackVisitAsync() => _repo.TrackVisitAsync();

        public async Task<bool> AddPurchaseRequestAsync(PurchaseRequestDto dto, string lang = "Uz")
        {
            var success = await _repo.AddPurchaseRequestAsync(dto);
            if (!success)
                return false;

            try
            {
                var botToken = _config["Telegram:BotToken"];
                var groupChatId = _config["Telegram:GroupChatId"];
                var botClient = new TelegramBotClient(botToken);

                // Build product name dictionary using ProductService, use correct language
                var productDict = new Dictionary<Guid, string>();
                foreach (var prod in dto.Products)
                {
                    var product = await _productService.GetByIdAsync(prod.ProductId);
                    if (product != null)
                    {
                        string prodName = lang switch
                        {
                            "Uz" => product.Uz.Name,
                            "Ru" => product.Ru.Name,
                            "En" => product.En.Name,
                            _    => product.Uz.Name
                        };
                        productDict[prod.ProductId] = prodName;
                    }
                    else
                    {
                        productDict[prod.ProductId] = "Noma'lum mahsulot";
                    }
                }

                string productsText = "\n🛍️ *Buyurtmadagi mahsulotlar:*";
                int i = 1;
                foreach (var prod in dto.Products)
                {
                    var prodName = productDict.TryGetValue(prod.ProductId, out var name) ? name : "Noma'lum mahsulot";
                    productsText += $"\n  {i++}. {prodName} - {prod.Quantity} Dona";
                }

                string message =
                    $"🛒 *Yangi xarid so‘rovi!*\n" +
                    $"👤 Ism: {dto.BuyerName}\n" +
                    $"📱 Telefon: {dto.Phone}\n" +
                    $"🎂 Yosh: {dto.Age}\n" +
                    $"🗺️ Hudud: {dto.Region}\n" +
                    $"👥 Kim uchun: {dto.ForWhom}\n" +
                    $"💬 Izoh: {dto.Comment}" +
                    productsText;

                await botClient.SendTextMessageAsync(
                    chatId: groupChatId,
                    text: message,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                );
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public Task<IEnumerable<PurchaseRequest>> GetAllPurchaseRequestsAsync() =>
            _repo.GetAllPurchaseRequestsAsync();

        public Task<IEnumerable<SiteStatistic>> GetSiteStatisticsAsync() =>
            _repo.GetSiteStatisticsAsync();
    }
}
