using Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class BlogImage : BaseEntity<Guid>
{
    public string ImageURL { get; set; }
    
    public Guid BlogId { get; set; }
    
    [ForeignKey("BlogId")]
    public virtual Blog Blog { get; set; }
}