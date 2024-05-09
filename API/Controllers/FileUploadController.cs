using System.Net;
using Application.Constants;
using Application.DTOs.Upload;
using Application.Interfaces;
using Application.Interfaces.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController(IFileUploadService fileUploadService) : Controller
{
    [HttpPost]
    public IActionResult UploadFile([FromForm] FileUploadDto uploads)
    {
        if (!int.TryParse(uploads.FilePath, out int filePathIndex))
        {
            return BadRequest(new ResponseDto<object>()
            {
                Message = "Invalid File Path.",
                StatusCode = HttpStatusCode.BadRequest,
                TotalCount = 0,
                Status = "Bad Request",
                Data = false
            });
        }

        var filePaths = filePathIndex switch
        {
            1 => Constants.FilePath.UsersImagesFilePath,
            2 => Constants.FilePath.BlogsImagesFilePath,
            _ => ""
        };

        if (string.IsNullOrEmpty(filePaths))
        {
            return BadRequest(new ResponseDto<object>()
            {
                Message = "Invalid File Path.",
                StatusCode = HttpStatusCode.BadRequest,
                TotalCount = 0,
                Status = "Bad Request",
                Data = false
            });
        }

        const long maxSize = 3 * 1024 * 1024;

        if (uploads.Files.Any(upload => upload.Length > maxSize))
        {
            return BadRequest(new ResponseDto<object>()
            {
                Message = "Invalid File Size.",
                StatusCode = HttpStatusCode.BadRequest,
                TotalCount = 0,
                Status = "Bad Request",
                Data = false
            });
        }

        var fileNames = uploads.Files.Select(file => fileUploadService.UploadDocument(filePaths, file)).ToList();

        var response = new ResponseDto<List<string>>()
        {
            Message = "File successfully uploaded.",
            Data = fileNames,
            StatusCode = HttpStatusCode.OK,
            Status = "Success",
            TotalCount = fileNames.Count
        };

        return Ok(response);
    }
}