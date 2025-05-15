using Microsoft.AspNetCore.Http;
namespace NutvaCms.Application.DTOs;

public class BitrixLeadDto
{
    public string Title { get; set; } = "New Product Purchase";
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Comment { get; set; } = string.Empty;
}


