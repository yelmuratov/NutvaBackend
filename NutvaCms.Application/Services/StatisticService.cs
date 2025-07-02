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
            var productPriceMap = new Dictionary<Guid, decimal>();

            foreach (var prod in dto.Products)
            {
                var product = await _productService.GetByIdAsync(prod.ProductId);
                if (product == null) continue;

                var boxDiscount = await _boxPriceService.GetByProductAndBoxCountAsync(prod.ProductId, prod.Quantity);
                string discountLabel = boxDiscount?.DiscountLabel ?? "0%";

                decimal basePrice = Math.Abs(product.Price);
                decimal discountedUnitPrice = CalculateDiscountedPrice(basePrice, discountLabel);
                decimal itemTotal = discountedUnitPrice * prod.Quantity;

                totalPrice += itemTotal;

                productEntities.Add(new PurchaseRequestProduct
                {
                    ProductId = prod.ProductId,
                    Quantity = prod.Quantity,
                    DiscountedPrice = discountedUnitPrice
                });

                productPriceMap[prod.ProductId] = basePrice;

                string prodName = lang switch
                {
                    "Uz" => product.Uz?.Name ?? "Noma'lum mahsulot",
                    "Ru" => product.Ru?.Name ?? "Неизвестный продукт",
                    "En" => product.En?.Name ?? "Unknown product",
                    _ => product.Uz?.Name ?? "Noma'lum mahsulot"
                };
                productNameMap[prod.ProductId] = prodName;
            }

            var request = new PurchaseRequest
            {
                BuyerName = dto.BuyerName,
                Age = dto.Age,
                ForWhom = dto.ForWhom,
                Problem = dto.Problem,
                Region = dto.Region,
                Phone = dto.Phone,
                Comment = dto.Comment,
                TotalPrice = totalPrice,
                Products = productEntities
            };

            var saved = await _repo.AddPurchaseRequestEntityAsync(request);
            if (!saved) return false;

            // ✅ Telegram notification
            try
            {
                var botToken = _config["Telegram:BotToken"];
                var groupChatId = _config["Telegram:GroupChatId"];
                var botClient = new TelegramBotClient(botToken);

                string message = $"📝 Yangi so‘rov saytdan\n" +
                                 $"🧍 Ism: {dto.BuyerName}\n" +
                                 $"📞 Telefon: {dto.Phone}\n" +
                                 $"🎂 Yosh: {dto.Age}\n" +
                                 $"🌍 Hudud: {dto.Region}\n" +
                                 $"👥 Kim uchun: {dto.ForWhom}\n" +
                                 $"🧠 Muammo: {dto.Problem}\n" +
                                 $"💬 Izoh: {(string.IsNullOrWhiteSpace(dto.Comment) ? "Yo‘q" : dto.Comment)}\n\n" +
                                 $"🛍️ Mahsulotlar:\n";

                int index = 1;
                foreach (var item in productEntities)
                {
                    var prodName = productNameMap.TryGetValue(item.ProductId, out var name) ? name : "Noma'lum mahsulot";
                    var total = item.Quantity * item.DiscountedPrice;

                    // Calculate % discount (if any)
                    var basePrice = productPriceMap.TryGetValue(item.ProductId, out var originalPrice) ? originalPrice : item.DiscountedPrice;
                    var discountPercent = basePrice > 0 ? Math.Round(100 - (item.DiscountedPrice / basePrice) * 100) : 0;
                    string discountNote = discountPercent > 0 ? $" ({discountPercent}%)" : "";

                    message += $"{index++}. {prodName} — {item.Quantity} dona — {total:N0} so'm{discountNote}\n";
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
                return true;
            }

            return true;
        }


        public async Task<IEnumerable<PurchaseRequestDto>> GetAllPurchaseRequestsAsync()
        {
            var entities = await _repo.GetAllPurchaseRequestsAsync();
            return entities.Select(pr => new PurchaseRequestDto
            {
                BuyerName = pr.BuyerName,
                Age = pr.Age,
                ForWhom = pr.ForWhom,
                Problem = pr.Problem,
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
