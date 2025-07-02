using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using System.Text.RegularExpressions;

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
            IProductBoxPriceService boxPriceService
        )
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

            var percentMatch = Regex.Match(discountLabel, @"\d+");
            if (!percentMatch.Success)
                return productPrice;

            var percent = decimal.Parse(percentMatch.Value);
            var discountedPrice = productPrice * (1 - percent / 100m);
            return Math.Round(discountedPrice, 0);
        }

        private int GetDiscountPercent(string discountLabel)
        {
            if (string.IsNullOrWhiteSpace(discountLabel)) return 0;
            var match = Regex.Match(discountLabel, @"\d+");
            return match.Success ? int.Parse(match.Value) : 0;
        }

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

                // Get product names in selected language
                var productDict = new Dictionary<Guid, string>();
                foreach (var prod in dto.Products)
                {
                    var product = await _productService.GetByIdAsync(prod.ProductId);
                    if (product != null)
                    {
                        string prodName = lang switch
                        {
                            "Uz" => product.Uz?.Name ?? "Noma'lum mahsulot",
                            "Ru" => product.Ru?.Name ?? "Неведомый продукт",
                            "En" => product.En?.Name ?? "Unknown product",
                            _ => product.Uz?.Name ?? "Noma'lum mahsulot"
                        };
                        productDict[prod.ProductId] = prodName;
                    }
                    else
                    {
                        productDict[prod.ProductId] = "Noma'lum mahsulot";
                    }
                }

                // Build Telegram message
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
                decimal totalPrice = 0;

                foreach (var prod in dto.Products)
                {
                    var prodName = productDict.TryGetValue(prod.ProductId, out var name) ? name : "Noma'lum mahsulot";
                    var product = await _productService.GetByIdAsync(prod.ProductId);

                    if (product != null)
                    {
                        var boxDiscount = await _boxPriceService.GetByProductAndBoxCountAsync(prod.ProductId, prod.Quantity);
                        string discountLabel = boxDiscount?.DiscountLabel ?? "0%";

                        decimal basePrice = Math.Abs(product.Price);
                        decimal discountedUnit = CalculateDiscountedPrice(basePrice, discountLabel);
                        decimal itemTotal = discountedUnit * prod.Quantity;
                        totalPrice += itemTotal;

                        string discountNote = discountLabel != "0%" ? $" ({discountLabel} chegirma)" : "";

                        message += $"{index++}. {prodName} — {prod.Quantity} dona — {itemTotal:N0} so'm{discountNote}\n";
                    }
                    else
                    {
                        message += $"{index++}. {prodName} — {prod.Quantity} dona — narx topilmadi\n";
                    }
                }

                message += $"\n💰 Umumiy narx: {totalPrice:N0} so'm\n\n";

                await botClient.SendTextMessageAsync(
                    chatId: groupChatId,
                    text: message,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Telegram Error] {ex.Message}");
                return true; // still return true, because DB insert succeeded
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
                Products = pr.Products.Select(pp => new PurchaseProductDto
                {
                    ProductId = pp.ProductId,
                    Quantity = pp.Quantity
                }).ToList()
            });
        }

        public async Task<IEnumerable<SiteStatisticDto>> GetSiteStatisticsAsync()
        {
            var entities = await _repo.GetSiteStatisticsAsync();
            return entities.Select(s => new SiteStatisticDto
            {
                Date = s.Date,
                TotalVisits = s.TotalVisits
            });
        }
    }
}
