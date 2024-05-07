using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IFileUploadService
{
    string UploadDocument(string uploadedFilePath, IFormFile file);
}