using Microsoft.AspNetCore.Http;

namespace Application.Records;

public class FileUploadRecord
{
    public string FilePath { get; set; }
    
    public List<IFormFile> Files { get; set; }
}