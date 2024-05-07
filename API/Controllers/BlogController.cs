using System.Net;
using Application.DTOs.Blog;
using Application.Interfaces;
using Application.Interfaces.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BlogController(IBlogService blogService) : Controller
{
    [HttpPost("create-blog")]
    public IActionResult CreateBlog(BlogCreateDto blog)
    {
        var result = blogService.CreateBlog(blog);

        if (result)
        {
            return Ok(new ResponseDto<object>()
            {
                Message = "Blog Created Successfully",
                Data = true,
                Status = "Success",
                StatusCode = HttpStatusCode.OK,
                TotalCount = 1
            });
        }
        else
        {
            return BadRequest(new ResponseDto<bool>()
            {
                Message = "Failed to create blog",
                Data = false,
                Status = "Bad Request",
                StatusCode = HttpStatusCode.BadRequest,
                TotalCount = 0
            });
        }
    }

    [HttpPatch("update-blog")]
    public IActionResult UpdateBlog(BlogDetailsDto blog)
    {
        var result = blogService.UpdateBlog(blog);

        if (result)
        {
            return Ok(new ResponseDto<object>()
            {
                Message = "Blog Updated Successfully",
                Data = true,
                Status = "Success",
                StatusCode = HttpStatusCode.OK,
                TotalCount = 1
            });
        }
        else
        {
            return BadRequest(new ResponseDto<bool>()
            {
                Message = "Failed to update blog",
                Data = false,
                Status = "Bad Request",
                StatusCode = HttpStatusCode.BadRequest,
                TotalCount = 0
            });
        }
    }

    [HttpDelete("delete-blog/{blogId:int}")]
    public IActionResult DeleteBlog(Guid blogId)
    {
        var result = blogService.DeleteBlog(blogId);

        if (result)
        {
            return Ok(new ResponseDto<object>()
            {
                Message = "Blog Deleted Successfully",
                Data = true,
                Status = "Success",
                StatusCode = HttpStatusCode.OK,
                TotalCount = 1
            });
        }
        else
        {
            return BadRequest(new ResponseDto<bool>()
            {
                Message = "Failed to delete blog",
                Data = false,
                Status = "Bad Request",
                StatusCode = HttpStatusCode.BadRequest,
                TotalCount = 0
            });
        }
    }
}