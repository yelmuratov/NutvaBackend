namespace NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;

public interface IContactFormRepository
{
    Task AddAsync(ContactForm entity);
    Task<IEnumerable<ContactForm>> GetAllAsync();
}
