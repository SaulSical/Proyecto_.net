using AuthServiceIN6BV.Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace AuthServiceIN6BV.Application.Services;

/// <summary>
/// Servicio encargado de gestionar imágenes en Cloudinary:
/// subir, eliminar y construir URLs de acceso.
/// </summary>
/// <param name="configuration">
/// Permite acceder a las credenciales y configuraciones
/// definidas en appsettings.json.
/// </param>
public class CloudinaryService(IConfiguration configuration) : ICloudinaryService
{
    /// <summary>
    /// Instancia del cliente Cloudinary configurada con las credenciales
    /// necesarias para conectarse a la cuenta.
    /// </summary>
    private readonly Cloudinary _cloudinary = new(new Account(
        configuration["CloudinarySettings:CloudName"],
        configuration["CloudinarySettings:ApiKey"],
        configuration["CloudinarySettings:ApiSecret"]
    ));

    /// <summary>
    /// Sube una imagen a Cloudinary de forma asíncrona.
    /// </summary>
    /// <param name="imageFile">Archivo de imagen en memoria</param>
    /// <param name="fileName">Nombre original del archivo</param>
    /// <returns>Ruta relativa de la imagen subida</returns>
    public async Task<string> UploadImageAsync(IFileData imageFile, string fileName)
    {
        try
        {
            // Convierte los bytes de la imagen en un Stream
            using var stream = new MemoryStream(imageFile.Data);

            // Carpeta donde se guardarán las imágenes en Cloudinary
            var folder = configuration["CloudinarySettings:Folder"]
                         ?? "auth_service/profiles";

            // Limpia el nombre del archivo quitando la extensión
            var cleanName = Path.GetFileNameWithoutExtension(fileName);

            // PublicId define la ruta + nombre del archivo en Cloudinary
            var publicId = $"{folder}/{cleanName}";

            // Parámetros de subida de la imagen
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imageFile.FileName, stream),
                PublicId = publicId,
            };

            // Subida asíncrona de la imagen
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            // Verifica si Cloudinary devolvió un error
            if (uploadResult.Error != null)
                throw new InvalidOperationException(
                    $"Error uploading image: {uploadResult.Error.Message}"
                );

            // Retorna la ruta relativa de la imagen subida
            return $"v{uploadResult.Version}/{uploadResult.PublicId}.{uploadResult.Format}";
        }
        catch (Exception ex)
        {
            // Manejo general de errores
            throw new InvalidOperationException(
                $"Failed to upload image to Cloudinary: {ex.Message}", ex
            );
        }
    }

    /// <summary>
    /// Elimina una imagen de Cloudinary usando su nombre o ruta.
    /// </summary>
    /// <param name="fileName">Nombre o ruta del archivo</param>
    /// <returns>true si se eliminó correctamente, false si falló</returns>
    public async Task<bool> DeleteImageAsync(string fileName)
    {
        try
        {
            // Carpeta base de Cloudinary
            var folder = configuration["CloudinarySettings:Folder"]
                         ?? "auth_service/profiles";

            // Elimina la versión (v123456) si existe en la ruta
            var withoutVersion = fileName.Contains('/')
                ? string.Join('/', fileName.Split('/').Skip(1))
                : fileName;

            // Elimina la extensión del archivo
            var withoutExtension = Path.Combine(
                Path.GetDirectoryName(withoutVersion) ?? "",
                Path.GetFileNameWithoutExtension(withoutVersion)
            ).Replace("\\", "/");

            // Parámetros para eliminar recursos
            var deleteParams = new DelResParams
            {
                PublicIds = [withoutExtension]
            };

            // Llamada a Cloudinary para eliminar la imagen
            var result = await _cloudinary.DeleteResourcesAsync(deleteParams);

            // Verifica si la imagen fue eliminada correctamente
            return result.Deleted?.ContainsKey(withoutExtension) == true;
        }
        catch
        {
            // Si ocurre cualquier error, retorna false
            return false;
        }
    }

    /// <summary>
    /// Devuelve la ruta del avatar por defecto.
    /// </summary>
    public string GetDefaultAvatarUrl()
    {
        var defaultFile = configuration["CloudinarySettings:DefaultAvatarPath"]
                          ?? "default-avatar.png";

        return defaultFile;
    }

    /// <summary>
    /// Construye la URL completa de una imagen almacenada en Cloudinary.
    /// Aplica transformaciones automáticas (tamaño, calidad, formato).
    /// </summary>
    /// <param name="fileName">Ruta relativa de la imagen</param>
    /// <returns>URL completa de la imagen</returns>
    public string GetFullImageUrl(string fileName)
    {
        // URL base de Cloudinary
        var baseUrl = configuration["CloudinarySettings:BaseUrl"]
                      ?? "https://res.cloudinary.com/dqx1m6nxh/image/upload/";

        // Si no hay imagen, devuelve el avatar por defecto
        if (string.IsNullOrWhiteSpace(fileName))
        {
            var defaultFile = configuration["CloudinarySettings:DefaultAvatarPath"]
                              ?? "default-avatar.png";
            return $"{baseUrl}{defaultFile}";
        }

        // Devuelve la URL con transformaciones automáticas
        return $"{baseUrl}w_400,h_400,c_fill,g_auto,q_auto,f_auto/{fileName}";
    }

    // Métodos pendientes de implementar (probablemente duplicados)
    public string GetDefaulAvatarUrl()
    {
        throw new NotImplementedException();
    }

    public string GetFullimageUrl(string imagePath)
    {
        throw new NotImplementedException();
    }
}
