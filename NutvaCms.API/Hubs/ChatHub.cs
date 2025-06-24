using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using NutvaCms.API.Services;
using NutvaCms.API.Settings;
using System.Text.Json;
using System.Net.Http.Json;

namespace NutvaCms.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DocxReaderService _docReader;
        private readonly GeminiSettings _geminiSettings;

        public ChatHub(
            IHttpClientFactory httpClientFactory,
            DocxReaderService docReader,
            IOptions<GeminiSettings> geminiOptions)
        {
            _httpClientFactory = httpClientFactory;
            _docReader = docReader;
            _geminiSettings = geminiOptions.Value;
        }

        public async Task AskQuestion(string question)
        {
            var normalized = question?.Trim().ToLower();
            var identityQuestions = new[]
            {
                "sen kimsan", "siz kimsiz", "isming nima",
                "sen kim", "siz kim", "bu nima", "bu kim",
                "siz kimsiz?", "sen kimsan?"
            };

            string answer;

            if (identityQuestions.Contains(normalized))
            {
                answer = "Men Nutva kompaniyasi uchun yaratilgan sun'iy intellekt chat botman.";
                await Clients.Caller.SendAsync("ReceiveAnswer", answer);
                return;
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "docs", "company-products.docx");

            string context = System.IO.File.Exists(filePath)
                ? _docReader.ReadDocxText(filePath)
                : string.Empty;

            var prompt = $"""
                Siz Nutva kompaniyasining rasmiy sun'iy intellekt yordamchisiz. Siz faqat Nutva haqida ma'lumot bera olasiz va hech qachon boshqa texnologiyalar (masalan, Gemini, Google, ChatGPT) haqida o'zingizni tanishtirmaysiz.

                Quyidagi hujjat asosida foydalanuvchining savoliga aniq, do'stona va qisqacha javob bering:

                {context}

                Savol: {question}

                Javobni o‘zbek tilida yozing. Agar savol Nutva bilan bog‘liq bo‘lmasa, quyidagicha javob bering:
                "Uzr, men faqat Nutva kompaniyasi haqida ma'lumot bera olaman."
            """;

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var client = _httpClientFactory.CreateClient();
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_geminiSettings.ApiKey}";
            var response = await client.PostAsJsonAsync(url, payload);
            var json = await response.Content.ReadAsStringAsync();

            answer = "Uzr, men faqat Nutva kompaniyasi haqida ma'lumot bera olaman.";

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    using var doc = JsonDocument.Parse(json);
                    answer = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();
                }
                catch
                {
                    // Keep fallback answer
                }
            }

            if (string.IsNullOrWhiteSpace(answer) || answer.Trim().Length < 10)
            {
                answer = "Uzr, men faqat Nutva kompaniyasi haqida ma'lumot bera olaman.";
            }

            await Clients.Caller.SendAsync("ReceiveAnswer", answer);
        }
    }
}
