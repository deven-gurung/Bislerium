using System.ComponentModel.DataAnnotations.Schema;
using Domain.Shared;

namespace Domain.Entities;

public class Comment : BaseEntity<Guid>
{
    public string Text { get; set; }
    
    public bool IsCommentForBlog { get; set; }
    
    public bool IsCommentForComment { get; set; }
    
    public Guid? BlogId { get; set; }
    
    public Guid? CommentId { get; set; }
    
    [ForeignKey("BlogId")]
    public virtual Blog? Blog { get; set; }
    
    [ForeignKey("CommentId")]
    public virtual Comment? Comments { get; set; }
}