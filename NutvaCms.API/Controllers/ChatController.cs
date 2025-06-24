using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NutvaCms.API.Settings;
using NutvaCms.API.Services;
using NutvaCms.API.Domain.Entities;
using System.Net.Http.Json;
using System.Text.Json;

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
            var question = request.Question?.Trim().ToLower();

            // ✅ Identity check override
            var identityQuestions = new[]
            {
            "sen kimsan",
            "siz kimsiz",
            "isming nima",
            "sen kim",
            "siz kim",
            "bu nima",
            "bu kim",
            "siz kimsiz?",
            "sen kimsan?"
        };

            if (identityQuestions.Contains(question))
            {
                return Ok(new
                {
                    success = true,
                    answer = "Men Nutva kompaniyasi uchun yaratilgan sun'iy intellekt chat botman."
                });
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

            var prompt = $"""
            Siz Nutva kompaniyasining rasmiy sun'iy intellekt yordamchisiz. Siz faqat Nutva haqida ma'lumot bera olasiz va hech qachon boshqa texnologiyalar (masalan, Gemini, Google, ChatGPT) haqida o'zingizni tanishtirmaysiz.

            Quyidagi hujjat asosida foydalanuvchining savoliga aniq, do'stona va qisqacha javob bering:

            {context}

            Savol: {request.Question}

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

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized(new
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
                answer = "Uzr, men faqat Nutva kompaniyasi haqida ma'lumot bera olaman.";
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
}
