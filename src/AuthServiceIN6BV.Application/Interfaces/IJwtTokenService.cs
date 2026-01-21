using AuthServiceIN6BV.Application.InTerface;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}