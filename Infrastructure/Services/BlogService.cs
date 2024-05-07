using Application.DTOs.Blog;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Identity;
using Infrastructure.Persistence;

namespace Infrastructure.Services;

public class BlogService(ApplicationDbContext dbContext, IUserService userService) : IBlogService
{
    public bool CreateBlog (BlogCreateDto blog)
    {
        var userId = userService.UserId;

        var user = dbContext.Users.Find(userId);

        var blogModel = new Blog()
        {
            Title = blog.Title,
            Body = blog.Body,
            Location = blog.Location,
            Reaction = blog.Reaction,
            BlogImages = blog.Images.Select(x => new BlogImage()
            {
                ImageURL = x,
                IsActive = true,
                CreatedAt = DateTime.Now,
                CreatedBy = user!.Id
            }).ToList(),
            CreatedAt = DateTime.Now,
            CreatedBy = user!.Id,
        };

        dbContext.Blogs.Add(blogModel);

        return true;
    }

    public bool UpdateBlog (BlogDetailsDto blog)
    {
        var userId = userService.UserId;

        var user = dbContext.Users.Find(userId);

        var blogModel = dbContext.Blogs.Find(blog.Id);

        var blogLog = new BlogLog()
        {
            BlogId = blogModel!.Id,
            Title = blogModel.Title,
            Location = blogModel.Location,
            Reaction = blogModel.Reaction,
            CreatedAt = DateTime.Now,
            CreatedBy = user!.Id,
            Body = blogModel.Body,
            IsActive = false
        };

        dbContext.BlogLogs.Add(blogLog);

        blogModel.Title = blog.Title;
        blogModel.Body = blog.Body;
        blogModel.Location = blog.Location;
        blogModel.Reaction = blog.Reaction;

        blogModel.LastModifiedAt = DateTime.Now;
        blogModel.LastModifiedBy = user.Id;

        dbContext.Update(blogModel);

        return true;
    }

    public bool DeleteBlog(Guid blogId)
    { 
        var userId = userService.UserId;

        var user = dbContext.Users.Find(userId);

        var blogModel = dbContext.Blogs.Find(blogId);

        blogModel!.IsActive = false;
        blogModel.DeletedAt = DateTime.Now;
        blogModel.DeletedBy = user!.Id;

        dbContext.Update(blogModel);

        return true;
    }
}