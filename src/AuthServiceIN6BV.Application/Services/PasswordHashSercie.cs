using AuthServiceIN6BV.Application.Interfaces;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;
using System.Text;

namespace AuthServiceIN6BV.Application.Services;

public class PasswordHasServices : IPasswordHashService
{
    private const int SaltSize = 16; // 128 bit
    private const int HashSize = 32; // 256 bit
    private const int Iterations = 2; // Number of iterations
    private const int Memory = 102400; // 100 MB
    private const int Parallelism = 8; // Number of threads
    public string HashPassword(object password)
    {
        //Encriptacion de la contraseña usando Argon2id
        var salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        var passwordString = password?.ToString() ?? string.Empty;
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(passwordString))
        {
            Salt = salt,
            DegreeOfParallelism = Parallelism,
            Iterations = Iterations,
            MemorySize = Memory
        };

        //Pasandole el tamaño del hash
        var hash = argon2.GetBytes(HashSize);

        //Hacer un casteo para que sea conpatible con Node.js
        var saltBase64 = Convert.ToBase64String(salt);
        var hashBase64 = Convert.ToBase64String(hash);

        //retornamos el formato que usa argon2id en Node.js
        return $"$argon2id$v=19$m={Memory},t={Iterations},p={Parallelism}${saltBase64}${hashBase64}";    
        }

    //Verificacion de la contraseña en formato legacy
     public bool VerifyPassword(string password, string hashedPassword)
    {
        //Metodo para verificar contraseñas en formato legacy
        try
        {
            Console.WriteLine($"[DEBUG] Verifying password for hash: {hashedPassword.Substring(0, Math.Min(50, hashedPassword.Length))}...");
            if (hashedPassword.StartsWith("$argon2id$"))
            {
                Console.WriteLine($"[DEBUG] Using Argon2id standardfor verification.");
                var results = VerifyArgon2StandardFormat(password, hashedPassword);
                Console.WriteLine($"[DEBUG] Verification result: {results}");
                return results;
            }
            else
            {
                Console.WriteLine($"[DEBUG] Using Legacy format verification.");
                return VerifyLegacyFormat(password, hashedPassword);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG] Exception in VerifyPassword: {ex.Message}");
            return false;
        }
    }

    private bool VerifyArgon2StandardFormat(string password, string hashedPassword)
    {
        //Verificacion del formato estandar de Argon2id

        try
        {
            var argon2Verifier = new Argon2id(Encoding.UTF8.GetBytes(password));
            var parts = hashedPassword.Split('$');
            if(parts.Length != 6) return false;

            //Extraer los parametros del hash
            var paramsPart = parts[3];

            //Extraer la sal y el hash
            var saltBase64 = parts[4];
            var hashBase64 = parts[5];

            //hacer un parseo a int para los parametros
            var parameters = paramsPart.Split(',');
            var memory = int.Parse(parameters[0].Split('=')[1]);
            var iterations = int.Parse(parameters[1].Split('=')[1]);
            var parallelism = int.Parse(parameters[2].Split('=')[1]);


            //convertirlo a base64 standar
            var salt = Convert.FromBase64String(FromBase64UrlSafe(saltBase64));
            var expectedHash = Convert.FromBase64String(FromBase64UrlSafe(hashBase64));

            //Configurar el verificador Argon2id
    
            argon2Verifier.Salt = salt;
            argon2Verifier.DegreeOfParallelism = parallelism;
            argon2Verifier.Iterations = iterations;
            argon2Verifier.MemorySize = memory;

            //Crear otra varianle para obtener el hash
            var computedHash = argon2Verifier.GetBytes(expectedHash.Length);
            //Comparar los hashes vieja y nueva
            return expectedHash.SequenceEqual(computedHash);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error verifying Argon2id standard format: {ex.Message}");
        
        
            return false;
        }
    }

    private bool VerifyLegacyFormat(string password, string hashedPassword)
    {
        //casteo de la de contraseña hasheada en formato legacy
        var hashBytes = Convert.FromBase64String(hashedPassword);
        var salt = new byte[SaltSize];
        var hash = new byte[HashSize];

        //hacer una copia inciando en el origen Destino, origen, longitud
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);
        Array.Copy(hashBytes, SaltSize, hash, 0, HashSize);

        //Configurar Argon2id para verificar
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = Parallelism,
            Iterations = Iterations,
            MemorySize = Memory
        };

        //Obtener el hash de la contraseña proporcionada
        var computedHash = argon2.GetBytes(HashSize);

        //Comparar los hashes y retornar la verificacion del computerd hash con el hash almacenado
        return hash.SequenceEqual(computedHash);

    }

    //PAsar de base 64 URL Safe a Base64 estandar para ser legible por .NET
    private static string FromBase64UrlSafe(string base64Url)
    {
        //Convertir Base64 URL Safe a Base64 estandar
        string base64 = base64Url.Replace('-', '+').Replace('_', '/');

        // Agregar padding si es necesario
        switch (base64.Length % 4)
        {
            case 2:
                 base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }
        //Retornar el base64 estandar
        return base64;
    }

    public string HashPassword(string password)
    {
        throw new NotImplementedException();
    }
}