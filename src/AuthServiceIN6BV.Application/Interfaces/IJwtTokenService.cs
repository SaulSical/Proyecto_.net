using AuthServiceIN6BV.Application.InTerface;
using AuthServiceIN6BV.Domain.Entities;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}