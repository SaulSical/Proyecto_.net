using AuthServiceIN6BV.Application.Interfaces;
using AuthServiceIN6BV.Domain.Entities;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}