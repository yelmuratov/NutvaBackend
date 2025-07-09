using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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

        // Discount table matches frontend
        private readonly Dictionary<string, Dictionary<int, decimal>> DiscountTable = new()
        {
            ["VIRIS"]     = new() { { 1, 860_000 }, { 2, 690_000 }, { 3, 490_000 }, { 5, 390_000 } },
            ["FERTILIA"]  = new() { { 1, 860_000 }, { 2, 690_000 }, { 3, 490_000 }, { 5, 390_000 } },
            ["GELMIN"]    = new() { { 1, 490_000 }, { 2, 390_000 }, { 3, 290_000 }, { 5, 220_000 } },
            ["COMPLEX"]   = new() { { 1, 1_170_000 }, { 2, 990_000 }, { 3, 640_000 }, { 5, 560_000 } },
            ["EXTRA"]     = new() { { 1, 1_170_000 }, { 2, 990_000 }, { 3, 640_000 }, { 5, 560_000 } },
        };

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

        // Helper: Map product name to table key
        private string GetDiscountKey(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName)) return null;
            productName = productName.ToUpperInvariant();
            if (productName.Contains("VIRIS")) return "VIRIS";
            if (productName.Contains("FERTILIA")) return "FERTILIA";
            if (productName.Contains("GELMIN")) return "GELMIN";
            if (productName.Contains("COMPLEX") && productName.Contains("EXTRA")) return "EXTRA"; // Complex Extra or Extra
            if (productName.Contains("EXTRA")) return "EXTRA";
            if (productName.Contains("COMPLEX")) return "COMPLEX";
            return null;
        }

        // Helper: Given product key and total box count, return price
        private decimal GetDiscountedPrice(string productKey, int totalBoxes)
        {
            if (string.IsNullOrEmpty(productKey) || !DiscountTable.TryGetValue(productKey, out var priceMap))
                return 0;
            // Discount logic: 1->1, 2->2, 3/4->3, 5+->5
            if (totalBoxes <= 1 && priceMap.ContainsKey(1)) return priceMap[1];
            if (totalBoxes == 2 && priceMap.ContainsKey(2)) return priceMap[2];
            if ((totalBoxes == 3 || totalBoxes == 4) && priceMap.ContainsKey(3)) return priceMap[3];
            if (totalBoxes >= 5 && priceMap.ContainsKey(5)) return priceMap[5];
            // fallback (should never hit if table covers all cases)
            return priceMap.OrderBy(p => p.Key).First().Value;
        }

        public async Task<bool> AddPurchaseRequestAsync(PurchaseRequestDto dto, string lang = "Uz")
        {
            if (dto.Products == null || dto.Products.Count == 0) return false;

            // 1. Calculate total box count
            int totalBoxCount = dto.Products.Sum(p => p.Quantity);

            decimal totalPrice = 0;
            var productEntities = new List<PurchaseRequestProduct>();
            var productNameMap = new Dictionary<Guid, string>();

            foreach (var prod in dto.Products)
            {
                var product = await _productService.GetByIdAsync(prod.ProductId, lang);
                if (product == null) continue;

                string productName = product.Name ?? "Noma'lum mahsulot";
                string key = GetDiscountKey(productName);

                decimal discountedUnitPrice = GetDiscountedPrice(key, totalBoxCount);

                decimal itemTotal = discountedUnitPrice * prod.Quantity;
                totalPrice += itemTotal;

                productEntities.Add(new PurchaseRequestProduct
                {
                    ProductId = prod.ProductId,
                    Quantity = prod.Quantity,
                    DiscountedPrice = discountedUnitPrice
                });

                productNameMap[prod.ProductId] = productName;
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
                                 $"🛍️ Mahsulotlar:\n";

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
