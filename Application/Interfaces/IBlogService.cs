using Application.DTOs.Blog;

namespace Application.Interfaces;

public interface IBlogService
{
    bool CreateBlog(BlogCreateDto blog);

    bool UpdateBlog(BlogDetailsDto blog);

    bool DeleteBlog(Guid blogId);
}