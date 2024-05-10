using Application.Interfaces;
using Application.Records;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Bislerium.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminController(
        IGenericRepository<User> userRepository,
        IGenericRepository<Role> roleRepository,
        IGenericRepository<UserRoles> userRoleRepository,
        IGenericRepository<Blog> blogRepository,
        IGenericRepository<Comment> commentRepository,
        IGenericRepository<Reaction> reactionRepository)
        : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = userRepository.GetAll().Select(u => new UserDetailRecord
            {
                Id = u.Id,
                RoleId = roleRepository.GetById(userRoleRepository.GetAll().FirstOrDefault(x => x.UserId == u.Id)!.RoleId)!.Id,
                EmailAddress = u.Email ?? "",
                ImageURL = u.ImageURL ?? "",
                Username = u.UserName ?? "",
                Name = u.Name,
                RoleName = roleRepository.GetById(userRoleRepository.GetAll().FirstOrDefault(x => x.UserId == u.Id)!.RoleId)!.Name ?? ""
            }).ToList();
            
            return Ok(users);
        }

        [HttpPost]
        public IActionResult RegisterAdmin(RegisterRecord register)
        {
            var existingUser = userRepository.GetAll().FirstOrDefault(u => u.Email == register.EmailAddress);

            if (existingUser == null)
            {
                var role = roleRepository.GetAll().FirstOrDefault(r => r.Name == "Admin");

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

                userRepository.Insert(appUser);

                var adminUserRole = new UserRoles()
                {
                    UserId = appUser.Id,
                    RoleId = role!.Id
                };
                
                userRoleRepository.Insert(adminUserRole);

                userRepository.Save();

                return Ok(true);
            }

            return BadRequest(false);
        }

        [HttpGet]
        public IActionResult GetDashboardDetails()
        {
            var blogs = blogRepository.GetAll().Where(x => x.IsActive).ToList();

            var reactions = reactionRepository.GetAll().Where(x => x.IsActive).ToList();

            var comments = commentRepository.GetAll().Where(x => x.IsActive).ToList();

            var dashboardDetails = new DashboardCount(blogs.Count, reactions.Count(x => x.ReactionId == 1), reactions.Count(x => x.ReactionId == 2), comments.Count)
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

                blogDetailsList.Add(new BlogDetails(blog.Id, blog.Title, blog.CreatedBy, popularity)
                {
                    BlogId = blog.Id,
                    Blog = blog.Title,
                    BloggerId = blog.CreatedBy,
                    Popularity = popularity
                });
            }

            var bloggerDetailsList = blogDetailsList
                .GroupBy(blog => blog.BloggerId)
                .Select(group => new BloggerDetails(group.Key, userRepository.GetById(group.Key)!.Name, group.Sum(blog => blog.Popularity))
                {
                    BloggerId = group.Key,
                    BloggerName = userRepository.GetById(group.Key)!.Name,
                    Popularity = group.Sum(blog => blog.Popularity)
                }).ToList();

            var popularBlogs = blogDetailsList
                .OrderByDescending(x => x.Popularity)
                .Take(10).Select(z => new PopularBlog(z.BlogId, z.Blog)
                {
                    BlogId = z.BlogId,
                    Blog = z.Blog
                }).ToList();

            var popularBloggers = bloggerDetailsList
                .OrderByDescending(x => x.Popularity)
                .Take(10).Select(z => new PopularBlogger(z.BloggerId, z.BloggerName)
                {
                    BloggerId = z.BloggerId,
                    BloggerName = z.BloggerName
                }).ToList();

            var dashboardCounts = new DashboardDetailsRecord(dashboardDetails, popularBlogs, popularBloggers)
            {
                DashboardCount = dashboardDetails,
                PopularBloggers = popularBloggers,
                PopularBlogs = popularBlogs
            };

            return Ok(dashboardCounts);
        }
    }
}
