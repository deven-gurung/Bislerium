using Microsoft.AspNetCore.Mvc;
using Application.Records;

namespace Bislerium.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController(IWebHostEnvironment webHostEnvironment) : Controller
{
    [HttpPost]
    public IActionResult UploadFile([FromForm] FileUploadRecord uploads)
    {
        var filePaths = GetFilePath(uploads.FilePath);

        if (string.IsNullOrEmpty(filePaths))
        {
            return BadRequest(false);
        }

        const long maxSize = 3 * 1024 * 1024;

        if (uploads.Files.Any(upload => upload.Length > maxSize))
        {
            return BadRequest(false);
        }

        var fileNames = uploads.Files.Select(file =>
        {
            if (!Directory.Exists(Path.Combine(webHostEnvironment.WebRootPath, filePaths)))
            {
                Directory.CreateDirectory(Path.Combine(webHostEnvironment.WebRootPath, filePaths));
            }

            var uploadedDocumentPath = Path.Combine(webHostEnvironment.WebRootPath, filePaths);

            var extension = Path.GetExtension(file.FileName);

            var fileName = Guid.NewGuid().ToString() + DateTime.Now.ToString("dd-MM-yyyy:HH:mm:ss") + GetFileExtension(file);

            using var stream = new FileStream(Path.Combine(uploadedDocumentPath, fileName), FileMode.Create);

            file.CopyTo(stream);

            return fileName;
        }).ToList();

        return Ok(fileNames);
    }

    private string GetFilePath(string filePath)
    {
        return filePath switch
        {
            "1" => "user-images",
            "2" => "post-images",
            _ => ""
        };
    }
        
    public static string GetFileExtension(IFormFile file)
    {
        var contentType = file.ContentType;

        var parts = contentType.Split('/');
        if (parts.Length == 2)
        {
            return parts[1];
        }

        var fileName = file.FileName;
        return Path.GetExtension(fileName);
    }
}