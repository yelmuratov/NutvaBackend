namespace NutvaCms.Application.Interfaces;
using NutvaCms.Application.DTOs;

public interface IContactFormService
{
    Task<bool> SubmitContactFormAsync(ContactFormDto dto);
    Task<IEnumerable<ContactFormDto>> GetAllAsync();
}
