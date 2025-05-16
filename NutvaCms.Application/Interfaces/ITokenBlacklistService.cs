
namespace NutvaCms.Application.Interfaces;

public interface ITokenBlacklistService
{
    void BlacklistToken(string token, DateTime expiration);
    bool IsTokenBlacklisted(string token);
}

