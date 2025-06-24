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

            var prompt = $"Quyidagi Nutva kompaniyasiga oid hujjat asosida savolga javob bering:\n\n{context}\n\nSavol: {request.Question}\n\nIltimos, javobni o'zbek tilida bering.";

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

            // ✅ Fallback if no usable response
            if (string.IsNullOrWhiteSpace(answer) || answer.Trim().Length < 10)
            {
                answer = "Uzur, lekin men faqat Nutva kompaniyasi haqida ma'lumotga egaman.";
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
