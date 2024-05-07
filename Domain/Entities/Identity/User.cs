using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; }

    public string? ImageURL { get; set; }
    
    public virtual ICollection<Blog>? Blogs { get; set; }

    public virtual ICollection<BlogImage>? BlogImages { get; set; }

    public virtual ICollection<BlogLog>? BlogLogs { get; set; }

    public virtual ICollection<Comment>? Comments { get; set; }
	
    public virtual ICollection<CommentLog>? CommentLogs { get; set; }
	
    public virtual ICollection<Notification>? Notifications { get; set; }
	
    public virtual ICollection<Reaction>? Reactions { get; set; }
}