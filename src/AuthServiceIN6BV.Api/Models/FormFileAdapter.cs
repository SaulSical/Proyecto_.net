using AuthServiceIN6BV.Application.Interfaces;

namespace AuthServiceIN6BV.Api.Models;

public class FormFileAdapter : IFileData
{
    // Fields
    private readonly IFormFile _formFile;

    private byte[]? _data;

    public FormFileAdapter(IFormFile formFile)
    {
       ArgumentNullException.ThrowIfNull(formFile);
         _formFile = formFile;
    }

    // Properties 
    public byte[] Data
    {
        get
        {
            if (_data == null)
            {
                using var memoryStream = new MemoryStream();
                _formFile.CopyTo(memoryStream);
                _data = memoryStream.ToArray();
            }
            return _data;
        }
    }

    public string ContentType => _formFile.ContentType;
    public string FileName => _formFile.FileName;
    public long Length => _formFile.Length;

    public long Size => throw new NotImplementedException();
}