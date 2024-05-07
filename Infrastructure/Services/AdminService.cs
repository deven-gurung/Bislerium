using Application.DTOs.Account;
using Application.DTOs.Dashboard;
using Application.DTOs.User;
using Application.Interfaces;
using Domain.Entities.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class AdminService(ApplicationDbContext dbContext) : IAdminService
{
    public List<UserDetailDto> GetAllUsers()
    {
        var users = dbContext.Users.ToList().Select(u => new UserDetailDto
        {
            Id = u.Id,
            RoleId = dbContext.UserRoles.FirstOrDefault(x => x.UserId == u.Id)?.RoleId ?? new Guid(),
            EmailAddress = u.Email ?? "",
            ImageURL = u.ImageURL ?? "",
            Username = u.UserName ?? "",
            Name = u.Name,
            RoleName = dbContext.Roles.Find(dbContext.UserRoles.FirstOrDefault(x => x.UserId == u.Id)?.RoleId)!.Name ?? ""
        }).ToList();

        return users;
    }

    public bool RegisterAdmin(RegisterDto register)
    { 
        var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == register.EmailAddress);

        if (existingUser == null)
        {
            var role = dbContext.Roles.FirstOrDefault(r => r.Name == "Admin");

            var passwordHash = new PasswordHasher<User>();

            var adminId = new Guid();
            
            var appUser = new User()
            {
                Id = adminId,
                Name = register.FullName,
                Email = register.EmailAddress,
                UserName = register.EmailAddress,
                ImageURL = register.ImageURL,
                NormalizedEmail = register.EmailAddress.ToUpper(),
                NormalizedUserName = register.EmailAddress.ToUpper(),
                PhoneNumber = register.MobileNumber,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
            };
            
            appUser.PasswordHash = passwordHash.HashPassword(appUser, register.Password);

            dbContext.Users.Add(appUser);

            var adminUserRole = new UserRoles()
            {
                UserId = appUser.Id,
                RoleId = role!.Id
            };
            
            dbContext.UserRoles.Add(adminUserRole);

            return true;
        }

        return false;
    }

    public DashboardDetailsDto GetDashboardDetails()
    {
        var blogs = dbContext.Blogs.Where(x => x.IsActive).ToList();

        var reactions = dbContext.Reactions.Where(x => x.IsActive).ToList();

        var comments = dbContext.Comments.Where(x => x.IsActive).ToList();

        var dashboardDetails = new DashboardCount()
        {
            Posts = blogs.Count,
            Comments = comments.Count,
            UpVotes = reactions.Count(x => x.ReactionId == 1),
            DownVotes = reactions.Count(x => x.ReactionId == 2),
        };

        var blogDetailsList = new List<BlogDetails>();

        foreach (var blog in blogs)
        {
            var upVotes = reactions.Where(x => x.ReactionId == 1 && x.BlogId == blog.Id && x.IsReactedForBlog);

            var downVotes = reactions.Where(x => x.ReactionId == 2 && x.BlogId == blog.Id && x.IsReactedForBlog);

            var commentReactions = comments.Where(x => x.BlogId == blog.Id && x.IsCommentForBlog);

            var commentForComments =
                comments.Where(x =>
                    commentReactions.Select(z =>
                        z.CommentId).Contains(x.CommentId) && x.IsCommentForComment);

            var popularity = upVotes.Count() * 2 -
                                downVotes.Count() * 1 +
                                commentReactions.Count() + commentForComments.Count();

            blogDetailsList.Add(new BlogDetails()
            {
                BlogId = blog.Id,
                Blog = blog.Title,
                BloggerId = blog.CreatedBy,
                Popularity = popularity
            });
        }

        var bloggerDetailsList = blogDetailsList
            .GroupBy(blog => blog.BloggerId)
            .Select(group => new BloggerDetails
            {
                BloggerId = group.Key,
                BloggerName = dbContext.Users.Find(group.Key)!.Name,
                Popularity = group.Sum(blog => blog.Popularity)
            }).ToList();

        var popularBlogs = blogDetailsList
            .OrderByDescending(x => x.Popularity)
            .Take(10).Select(z => new PopularBlog()
            {
                BlogId = z.BlogId,
                Blog = z.Blog
            }).ToList();

        var popularBloggers = bloggerDetailsList
            .OrderByDescending(x => x.Popularity)
            .Take(10).Select(z => new PopularBlogger()
            {
                BloggerId = z.BloggerId,
                BloggerName = z.BloggerName
            }).ToList();

        var dashboardCounts = new DashboardDetailsDto()
        {
            DashboardCount = dashboardDetails,
            PopularBloggers = popularBloggers,
            PopularBlogs = popularBlogs
        };

       return dashboardCounts;
    }
}