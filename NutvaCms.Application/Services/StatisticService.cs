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
            var productDiscounts = new Dictionary<string, decimal[]>()
    {
        // ProductName or type : discounts for [1,2,3-4,>=5]
        { "Virus",    new decimal[] { 0m, 19.77m, 43.02m, 54.65m } },
        { "Fertilia", new decimal[] { 0m, 19.77m, 43.02m, 54.65m } },
        { "Gelmin",   new decimal[] { 0m, 20.41m, 40.82m, 55.10m } },
        { "Complex",  new decimal[] { 0m, 15.38m, 45.30m, 52.14m } },
        { "Extra",    new decimal[] { 0m, 15.38m, 45.30m, 52.14m } },
        // Add aliases if your names are not exact
        { "Complex Extra", new decimal[] { 0m, 15.38m, 45.30m, 52.14m } },
    };

            // Helper to get discount percent for product
            decimal GetDiscount(string name, int quantity)
            {
                // Normalize name
                name = name?.ToLowerInvariant() ?? "";
                string key = productDiscounts.Keys.FirstOrDefault(k => name.Contains(k.ToLowerInvariant()));
                if (string.IsNullOrEmpty(key)) return 0m;

                var discounts = productDiscounts[key];
                if (quantity == 1) return discounts[0];
                if (quantity == 2) return discounts[1];
                if (quantity == 3 || quantity == 4) return discounts[2];
                if (quantity >= 5) return discounts[3];
                return 0m;
            }

            foreach (var prod in dto.Products)
            {
                // Fetch product DTO in requested language
                var product = await _productService.GetByIdAsync(prod.ProductId, lang);
                if (product == null) continue;

                decimal basePrice = Math.Abs(product.Price);
                string productName = product.Name ?? "Noma'lum mahsulot";
                decimal discountPercent = GetDiscount(productName, prod.Quantity);

                // Calculate discounted price (round as needed for your use-case)
                decimal discountedUnitPrice = Math.Round(basePrice * (1 - discountPercent / 100m), 0); // or remove Math.Round if you want decimals

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
