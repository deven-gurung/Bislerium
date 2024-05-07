using System.ComponentModel.DataAnnotations.Schema;
using Domain.Shared;

namespace Domain.Entities;

public class CommentLog : BaseEntity<Guid>
{
    public Guid CommentId { get; set; }
    
    public string Text { get; set; }
    
    [ForeignKey("CommentId")]
    public virtual Comment? Comment { get; set; }
}