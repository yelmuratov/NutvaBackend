using NutvaCms.Application.DTOs;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Application.Interfaces;

public interface IBitrixService
{
    Task SendLeadAsync(BitrixLeadDto dto);
}

