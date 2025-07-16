using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NutvaCms.API.Settings;
using NutvaCms.API.Services;
using NutvaCms.API.Domain.Entities;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DocxReaderService _docReader;
    private readonly GeminiSettings _geminiSettings;

    public ChatController(
        IHttpClientFactory httpClientFactory,
        DocxReaderService docReader,
        IOptions<GeminiSettings> geminiOptions)
    {
        _httpClientFactory = httpClientFactory;
        _docReader = docReader;
        _geminiSettings = geminiOptions.Value;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] ChatRequest request)
    {
        try
        {
            var question = request.Question?.Trim();
            if (string.IsNullOrWhiteSpace(question))
            {
                return BadRequest(new { success = false, message = "Question cannot be empty" });
            }

            string langCode = DetectLanguage(question);

            var identityResponses = new Dictionary<string[], string>
            {
                {
                    new[] { "sen kimsan", "siz kimsiz", "isming nima", "sen kim", "siz kim", "bu kim" },
                    "Men Nutva kompaniyasi uchun yaratilgan sun'iy intellekt chat botman."
                },
                {
                    new[] { "who are you", "who are u", "what is your name", "what are you" },
                    "I am an AI chatbot created for the Nutva company."
                },
                {
                    new[] { "кто ты", "кто вы", "как тебя зовут", "что ты" },
                    "Я чат-бот с искусственным интеллектом, созданный для компании Nutva."
                }
            };

            foreach (var entry in identityResponses)
            {
                if (entry.Key.Any(q => question.Equals(q, StringComparison.OrdinalIgnoreCase)))
                {
                    return Ok(new { success = true, answer = entry.Value });
                }
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "docs", "company-products.docx");
            if (!System.IO.File.Exists(filePath))
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Could not find file '{filePath}'."
                });
            }

            string context = _docReader.ReadDocxText(filePath);

            string languageName = langCode switch
            {
                "ru" => "rus tilida",
                "en" => "ingliz tilida",
                _ => "o‘zbek tilida"
            };

            var prompt = $"""
Hujjat quyida o‘zbek tilida berilgan. Siz foydalanuvchining savoliga **ushbu hujjat asosida**, lekin **{languageName}** javob yozing.

Foydalanuvchi savolni {languageName} yozgan. Siz ham shunday tilda javob bering. Hech qachon o‘zboshimchalik bilan tilni o‘zgartirmang.

Hujjat matni (O‘zbek tilida):

{context}

Savol: {request.Question}

Javobni {languageName} yozing.

Agar savol Nutva bilan bog‘liq bo‘lmasa, quyidagicha javob bering:
- 🇺🇿 Uzbek: "Uzr, men faqat Nutva kompaniyasi haqida ma'lumot bera olaman."
- 🇷🇺 Russian: "Извините, я могу предоставить информацию только о компании Nutva."
- 🇬🇧 English: "Sorry, I can only provide information about Nutva."

Agar sizdan kimligingiz so‘ралса, quyidagicha javob bering:
- 🇺🇿 "Men Nutva kompaniyasi uchun yaratilgan sun'iy intellekt chat botman."
- 🇷🇺 "Я чат-бот с искусственным интеллектом, созданный для компании Nutva."
- 🇬🇧 "I am an AI chatbot created for the Nutva company."
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

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(502, new
                {
                    success = false,
                    message = "Gemini API failed",
                    detail = json
                });
            }

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var answer = root
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(answer) || answer.Trim().Length < 10)
            {
                answer = langCode switch
                {
                    "ru" => "Извините, я могу предоставить информацию только о компании Nutva.",
                    "en" => "Sorry, I can only provide information about Nutva.",
                    _ => "Uzr, men faqat Nutva kompaniyasi haqida ma'lumot bera olaman."
                };
            }

            return Ok(new
            {
                success = true,
                answer
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Internal server error",
                detail = ex.Message
            });
        }
    }

    private string DetectLanguage(string input)
    {
        input = input.ToLower();

        if (Regex.IsMatch(input, "[а-яё]")) return "ru"; // Cyrillic → assume Russian
        if (input.Contains("who") || input.Contains("what") || input.Contains("you")) return "en"; // English keywords

        return "uz"; // Default: Uzbek
    }
}
