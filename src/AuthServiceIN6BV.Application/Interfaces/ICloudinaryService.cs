namespace AuthServiceIN6BV.Application.InTerface;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFileData imagenFile, string fileName);

    Task<bool> DeleteImageAsync(string publicId);

    string GetDefaulAvatarUrl();

    string GetFullimageUrl(string imagePath);

}

