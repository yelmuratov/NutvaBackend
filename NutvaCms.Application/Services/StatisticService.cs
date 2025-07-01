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

                // Product names by lang
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
                            _ => product.Uz.Name
                        };
                        productDict[prod.ProductId] = prodName;
                    }
                    else
                    {
                        productDict[prod.ProductId] = "Noma'lum mahsulot";
                    }
                }

                // Main part: products, prices, discounts
                string productsText = "\n🛍️ *Buyurtmadagi mahsulotlar:*";
                decimal allTotal = 0;
                decimal allDiscount = 0;
                int i = 1;

                foreach (var prod in dto.Products)
                {
                    var prodName = productDict.TryGetValue(prod.ProductId, out var name) ? name : "Noma'lum mahsulot";
                    var product = await _productService.GetByIdAsync(prod.ProductId);

                    if (product != null)
                    {
                        // 1. Get discount for box count
                        var boxDiscount = await _boxPriceService.GetByProductAndBoxCountAsync(prod.ProductId, prod.Quantity);
                        string discountLabel = boxDiscount?.DiscountLabel ?? "0%";
                        int discountPercent = GetDiscountPercent(discountLabel);

                        // 2. Calculate prices
                        decimal basePrice = product.Price;
                        decimal unitDiscounted = CalculateDiscountedPrice(basePrice, discountLabel);
                        decimal productTotal = unitDiscounted * prod.Quantity;
                        decimal productDiscountAmount = (basePrice - unitDiscounted) * prod.Quantity;

                        // 3. Add to message
                        productsText += $"\n  {i++}. {prodName}\n" +
                            $"     Soni: {prod.Quantity} dona\n" +
                            $"     Narx (1 dona): {basePrice:N0} so'm\n" +
                            $"     Chegirma: {discountPercent}%\n" +
                            $"     Yakuniy narx (1 dona): {unitDiscounted:N0} so'm\n" +
                            $"     Jami: {productTotal:N0} so'm\n" +
                            $"     Chegirma miqdori: {productDiscountAmount:N0} so'm";

                        allTotal += productTotal;
                        allDiscount += productDiscountAmount;
                    }
                    else
                    {
                        productsText += $"\n  {i++}. {prodName} - {prod.Quantity} dona - Noma'lum narx";
                    }
                }

                productsText += $"\n\n💵 *Umumiy jami:* {allTotal:N0} so'm";
                productsText += $"\n💸 *Umumiy chegirma:* {allDiscount:N0} so'm";

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
