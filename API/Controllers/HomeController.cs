using System.Security.Claims;
using Application.Interfaces;
using Application.Records;
using Domain.Entities;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class HomeController(IHttpContextAccessor contextAccessor,
    IGenericRepository<User> userRepository,
    IGenericRepository<Blog> blogRepository,
    IGenericRepository<Reaction> reactionRepository,
    IGenericRepository<Comment> commentRepository,
    IGenericRepository<BlogImage> blogImageRepository) : Controller
{
    [HttpGet]
    public IActionResult GetHomePageBlogs(int pageNumber, int pageSize, string? sortBy = null)
    {
        var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        Guid.TryParse(userIdClaimValue, out var userId);
        var user = blogRepository.GetById(userId);
        var blogDetails = blogRepository.GetAll().Where(x => x.IsActive).ToArray();
        var blogPostDetails = new List<BlogPostDetailsRecord>();

        foreach (var blog in blogDetails)
        {
            var reactions = reactionRepository.GetAll().Where(x => x.BlogId == blog.Id && x.IsReactedForBlog && x.IsActive).ToArray();
            var comments = commentRepository.GetAll().Where(x => x.BlogId == blog.Id && x.IsActive).ToArray();
            var reactionDetails = reactions as Reaction[] ?? reactions.ToArray();
            var commentDetails = comments as Comment[] ?? comments.ToArray();
            var upVotes = reactionDetails.Where(x => x.ReactionId == 1 && x.BlogId == blog.Id && x.IsReactedForBlog);
            var downVotes = reactionDetails.Where(x => x.ReactionId == 2 && x.BlogId == blog.Id && x.IsReactedForBlog);
            var commentForComments = commentDetails.Where(x => commentDetails.Select(z => z.CommentId).Contains(x.CommentId) && x.IsCommentForComment);
            var popularity = upVotes.Count() * 2 - downVotes.Count() * 1 + commentDetails.Count() + commentForComments.Count();

            blogPostDetails.Add(new BlogPostDetailsRecord()
            {
                BlogId = blog.Id,
                Title = blog.Title,
                Body = blog.Body,
                UpVotes = reactionDetails.Count(x => x.ReactionId == 1),
                DownVotes = reactionDetails.Count(x => x.ReactionId == 2),
                IsUpVotedByUser = reactionDetails.Any(x => x.ReactionId == 1 && x.CreatedBy == user!.Id),
                IsDownVotedByUser = reactionDetails.Any(x => x.ReactionId == 2 && x.CreatedBy == user!.Id),
                IsEdited = blog.LastModifiedAt != null,
                CreatedAt = blog.CreatedAt,
                PopularityPoints = popularity,
                Images = blogRepository.GetAll().Where(x => x.Id == blog.Id && x.IsActive).Select(x => x.Reaction).ToList(),
                UploadedTimePeriod = DateTime.Now.Hour - blog.CreatedAt.Hour < 24 ? $"{(int)(DateTime.Now - blog.CreatedAt).TotalHours} hours ago" : blog.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                Comments = commentRepository.GetAll().Where(x => x.BlogId == blog.Id && x.IsActive && x.IsCommentForBlog).Select(x => new PostComments()
                {
                    Comment = x.Text,
                    UpVotes = reactionRepository.GetAll().Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Count(z => z.ReactionId == 1),
                    DownVotes = reactionRepository.GetAll().Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Count(z => z.ReactionId == 2),
                    IsUpVotedByUser = reactionRepository.GetAll().Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Any(z => z.ReactionId == 1 && z.CreatedBy == user.Id),
                    IsDownVotedByUser = reactionRepository.GetAll().Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Any(z => z.ReactionId == 2 && z.CreatedBy == user.Id),
                    CommentId = x.Id,
                    CommentedBy = blogRepository.GetById(x.CreatedBy)!.Body,
                    ImageUrl = blogRepository.GetById(x.CreatedBy)!.Title ?? "dummy.svg",
                    IsUpdated = x.LastModifiedAt != null,
                    CommentedTimePeriod = DateTime.Now.Hour - x.CreatedAt.Hour < 24 ? $"{(int)(DateTime.Now - x.CreatedAt).TotalHours} hours ago" : x.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                }).Take(1).ToList()
            });
        }

        switch (sortBy)
        {
            case null:
                blogPostDetails = blogPostDetails.OrderByDescending(x => x.CreatedAt).ToList();
                break;
            case "Popularity":
                blogPostDetails = blogPostDetails.OrderByDescending(x => x.PopularityPoints).ToList();
                break;
            case "Recency":
                blogPostDetails = blogPostDetails.OrderByDescending(x => x.CreatedAt).ToList();
                break;
            default:
                Shuffle(blogPostDetails);
                break;
        }

        return Ok(blogPostDetails.Skip(pageNumber - 1).Take(pageSize).ToList());
    }


    [HttpGet]
    public IActionResult GetBloggersBlogs(int pageNumber, int pageSize, string? sortBy = null)
    {
        var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        Guid.TryParse(userIdClaimValue, out var userId);
        var user = userRepository.GetById(userId);
        var blogDetails = blogRepository.GetAll().Where(x => x.IsActive && x.CreatedBy == user.Id).ToArray();
        var blogPostDetails = new List<BlogPostDetailsRecord>();

        foreach (var blog in blogDetails)
        {
            var reactions = reactionRepository.GetAll().Where(x => x.BlogId == blog.Id && x.IsReactedForBlog && x.IsActive).ToArray();
            var comments = commentRepository.GetAll().Where(x => x.BlogId == blog.Id && x.IsActive).ToArray();
            var reactionDetails = reactions as Reaction[] ?? reactions.ToArray();
            var commentDetails = comments as Comment[] ?? comments.ToArray();
            var upVotes = reactionDetails.Where(x => x.ReactionId == 1 && x.BlogId == blog.Id && x.IsReactedForBlog);
            var downVotes = reactionDetails.Where(x => x.ReactionId == 2 && x.BlogId == blog.Id && x.IsReactedForBlog);
            var commentForComments = commentDetails.Where(x => commentDetails.Select(z => z.CommentId).Contains(x.CommentId) && x.IsCommentForComment);
            var popularity = upVotes.Count() * 2 - downVotes.Count() * 1 + commentDetails.Count() + commentForComments.Count();

            blogPostDetails.Add(new BlogPostDetailsRecord()
            {
                BlogId = blog.Id,
                Title = blog.Title,
                Body = blog.Body,
                UpVotes = reactionDetails.Count(x => x.ReactionId == 1),
                DownVotes = reactionDetails.Count(x => x.ReactionId == 2),
                IsUpVotedByUser = reactionDetails.Any(x => x.ReactionId == 1 && x.CreatedBy == user.Id),
                IsDownVotedByUser = reactionDetails.Any(x => x.ReactionId == 2 && x.CreatedBy == user.Id),
                IsEdited = blog.LastModifiedAt != null,
                CreatedAt = blog.CreatedAt,
                PopularityPoints = popularity,
                Images = blogImageRepository.GetAll().Where(x => x.BlogId == blog.Id && x.IsActive).Select(x => x.ImageURL).ToList(),
                UploadedTimePeriod = DateTime.Now.Hour - blog.CreatedAt.Hour < 24 ? $"{(int)(DateTime.Now - blog.CreatedAt).TotalHours} hours ago" : blog.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                Comments = commentRepository.GetAll().Where(x => x.BlogId == blog.Id && x.IsActive && x.IsCommentForBlog).Select(x => new PostComments()
                {
                    Comment = x.Text,
                    UpVotes = reactionRepository.GetAll().Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Count(z => z.ReactionId == 1),
                    DownVotes = reactionRepository.GetAll().Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Count(z => z.ReactionId == 2),
                    IsUpVotedByUser = reactionRepository.GetAll().Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Any(z => z.ReactionId == 1 && z.CreatedBy == user.Id),
                    IsDownVotedByUser = reactionRepository.GetAll().Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Any(z => z.ReactionId == 2 && z.CreatedBy == user.Id),
                    CommentId = x.Id,
                    CommentedBy = userRepository.GetById(x.CreatedBy)!.Name,
                    ImageUrl = userRepository.GetById(x.CreatedBy)!.ImageURL ?? "dummy.svg",
                    IsUpdated = x.LastModifiedAt != null,
                    CommentedTimePeriod = DateTime.Now.Hour - x.CreatedAt.Hour < 24 ? $"{(int)(DateTime.Now - x.CreatedAt).TotalHours} hours ago" : x.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                }).Take(1).ToList()
            });
        }

        switch (sortBy)
        {
            case null:
                blogPostDetails = blogPostDetails.OrderByDescending(x => x.CreatedAt).ToList();
                break;
            case "Popularity":
                blogPostDetails = blogPostDetails.OrderByDescending(x => x.PopularityPoints).ToList();
                break;
            case "Recency":
                blogPostDetails = blogPostDetails.OrderByDescending(x => x.CreatedAt).ToList();
                break;
            default:
                Shuffle(blogPostDetails);
                break;
        }

        return Ok(blogPostDetails.Skip(pageNumber - 1).Take(pageSize).ToList());
    }

    [HttpGet("{blogId:guid}")]
    public IActionResult GetBlogDetails(Guid blogId)
    {
        var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        Guid.TryParse(userIdClaimValue, out var userId);
        var user = userRepository.GetById(userId);
        var blog = blogRepository.GetById(blogId);

        var reactions = reactionRepository.GetAll()
            .Where(x => x.BlogId == blog!.Id && x.IsReactedForBlog && x.IsActive).ToArray();

        var comments = commentRepository.GetAll()
            .Where(x => x.BlogId == blog!.Id && x.IsActive).ToArray();

        var reactionDetails = reactions.ToArray();
        var commentDetails = comments.ToArray();

        var upVotes = reactionDetails.Where(x => x.ReactionId == 1 && x.BlogId == blog!.Id && x.IsReactedForBlog);
        var downVotes = reactionDetails.Where(x => x.ReactionId == 2 && x.BlogId == blog!.Id && x.IsReactedForBlog);

        var commentForComments = commentDetails
            .Where(x => commentDetails.Select(z => z.CommentId).Contains(x.CommentId) && x.IsCommentForComment);

        var popularity = upVotes.Count() * 2 - downVotes.Count() * 1 + commentDetails.Length + commentForComments.Count();

        var blogDetails = new BlogPostDetailsRecord()
        {
            BlogId = blog!.Id,
            Title = blog.Title,
            Body = blog.Body,
            UpVotes = reactionDetails.Count(x => x.ReactionId == 1),
            DownVotes = reactionDetails.Count(x => x.ReactionId == 2),
            IsUpVotedByUser = reactionDetails.Any(x => x.ReactionId == 1 && x.CreatedBy == user.Id),
            IsDownVotedByUser = reactionDetails.Any(x => x.ReactionId == 2 && x.CreatedBy == user.Id),
            IsEdited = blog.LastModifiedAt != null,
            CreatedAt = blog.CreatedAt,
            PopularityPoints = popularity,
            Images = blogImageRepository.GetAll().Where(x => x.BlogId == blog.Id && x.IsActive).Select(x => x.ImageURL).ToList(),
            UploadedTimePeriod = DateTime.Now.Hour - blog.CreatedAt.Hour < 24 ? $"{(int)(DateTime.Now - blog.CreatedAt).TotalHours} hours ago" : blog.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
            Comments = GetCommentsRecursive(blog.Id, false, true)
        };


        return Ok(blogDetails);
    }

    [HttpPost]
    public IActionResult UpVoteDownVoteBlog(ReactionActionRecord reactionModel)
    {
        var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        Guid.TryParse(userIdClaimValue, out var userId);
        var user = userRepository.GetById(userId);
        var blog = blogRepository.GetById(reactionModel.BlogId!);

        var existingReaction = reactionRepository.GetAll()
            .Where(x => x.CreatedBy == user!.Id && x.ReactionId != 3 && x.IsReactedForBlog).ToArray();

        if (existingReaction.Any())
        {
            reactionRepository.Delete(existingReaction);
        }

        var reaction = new Reaction()
        {
            ReactionId = reactionModel.ReactionId ?? 1,
            BlogId = blog!.Id,
            CommentId = null,
            IsReactedForBlog = true,
            IsReactedForComment = false,
            CreatedAt = DateTime.Now,
            CreatedBy = user!.Id,
            IsActive = true,
        };

        reactionRepository.Insert(reaction);
        reactionRepository.Save();

        return Ok(true);
    }

    [HttpPost]
    public IActionResult UpVoteDownVoteComment(ReactionActionRecord reactionModel)
    {
        var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        Guid.TryParse(userIdClaimValue, out var userId);
        var user = userRepository.GetById(userId);
        var comment = commentRepository.GetById(reactionModel.CommentId);

        var existingReaction = reactionRepository.GetAll()
            .Where(x => x.CreatedBy == user!.Id && x.ReactionId == 3 && x.IsReactedForComment).ToArray();

        if (existingReaction.Any())
        {
            reactionRepository.Delete(existingReaction);
        }

        var reaction = new Reaction()
        {
            ReactionId = reactionModel.ReactionId ?? 2,
            BlogId = null,
            CommentId = comment!.Id,
            IsReactedForBlog = false,
            IsReactedForComment = true,
            CreatedAt = DateTime.Now,
            CreatedBy = user!.Id,
            IsActive = true,
        };

        reactionRepository.Insert(reaction);
        reactionRepository.Save();

        return Ok(true);
    }

    [HttpPost]
    public IActionResult CommentForBlog(ReactionActionRecord reactionModel)
    {
        var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        Guid.TryParse(userIdClaimValue, out var userId);
        var user = userRepository.GetById(userId);
        var blog = blogRepository.GetById(reactionModel.BlogId!);

        var comment = new Comment()
        {
            BlogId = blog!.Id,
            CommentId = null,
            Text = reactionModel.Comment ?? "",
            IsCommentForBlog = true,
            IsCommentForComment = false,
            IsActive = true,
            CreatedAt = DateTime.Now,
            CreatedBy = user!.Id,
        };

        commentRepository.Insert(comment);
        commentRepository.Save();

        return Ok(true);
    }

    [HttpPost]
    public IActionResult CommentForComment(ReactionActionRecord reactionModel)
    {
        var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        Guid.TryParse(userIdClaimValue, out var userId);
        var user = userRepository.GetById(userId);
        var commentModel = commentRepository.GetById(reactionModel.CommentId);

        var comment = new Comment()
        {
            BlogId = null,
            CommentId = commentModel!.Id,
            Text = reactionModel.Comment ?? "",
            IsCommentForBlog = false,
            IsCommentForComment = true,
            IsActive = true,
            CreatedAt = DateTime.Now,
            CreatedBy = user.Id,
        };

        commentRepository.Insert(comment);
        commentRepository.Save();

        return Ok(true); 
    }

    [HttpDelete("{commentId:guid}")]
    public IActionResult DeleteComment(Guid commentId)
    {
        var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        Guid.TryParse(userIdClaimValue, out var userId);
        var comment = commentRepository.GetById(commentId);

        comment!.IsActive = false;
        comment.DeletedBy = userId;
        comment.DeletedAt = DateTime.Now;

        commentRepository.Update(comment);
        commentRepository.Save();
        
        return Ok(true);
    }

    [HttpDelete("remove-blog-reaction/{blogId:guid}")]
    public IActionResult RemoveBlogVote(Guid blogId)
    {
        var blog = blogRepository.GetById(blogId);

        var blogReactions = reactionRepository.GetAll()
            .Where(x => x.BlogId == blog!.Id && x.IsReactedForBlog);

        foreach (var blogReaction in blogReactions)
        {
            blogReaction.IsActive = false;
            reactionRepository.Update(blogReaction);
        }

        reactionRepository.Save();
        
        return Ok(true);
    }

    [HttpDelete("{commentId:guid}")]
    public IActionResult RemoveCommentVote(Guid commentId)
    {
        var comment = commentRepository.GetById(commentId);

        var commentReactions = reactionRepository.GetAll()
            .Where(x => x.CommentId == comment!.Id && x.IsReactedForComment).ToList();

        foreach (var commentReaction in commentReactions)
        {
            commentReaction.IsActive = false;
            reactionRepository.Update(commentReaction);
        }

        reactionRepository.Save();

        return Ok(true);
    }
    
    private List<PostComments> GetCommentsRecursive(Guid blogId, bool isForComment, bool isForBlog, Guid? parentId = null)
    {
        var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        Guid.TryParse(userIdClaimValue, out var userId);
        var user = userRepository.GetById(userId);

        var comments = commentRepository.GetAll()
            .Where(x => x.BlogId == blogId && x.IsActive &&
                x.IsCommentForBlog == isForBlog && x.IsCommentForComment == isForComment && x.CommentId == parentId)
            .Select(x => new PostComments()
            {
                Comment = x.Text,
                UpVotes = reactionRepository.GetAll().Where(z => z.BlogId == blogId && z.IsReactedForComment)
                    .Count(z => z.ReactionId == 1 && z.CommentId == x.Id),
                DownVotes = reactionRepository.GetAll().Where(z => z.BlogId == blogId && z.IsReactedForComment)
                    .Count(z => z.ReactionId == 2 && z.CommentId == x.Id),
                IsUpVotedByUser = reactionRepository.GetAll().Where(z => z.BlogId == blogId && z.IsReactedForComment)
                    .Any(z => z.ReactionId == 1 && z.CreatedBy == user.Id && z.CommentId == x.Id),
                IsDownVotedByUser = reactionRepository.GetAll().Where(z => z.BlogId == blogId && z.IsReactedForComment)
                    .Any(z => z.ReactionId == 2 && z.CreatedBy == user.Id && z.CommentId == x.Id),
                CommentId = x.Id,
                CommentedBy = userRepository.GetById(x.CreatedBy)!.Name,
                ImageUrl = userRepository.GetById(x.CreatedBy)!.ImageURL ?? "dummy.svg",
                IsUpdated = x.LastModifiedAt != null,
                CommentedTimePeriod = DateTime.Now.Hour - x.CreatedAt.Hour < 24
                    ? $"{(int)(DateTime.Now - x.CreatedAt).TotalHours} hours ago"
                    : x.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                Comments = GetCommentsRecursive(blogId, true, false, x.Id)
            }).ToList();

        return comments;
    }
    
    private static void Shuffle<T>(IList<T> list)  
    {  
        var random = new Random();  
    
        for (int i = list.Count - 1; i > 0; i--)  
        {  
            int j = random.Next(i + 1);  
            (list[i], list[j]) = (list[j], list[i]);
        }  
    }
}