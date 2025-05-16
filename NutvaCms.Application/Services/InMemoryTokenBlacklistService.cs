using System.Collections.Concurrent;
using NutvaCms.Application.Interfaces;

namespace NutvaCms.Application.Services;

public class InMemoryTokenBlacklistService : ITokenBlacklistService
{
    private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();

    public void BlacklistToken(string token, DateTime expiration)
    {
        _blacklistedTokens.TryAdd(token, expiration);
    }

    public bool IsTokenBlacklisted(string token)
    {
        return _blacklistedTokens.TryGetValue(token, out var expires) && expires > DateTime.UtcNow;
    }
}

