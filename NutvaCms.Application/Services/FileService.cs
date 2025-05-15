using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NutvaCms.Application.Interfaces;

namespace NutvaCms.Application.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _http;

    private static readonly string[] AllowedImageTypes = new[]
    {
        "image/jpeg", "image/png", "image/webp", "image/gif"
    };

    public FileService(IWebHostEnvironment env, IHttpContextAccessor http)
    {
        _env = env;
        _http = http;
    }

    public async Task<string?> UploadSingleAsync(IFormFile file)
    {
        if (!IsValidImage(file))
            return null;

        if (string.IsNullOrEmpty(_env.WebRootPath))
            throw new InvalidOperationException("WebRootPath is null. Did you forget app.UseStaticFiles()?");

        var folderPath = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(folderPath);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(folderPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var baseUrl = $"{_http.HttpContext!.Request.Scheme}://{_http.HttpContext.Request.Host}";
        return $"{baseUrl}/uploads/{fileName}";
    }

    public async Task<List<string>> UploadManyAsync(List<IFormFile> files)
    {
        var result = new List<string>();

        foreach (var file in files)
        {
            var url = await UploadSingleAsync(file);
            if (url != null)
                result.Add(url);
        }

        return result;
    }

    private bool IsValidImage(IFormFile file)
    {
        return file != null &&
               file.Length > 0 &&
               AllowedImageTypes.Contains(file.ContentType.ToLower());
    }

    public async Task<bool> DeleteManyAsync(string fileName)
    {
        if (string.IsNullOrEmpty(_env.WebRootPath))
            throw new InvalidOperationException("WebRootPath is null. Did you forget app.UseStaticFiles()?");

        var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
        if (!File.Exists(filePath))
            return false;

        await Task.Run(() => File.Delete(filePath));
        return true;
    }
}
