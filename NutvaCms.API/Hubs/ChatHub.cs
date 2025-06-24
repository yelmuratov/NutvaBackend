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
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "docs", "company-products.docx");
            string context = System.IO.File.Exists(filePath)
                ? _docReader.ReadDocxText(filePath)
                : string.Empty;

            var prompt = $"Quyidagi Nutva kompaniyasiga oid hujjat asosida savolga javob bering:\n\n{context}\n\nSavol: {question}\n\nIltimos, javobni o'zbek tilida bering.";

            var payload = new
            {
                contents = new[]
                {
                    new {
                        parts = new[] { new { text = prompt } }
                    }
                }
            };

            var client = _httpClientFactory.CreateClient();
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_geminiSettings.ApiKey}";

            var response = await client.PostAsJsonAsync(url, payload);
            var json = await response.Content.ReadAsStringAsync();

            string answer = "Men faqat Nutva kompaniyasi haqida ma'lumotga egaman.";

            if (response.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(json);
                answer = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();
            }

            await Clients.Caller.SendAsync("ReceiveAnswer", answer);
        }
    }
}
