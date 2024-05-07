using System.ComponentModel.DataAnnotations.Schema;
using Domain.Shared;

namespace Domain.Entities;

public class Reaction : BaseEntity<Guid>
{
    public int ReactionId { get; set; }               // 1 is for Upvote, 2 is for Downvote, 3 is for Comment

    public bool IsReactedForBlog { get; set; }
    
    public bool IsReactedForComment { get; set; }
    
    public Guid? BlogId { get; set; }
    
    public Guid? CommentId { get; set; }
    
    [ForeignKey("BlogId")]
    public virtual Blog? Blog { get; set; }
    
    [ForeignKey("CommentId")]
    public virtual Comment? Comment { get; set; }
}