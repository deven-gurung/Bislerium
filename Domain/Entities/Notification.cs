using Domain.Shared;
using Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Notification : BaseEntity<Guid>
{
    public Guid SenderId { get; set; }
    
    public Guid ReceiverId { get; set; }
    
    public string Title { get; set; }
    
    public string Content { get; set; }
    
    public bool IsSeen { get; set; }
    
    public Guid BlogId { get; set; }
    
    [ForeignKey("SenderId")]
    public virtual User? Sender { get; set; }

    [ForeignKey("ReceiverId")]
    public virtual User? Receiver { get; set; }
    
    [ForeignKey("BlogId")]
    public virtual Blog? Blog { get; set; }
}