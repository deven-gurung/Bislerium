using System.Net;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using Application.Records;
using Domain.Entities;
using static System.Guid;

namespace Bislerium.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BlogController(IGenericRepository<Blog> blogRepository, IHttpContextAccessor contextAccessor)
        : Controller
    {
        [HttpPost]
        public IActionResult CreateBlog(BlogCreateRecord blog)
        {
            var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            TryParse(userIdClaimValue, out var userId);
            
            var blogModel = new Blog()
            {
                Title = blog.Title,
                Body = blog.Body,
                Location = blog.Location,
                Reaction = blog.Reaction,
                BlogImages = blog.Images?.Select(x => new BlogImage()
                {
                    ImageURL = x,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId
                }).ToList(),
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
            };

            blogRepository.Insert(blogModel);
            blogRepository.Save();

            return Ok(true);
        }

        [HttpPatch("update-blog")]
        public IActionResult UpdateBlog(BlogDetailsRecord blog)
        {
            var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            TryParse(userIdClaimValue, out var userId);

            var blogModel = blogRepository.GetById(blog.Id);

            blogModel.Title = blog.Title;
            blogModel.Body = blog.Body;
            blogModel.Location = blog.Location;
            blogModel.Reaction = blog.Reaction;

            blogModel.LastModifiedAt = DateTime.Now;
            blogModel.LastModifiedBy = userId;

            blogRepository.Update(blogModel);
            blogRepository.Save();

            return Ok(true);
        }

        [HttpDelete("{blogId:guid}")]
        public IActionResult DeleteBlog(Guid blogId)
        {
            var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            TryParse(userIdClaimValue, out var userId);

            var blogModel = blogRepository.GetById(blogId);

            blogModel.IsActive = false;
            blogModel.DeletedAt = DateTime.Now;
            blogModel.DeletedBy = userId;

            blogRepository.Update(blogModel);
            blogRepository.Save();

            return Ok(true);
        }
    }
}
