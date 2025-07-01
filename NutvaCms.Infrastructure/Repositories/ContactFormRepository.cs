using NutvaCms.Application.Interfaces;
using NutvaCms.Domain.Entities;
using NutvaCms.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

public class ContactFormRepository : IContactFormRepository
{
    private readonly AppDbContext _context;
    public ContactFormRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ContactForm entity)
    {
        await _context.ContactForms.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ContactForm>> GetAllAsync()
    {
        return await _context.ContactForms
            .OrderByDescending(x => x.SubmittedAt)
            .ToListAsync();
    }
}
