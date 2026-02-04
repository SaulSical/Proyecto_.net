namespace AuthServiceIN6BV.Application.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFileData imagenFile, string fileName);

    Task<bool> DeleteImageAsync(string publicId);

    string GetDefaulAvatarUrl();

    string GetFullimageUrl(string imagePath);
    string GetFullImageUrl(string v);
}

