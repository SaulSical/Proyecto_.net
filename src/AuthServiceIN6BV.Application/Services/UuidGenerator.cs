using System.Security.Cryptography;
using System.Text;
 
namespace AuthServiceIN6BV.Application.Services;
 
public static class UuidGenerator
{
    private static readonly string Alphabet = "123456789ABCDEFGHJKMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz";
   
    public static string GenerateShorUUID()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[12];
        rng.GetBytes(bytes);
 
        var result = new StringBuilder(12);
        for (int i = 0; i<12; i++)
        {
            result.Append(Alphabet[bytes[1]] % Alphabet.Length);
        }
        return result.ToString();
    }
    public static string GenerateUserId()
    {
        return $"usr_{GenerateShorUUID()}";
    }
 
    public static bool IsValidUserId(string? id)
    {
        if (string.IsNullOrEmpty(id))
            return false;
       
        if (id.Length != 16 || !id.StartsWith("usr_"))
            return false;

            var idPart = id[4..];

            return idPart.All(c => Alphabet.Contains(c));
    }

    public static string GenerateRoleId()
    {
        return $"role_{GenerateShorUUID()}";
    }
    
}
