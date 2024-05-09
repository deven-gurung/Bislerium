using System.Net;
using Application.DTOs.Home;
using Application.DTOs.Reaction;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController(IHomeService homeService) : Controller
{
    [HttpGet("home-page-blogs")]
    public IActionResult GetHomePageBlogs(int pageNumber, int pageSize, string? sortBy = null)
    {
        var blogPostDetails = homeService.GetHomePageBlogs();

        var blogDetails = homeService.GetActiveBlogs();

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
                blogPostDetails.Shuffle();
                break;
        }

        var result = new ResponseDto<List<BlogPostDetailsDto>>()
        {
            Message = "Success",
            Data = blogPostDetails.Skip(pageNumber - 1).Take(pageSize).ToList(),
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            TotalCount = blogDetails.Count()
        };

        return Ok(result);
    }


    [HttpGet("my-blogs")]
    public IActionResult GetBloggersBlogs(int pageNumber, int pageSize, string? sortBy = null)
    {
        var blogPostDetails = homeService.GetBloggersBlogs();

        var blogDetails = homeService.GetActiveBlogsByUserId();

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
                blogPostDetails.Shuffle();
                break;
        }

        var result = new ResponseDto<List<BlogPostDetailsDto>>()
        {
            Message = "Success",
            Data = blogPostDetails.Skip(pageNumber - 1).Take(pageSize).ToList(),
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            TotalCount = blogDetails.Count()
        };

        return Ok(result);
    }

    [HttpGet("blogs-details/{blogId:guid}")]
    public IActionResult GetBlogDetails(Guid blogId)
    {
        var blogDetails = homeService.GetBlogDetails(blogId);

        var result = new ResponseDto<BlogPostDetailsDto>()
        {
            Message = "Success",
            Data = blogDetails,
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            TotalCount = 1
        };

        return Ok(result);
    }

    [HttpPost("upvote-downvote-blog")]
    public IActionResult UpVoteDownVoteBlog(ReactionActionDto reactionModel)
    {
        var result = homeService.UpVoteDownVoteBlog(reactionModel.BlogId ?? Guid.NewGuid(), reactionModel.ReactionId ?? 1);

        return Ok(new ResponseDto<object>()
        {
            Message = "Success",
            StatusCode = HttpStatusCode.OK,
            TotalCount = 0,
            Status = "Success",
            Data = result //true or false
        });
    }

    [HttpPost("upvote-downvote-comment")]
    public IActionResult UpVoteDownVoteComment(ReactionActionDto reactionModel)
    {
        var result = homeService.UpVoteDownVoteComment(reactionModel.CommentId ?? Guid.NewGuid(), reactionModel.ReactionId ?? 0);

        return Ok(new ResponseDto<object>()
        {
            Message = "Success",
            StatusCode = HttpStatusCode.OK,
            TotalCount = 0,
            Status = "Success",
            Data = result //true or false
        });
    }

    [HttpPost("comment-for-blog")]
    public IActionResult CommentForBlog(ReactionActionDto reactionModel)
    {
        var result = homeService.CommentForBlog(reactionModel.BlogId ?? Guid.NewGuid(), reactionModel.Comment ?? "");

        return Ok(new ResponseDto<object>()
        {
            Message = "Success",
            StatusCode = HttpStatusCode.OK,
            TotalCount = 0,
            Status = "Success",
            Data = result //true or false
        });
    }

    [HttpPost("comment-for-comment")]
    public IActionResult CommentForComment(ReactionActionDto reactionModel)
    {
        var result = homeService.CommentForComment(reactionModel.CommentId ?? Guid.NewGuid(), reactionModel.Comment ?? "");

        return Ok(new ResponseDto<object>()
        {
            Message = "Success",
            StatusCode = HttpStatusCode.OK,
            TotalCount = 0,
            Status = "Success",
            Data = result //true or false
        }); 
    }

    [HttpDelete("delete-comment/{commentId:guid}")]
    public IActionResult DeleteComment(Guid commentId)
    {
        var result = homeService.DeleteComment(commentId);

        return Ok(new ResponseDto<object>()
        {
            Message = "Success",
            StatusCode = HttpStatusCode.OK,
            TotalCount = 0,
            Status = "Success",
            Data = result //true or false
        });
    }

    [HttpDelete("remove-blog-reaction/{blogId:guid}")]
    public IActionResult RemoveBlogVote(Guid blogId)
    {
        var result = homeService.RemoveBlogVote(blogId);

        return Ok(new ResponseDto<object>()
        {
            Message = "Success",
            StatusCode = HttpStatusCode.OK,
            TotalCount = 0,
            Status = "Success",
            Data = result //true or false
        });
    }

    [HttpDelete("remove-comment-reaction/{commentId:guid}")]
    public IActionResult RemoveCommentVote(Guid commentId)
    {
        var result = homeService.RemoveCommentVote(commentId);

        return Ok(new ResponseDto<object>()
        {
            Message = "Success",
            StatusCode = HttpStatusCode.OK,
            TotalCount = 0,
            Status = "Success",
            Data = result //true or false
        });
    }
}