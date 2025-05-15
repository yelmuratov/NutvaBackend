using Microsoft.AspNetCore.Http;


namespace NutvaCms.Application.Interfaces;

public interface IFileService
{
    Task<string?> UploadSingleAsync(IFormFile file);
    Task<List<string>> UploadManyAsync(List<IFormFile> files);
    Task<bool> DeleteManyAsync(string fileName);
}

