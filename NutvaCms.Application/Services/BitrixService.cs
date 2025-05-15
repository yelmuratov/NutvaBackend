using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NutvaCms.Application.DTOs;
using NutvaCms.Application.Interfaces;
using System.Net.Http.Json;


namespace NutvaCms.Application.Services;

public class BitrixService : IBitrixService
{
    private readonly HttpClient _http;

    public BitrixService(HttpClient http)
    {
        _http = http;
    }

    public async Task SendLeadAsync(BitrixLeadDto dto)
    {
        var url = "https://b24-q1rol7.bitrix24.ru/rest/1/ck9aihp0mu0hwty3/crm.lead.add.json";

        var payload = new
        {
            fields = new
            {
                TITLE = "New Product Purchase",
                NAME = dto.Name, // appears under contact, not card title
                PHONE = new[] {
                    new { VALUE = dto.Phone, VALUE_TYPE = "WORK" }
                },
                COMMENTS = dto.Comment ?? "No comment provided"
            }
        };

        await _http.PostAsJsonAsync(url, payload);
    }
}

