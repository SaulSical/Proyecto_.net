using AuthServiceIN6BV.Application.Interfaces;
using AuthServiceIN6BV.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthServiceIN6BV.Application.Services;

/// <summary>
/// Servicio encargado de generar tokens JWT para autenticación y autorización.
/// </summary>
/// <param name="configuration">
/// IConfiguration se usa para leer los valores de configuración
/// como la clave secreta, issuer, audience y tiempo de expiración.
/// </param>
public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    /// <summary>
    /// Genera un token JWT para un usuario autenticado.
    /// </summary>
    /// <param name="user">Usuario autenticado del sistema</param>
    /// <returns>Token JWT en formato string</returns>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si la clave secreta no está configurada.
    /// </exception>
    public string GenerateToken(User user)
    {
        // Obtiene la sección JwtSettings del archivo appsettings.json
        var jwtSettings = configuration.GetSection("JwtSettings");

        // Clave secreta usada para firmar el token (obligatoria)
        var secretKey = jwtSettings["SecretKey"] 
            ?? throw new InvalidOperationException("JWT SecretKey not configured");

        // Emisor del token (quién lo genera)
        var issuer = jwtSettings["Issuer"] ?? "AuthDotnet";

        // Audiencia del token (para quién es válido)
        var audience = jwtSettings["Audience"] ?? "AuthDotnet";

        // Tiempo de expiración del token en minutos
        var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"] ?? "30");

        // Se crea la clave de seguridad usando la clave secreta
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        // Se definen las credenciales de firmado usando HMAC SHA256
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Obtiene el rol del usuario (asumiendo que tiene un solo rol)
        // Si no tiene rol asignado, se usa uno por defecto
        var role = user.UserRoles?.FirstOrDefault()?.Role?.Name ?? "USER_ROLE";

        // Claims: información que viajará dentro del token
        var claims = new[]
        {
            // Identificador único del usuario
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),

            // Identificador único del token
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // Fecha de creación del token (en formato Unix)
            new Claim(
                JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64
            ),

            // Rol del usuario (para autorización)
            new Claim("role", role)
        };

        // Se construye el token JWT
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
            signingCredentials: credentials
        );

        // Se retorna el token serializado como string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
