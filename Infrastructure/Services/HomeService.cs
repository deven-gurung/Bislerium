using Application.DTOs.Home;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Identity;
using Infrastructure.Persistence;

namespace Infrastructure.Services;

public class HomeService(IUserService userService, ApplicationDbContext dbContext) : IHomeService
{
    public Blog[] GetActiveBlogs()
    {
        var blogs = dbContext.Blogs.Where(x => x.IsActive).ToList();

        return blogs.ToArray();
    }

    public Blog[] GetActiveBlogsByUserId()
    {
        var userId = userService.UserId;

        var user = dbContext.Users.Find(userId);

        var blogs = dbContext.Blogs.Where(x => x.IsActive && x.CreatedBy == user!.Id);

        return blogs.ToArray();
    }

    public List<BlogPostDetailsDto> GetHomePageBlogs()
    {
        var userId = userService.UserId;

        var user = dbContext.Users.Find(userId);

        var blogDetails = dbContext.Blogs.Where(x => x.IsActive).ToArray();

        var blogPostDetails = new List<BlogPostDetailsDto>();

        foreach (var blog in blogDetails)
        {
            var reactions = dbContext.Reactions.Where(x => x.BlogId == blog.Id && x.IsReactedForBlog && x.IsActive).ToArray();

            var comments = dbContext.Comments.Where(x => x.BlogId == blog.Id && x.IsActive).ToArray();

            var reactionDetails = reactions as Reaction[] ?? reactions.ToArray();

            var commentDetails = comments as Comment[] ?? comments.ToArray();

            var upVotes = reactionDetails.Where(x => x.ReactionId == 1 && x.BlogId == blog.Id && x.IsReactedForBlog);

            var downVotes = reactionDetails.Where(x => x.ReactionId == 2 && x.BlogId == blog.Id && x.IsReactedForBlog);

            var commentForComments =
                commentDetails.Where(x =>
                    commentDetails.Select(z =>
                        z.CommentId).Contains(x.CommentId) && x.IsCommentForComment);

            var popularity = upVotes.Count() * 2 -
                             downVotes.Count() * 1 +
                             commentDetails.Count() + commentForComments.Count();

            blogPostDetails.Add(new BlogPostDetailsDto()
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
                Images = dbContext.BlogImages.Where(x => x.BlogId == blog.Id && x.IsActive).Select(x => x.ImageURL).ToList(),
                UploadedTimePeriod = DateTime.Now.Hour - blog.CreatedAt.Hour < 24 ? $"{(int)(DateTime.Now - blog.CreatedAt).TotalHours} hours ago" : blog.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                Comments = dbContext.Comments.Where(x => x.BlogId == blog.Id && x.IsActive && x.IsCommentForBlog).Select(x => new PostComments()
                {
                    Comment = x.Text,
                    UpVotes = dbContext.Reactions.Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Count(z => z.ReactionId == 1),
                    DownVotes = dbContext.Reactions.Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Count(z => z.ReactionId == 2),
                    IsUpVotedByUser = dbContext.Reactions.Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Any(z => z.ReactionId == 1 && z.CreatedBy == user.Id),
                    IsDownVotedByUser = dbContext.Reactions.Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Any(z => z.ReactionId == 2 && z.CreatedBy == user.Id),
                    CommentId = x.Id,
                    CommentedBy = dbContext.Users.Find(x.CreatedBy)!.Name,
                    ImageUrl = dbContext.Users.Find(x.CreatedBy)!.ImageURL ?? "sample-profile.png",
                    IsUpdated = x.LastModifiedAt != null,
                    CommentedTimePeriod = DateTime.Now.Hour - x.CreatedAt.Hour < 24 ? $"{(int)(DateTime.Now - x.CreatedAt).TotalHours} hours ago" : x.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                }).Take(1).ToList()
            });
        }

        return blogPostDetails;
    }

    public List<BlogPostDetailsDto> GetBloggersBlogs()
    {
        var userId = userService.UserId;

        var user = dbContext.Users.Find(userId);

        var blogDetails = dbContext.Blogs.Where(x => x.IsActive && x.CreatedBy == user.Id).ToArray();

        var blogPostDetails = new List<BlogPostDetailsDto>();

        foreach (var blog in blogDetails)
        {
            var reactions = dbContext.Reactions.Where(x => x.BlogId == blog.Id && x.IsReactedForBlog && x.IsActive).ToArray();

            var comments = dbContext.Comments.Where(x => x.BlogId == blog.Id && x.IsActive).ToArray();

            var reactionDetails = reactions as Reaction[] ?? reactions.ToArray();

            var commentDetails = comments as Comment[] ?? comments.ToArray();

            var upVotes = reactionDetails.Where(x => x.ReactionId == 1 && x.BlogId == blog.Id && x.IsReactedForBlog);

            var downVotes = reactionDetails.Where(x => x.ReactionId == 2 && x.BlogId == blog.Id && x.IsReactedForBlog);

            var commentForComments =
                commentDetails.Where(x =>
                    commentDetails.Select(z =>
                        z.CommentId).Contains(x.CommentId) && x.IsCommentForComment);

            var popularity = upVotes.Count() * 2 -
                             downVotes.Count() * 1 +
                             commentDetails.Count() + commentForComments.Count();

            blogPostDetails.Add(new BlogPostDetailsDto()
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
                Images = dbContext.BlogImages.Where(x => x.BlogId == blog.Id && x.IsActive).Select(x => x.ImageURL).ToList(),
                UploadedTimePeriod = DateTime.Now.Hour - blog.CreatedAt.Hour < 24 ? $"{(int)(DateTime.Now - blog.CreatedAt).TotalHours} hours ago" : blog.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                Comments = dbContext.Comments.Where(x => x.BlogId == blog.Id && x.IsActive && x.IsCommentForBlog).Select(x => new PostComments()
                {
                    Comment = x.Text,
                    UpVotes = dbContext.Reactions.Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Count(z => z.ReactionId == 1),
                    DownVotes = dbContext.Reactions.Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Count(z => z.ReactionId == 2),
                    IsUpVotedByUser = dbContext.Reactions.Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Any(z => z.ReactionId == 1 && z.CreatedBy == user.Id),
                    IsDownVotedByUser = dbContext.Reactions.Where(z => z.BlogId == blog.Id && z.IsReactedForComment && x.IsActive).Any(z => z.ReactionId == 2 && z.CreatedBy == user.Id),
                    CommentId = x.Id,
                    CommentedBy = dbContext.Users.Find(x.CreatedBy)!.Name,
                    ImageUrl = dbContext.Users.Find(x.CreatedBy)!.ImageURL ?? "sample-profile.png",
                    IsUpdated = x.LastModifiedAt != null,
                    CommentedTimePeriod = DateTime.Now.Hour - x.CreatedAt.Hour < 24 ? $"{(int)(DateTime.Now - x.CreatedAt).TotalHours} hours ago" : x.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                }).Take(1).ToList()
            });
        }

        return blogPostDetails;
    }

    public BlogPostDetailsDto GetBlogDetails(Guid blogId)
    {
        var userId = userService.UserId;

        var user = dbContext.Users.Find(userId);

        var blog = dbContext.Blogs.Find(blogId);

        var reactions = dbContext.Reactions.Where(x => x.BlogId == blog!.Id && x.IsReactedForBlog && x.IsActive).ToArray();

        var comments = dbContext.Comments.Where(x => x.BlogId == blog!.Id && x.IsActive).ToArray();

        var reactionDetails = reactions as Reaction[] ?? reactions.ToArray();

        var commentDetails = comments as Comment[] ?? comments.ToArray();

        var upVotes = reactionDetails.Where(x => x.ReactionId == 1 && x.BlogId == blog!.Id && x.IsReactedForBlog);

        var downVotes = reactionDetails.Where(x => x.ReactionId == 2 && x.BlogId == blog!.Id && x.IsReactedForBlog);

        var commentForComments =
            commentDetails.Where(x =>
                commentDetails.Select(z =>
                    z.CommentId).Contains(x.CommentId) && x.IsCommentForComment);

        var popularity = upVotes.Count() * 2 -
                         downVotes.Count() * 1 +
                         commentDetails.Count() + commentForComments.Count();

        var blogDetails = new BlogPostDetailsDto()
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
            Images = dbContext.BlogImages.Where(x => x.BlogId == blog.Id && x.IsActive).Select(x => x.ImageURL).ToList(),
            UploadedTimePeriod = DateTime.Now.Hour - blog.CreatedAt.Hour < 24 ? $"{(int)(DateTime.Now - blog.CreatedAt).TotalHours} hours ago" : blog.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
            Comments = GetCommentsRecursive(blog.Id, false, true)
        };

        return blogDetails;
    }

    public bool UpVoteDownVoteBlog(Guid blogId, int reactionId)
    {
        var userId = userService.UserId;

        var user = dbContext.Users.Find(userId);

        var blog = dbContext.Blogs.Find(blogId);

        var existingReaction =
            dbContext.Reactions.Where(x => x.CreatedBy == user!.Id &&
                x.ReactionId != 3 && x.IsReactedForBlog).ToArray();

        if (existingReaction.Any())
        {
            dbContext.RemoveRange(existingReaction);
        }

        var reaction = new Reaction()
        {
            ReactionId = reactionId,
            BlogId = blog!.Id,
            CommentId = null,
            IsReactedForBlog = true,
            IsReactedForComment = false,
            CreatedAt = DateTime.Now,
            CreatedBy = user!.Id,
            IsActive = true,
        };

        dbContext.Reactions.Add(reaction);

        return true;
    }

    public bool UpVoteDownVoteComment(Guid commentId, int reactionId)
    {
        var userId = userService.UserId;

        var user = dbContext.Users.Find(userId);

        var comment = dbContext.Comments.Find(commentId);

        var existingReaction =
            dbContext.Reactions.Where(x => x.CreatedBy == user!.Id &&
                                                  x.ReactionId == 3 && x.IsReactedForComment).ToArray();

        var existingReactionDetails = existingReaction as Reaction[] ?? existingReaction.ToArray();

        if (existingReactionDetails.Any())
        {
            dbContext.RemoveRange(existingReactionDetails);
        }

        var reaction = new Reaction()
        {
            ReactionId = reactionId,
            BlogId = null,
            CommentId = comment!.Id,
            IsReactedForBlog = false,
            IsReactedForComment = true,
            CreatedAt = DateTime.Now,
            CreatedBy = user!.Id,
            IsActive = true,
        };

        dbContext.Reactions.Add(reaction);

        return true;
    }

    public bool CommentForBlog(Guid blogId, string commentText)
    {
        var userId = userService.UserId;

        var user = dbContext.Users.Find(userId);

        var blog = dbContext.Blogs.Find(blogId);

        var comment = new Comment()
        {
            BlogId = blog!.Id,
            CommentId = null,
            Text = commentText,
            IsCommentForBlog = true,
            IsCommentForComment = false,
            IsActive = true,
            CreatedAt = DateTime.Now,
            CreatedBy = user!.Id,
        };

        dbContext.Comments.Add(comment);

        return true;
    }

    public bool CommentForComment(Guid commentId, string commentText)
    {
        var userId = userService.UserId;

        var user = dbContext.Users.Find(userId);

        var commentModel = dbContext.Comments.Find(commentId);

        var comment = new Comment()
        {
            BlogId = null,
            CommentId = commentModel!.Id,
            Text = commentText,
            IsCommentForBlog = false,
            IsCommentForComment = true,
            IsActive = true,
            CreatedAt = DateTime.Now,
            CreatedBy = user.Id,
        };

        dbContext.Comments.Add(comment);

        return true;
    }

    public bool DeleteComment(Guid commentId)
    {
        var userId = userService.UserId;

        var comment = dbContext.Comments.Find(commentId);

        comment!.IsActive = false;
        comment.DeletedBy = userId;
        comment.DeletedAt = DateTime.Now;

        dbContext.Update(comment);

        return true;
    }

    public bool RemoveBlogVote(Guid blogId)
    {
        var blog = dbContext.Blogs.Find(blogId);

        var blogReactions = dbContext.Reactions.Where(x => x.BlogId == blog!.Id
                                                                     && x.IsReactedForBlog);

        foreach (var blogReaction in blogReactions)
        {
            blogReaction.IsActive = false;

            dbContext.Update(blogReaction);
        }
        
        return true;
    }

    public bool RemoveCommentVote(Guid commentId)
    {
        var comment = dbContext.Comments.Find(commentId);

        var commentReactions = dbContext.Reactions.Where(x => x.CommentId == comment!.Id
                                                                     && x.IsReactedForComment).ToList();

        foreach (var commentReaction in commentReactions)
        {
            commentReaction.IsActive = false;

            dbContext.Update(commentReaction);
        }
        return true;
    }

    private List<PostComments> GetCommentsRecursive(Guid blogId, bool isForComment, bool isForBlog, Guid? parentId = null)
    {
        var userId = userService.UserId;

        var user = dbContext.Users.Find(userId);

        var comments =
            dbContext.Comments.Where(x => x.BlogId == blogId && x.IsActive &&
                x.IsCommentForBlog == isForBlog && x.IsCommentForComment == isForComment && x.CommentId == parentId)
                    .Select(x => new PostComments()
                    {
                        Comment = x.Text,
                        UpVotes = dbContext.Reactions.Where(z => z.BlogId == blogId && z.IsReactedForComment).Count(z => z.ReactionId == 1 && z.CommentId == x.Id),
                        DownVotes = dbContext.Reactions.Where(z => z.BlogId == blogId && z.IsReactedForComment).Count(z => z.ReactionId == 2 && z.CommentId == x.Id),
                        IsUpVotedByUser = dbContext.Reactions.Where(z => z.BlogId == blogId && z.IsReactedForComment).Any(z => z.ReactionId == 1 && z.CreatedBy == user.Id && z.CommentId == x.Id),
                        IsDownVotedByUser = dbContext.Reactions.Where(z => z.BlogId == blogId && z.IsReactedForComment).Any(z => z.ReactionId == 2 && z.CreatedBy == user.Id && z.CommentId == x.Id),
                        CommentId = x.Id,
                        CommentedBy = dbContext.Users.Find(x.CreatedBy)!.Name,
                        ImageUrl = dbContext.Users.Find(x.CreatedBy)!.ImageURL ?? "sample-profile.png",
                        IsUpdated = x.LastModifiedAt != null,
                        CommentedTimePeriod = DateTime.Now.Hour - x.CreatedAt.Hour < 24 ? $"{(int)(DateTime.Now - x.CreatedAt).TotalHours} hours ago" : x.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                        Comments = GetCommentsRecursive(blogId, true, false, x.Id)
                    }).ToList();

        return comments;
    }
}