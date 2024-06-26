using Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class BlogLog : BaseEntity<Guid>
{
    public Guid BlogId { get; set; }
    
    public string Title { get; set; }
    
    public string Body { get; set; }
    
    public string Location { get; set; }
    
    public string Reaction { get; set; }
    
    [ForeignKey("BlogId")]
    public virtual Blog? Blog { get; set; }
}