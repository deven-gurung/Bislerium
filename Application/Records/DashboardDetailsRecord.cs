namespace Application.Records;

public record DashboardDetailsRecord(DashboardCount DashboardCount, List<PopularBlog> PopularBlogs, List<PopularBlogger> PopularBloggers);

public record DashboardCount(int Posts, int UpVotes, int DownVotes, int Comments);

public record PopularBlog(Guid BlogId, string Blog);

public record PopularBlogger(Guid BloggerId, string BloggerName);

public record BlogDetails(Guid BlogId, string Blog, Guid BloggerId, int Popularity) : PopularBlog(BlogId, Blog);

public record BloggerDetails(Guid BloggerId, string BloggerName, int Popularity) : PopularBlogger(BloggerId, BloggerName);