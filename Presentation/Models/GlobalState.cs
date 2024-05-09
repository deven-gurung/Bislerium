namespace SocialFeed.Models;

public class GlobalState
{
    public Guid UserId { get; set; }
    
    public string Name { get; set; } = null!;

    public Guid? RoleId { get; set; }

    public string? RoleName { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;
}