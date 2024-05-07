using Application.DTOs.Home;
using Domain.Entities;

namespace Application.Interfaces;

public interface IHomeService
{
    List<BlogPostDetailsDto> GetHomePageBlogs();

    Blog[] GetActiveBlogs();

    Blog[] GetActiveBlogsByUserId();

    List<BlogPostDetailsDto> GetBloggersBlogs();

    BlogPostDetailsDto GetBlogDetails(Guid blogId);

    bool UpVoteDownVoteBlog(Guid blogId, int reactionId);

    bool UpVoteDownVoteComment(Guid commentId, int reactionId);

    bool CommentForBlog(Guid blogId, string commentText);

    bool CommentForComment(Guid commentId, string commentText);

    bool DeleteComment(Guid commentId);

    bool RemoveBlogVote(Guid blogId);

    bool RemoveCommentVote(Guid commentId);
}