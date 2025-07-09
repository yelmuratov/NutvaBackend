using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using Telegram.Bot;

namespace NutvaCms.Application.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IStatisticRepository _repo;
        private readonly IConfiguration _config;
        private readonly IProductService _productService;
        private readonly IProductBoxPriceService _boxPriceService;

        public StatisticService(
            IStatisticRepository repo,
            IConfiguration config,
            IProductService productService,
            IProductBoxPriceService boxPriceService)
        {
            _repo = repo;
            _config = config;
            _productService = productService;
            _boxPriceService = boxPriceService;
        }

        public Task TrackVisitAsync() => _repo.TrackVisitAsync();

        private decimal CalculateDiscountedPrice(decimal productPrice, string discountLabel)
        {
            if (string.IsNullOrWhiteSpace(discountLabel) || discountLabel == "0%")
                return productPrice;

            var match = Regex.Match(discountLabel, @"\d+");
            if (!match.Success) return productPrice;

            var percent = decimal.Parse(match.Value);
            var discounted = productPrice * (1 - percent / 100m);
            return Math.Round(discounted, 0);
        }

        public async Task<bool> AddPurchaseRequestAsync(PurchaseRequestDto dto, string lang = "Uz")
        {
            if (dto.Products == null || dto.Products.Count == 0) return false;

            decimal totalPrice = 0;
            var productEntities = new List<PurchaseRequestProduct>();
            var productNameMap = new Dictionary<Guid, string>();

            // Step 1: Determine total quantity and apply global discount rate
            int totalBoxCount = dto.Products.Sum(p => p.Quantity);
            int discountPercent = totalBoxCount switch
            {
                1 => 0,
                2 => 15,
                3 or 4 => 45,
                >= 5 => 52,
                _ => 0
            };

            foreach (var prod in dto.Products)
            {
                // Fetch product DTO in requested language
                var product = await _productService.GetByIdAsync(prod.ProductId, lang);
                if (product == null) continue;

                decimal basePrice = Math.Abs(product.Price);
                decimal discountedUnitPrice = Math.Round(basePrice * (1 - discountPercent / 100m), 0);

                decimal itemTotal = discountedUnitPrice * prod.Quantity;
                totalPrice += itemTotal;

                productEntities.Add(new PurchaseRequestProduct
                {
                    ProductId = prod.ProductId,
                    Quantity = prod.Quantity,
                    DiscountedPrice = discountedUnitPrice
                });

                productNameMap[prod.ProductId] = product.Name ?? "Noma'lum mahsulot";
            }

            var request = new PurchaseRequest
            {
                BuyerName = dto.BuyerName,
                Region = dto.Region,
                Phone = dto.Phone,
                Comment = dto.Comment,
                TotalPrice = totalPrice,
                Products = productEntities
            };

            var saved = await _repo.AddPurchaseRequestEntityAsync(request);
            if (!saved) return false;

            // Telegram notification
            try
            {
                var botToken = _config["Telegram:BotToken"];
                var groupChatId = _config["Telegram:GroupChatId"];
                var botClient = new TelegramBotClient(botToken);

                string message = $"📝 Yangi so‘rov saytdan\n" +
                                 $"🧍 Ism: {dto.BuyerName}\n" +
                                 $"📞 Telefon: {dto.Phone}\n" +
                                 $"🌍 Hudud: {dto.Region}\n" +
                                 $"💬 Izoh: {(string.IsNullOrWhiteSpace(dto.Comment) ? "Yo‘q" : dto.Comment)}\n\n" +
                                 $"🛍️ Mahsulotlar (Umumiy chegirma: {discountPercent}%):\n";

                int index = 1;
                foreach (var item in productEntities)
                {
                    var prodName = productNameMap.TryGetValue(item.ProductId, out var name) ? name : "Noma'lum mahsulot";
                    var total = item.Quantity * item.DiscountedPrice;
                    message += $"{index++}. {prodName} — {item.Quantity} dona — {total:N0} so'm\n";
                }

                message += $"\n💰 Umumiy narx: {totalPrice:N0} so'm";

                await botClient.SendTextMessageAsync(
                    chatId: groupChatId,
                    text: message,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Telegram Error] {ex.Message}");
                return true; // Ignore telegram errors for user flow
            }

            return true;
        }

        public async Task<IEnumerable<PurchaseRequestDto>> GetAllPurchaseRequestsAsync()
        {
            var entities = await _repo.GetAllPurchaseRequestsAsync();
            return entities.Select(pr => new PurchaseRequestDto
            {
                BuyerName = pr.BuyerName,
                Region = pr.Region,
                Phone = pr.Phone,
                Comment = pr.Comment,
                Products = pr.Products.Select(p => new PurchaseProductDto
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList()
            });
        }

        public async Task<IEnumerable<SiteStatisticDto>> GetSiteStatisticsAsync()
        {
            var stats = await _repo.GetSiteStatisticsAsync();
            return stats.Select(s => new SiteStatisticDto
            {
                Date = s.Date,
                TotalVisits = s.TotalVisits
            });
        }
    }
}
