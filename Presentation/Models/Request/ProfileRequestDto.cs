namespace SocialFeed.Models.Request;

public class ProfileRequestDto
{
    public Guid UserId { get; set; }
    
    public Guid RoleId { get; set; }
    
    public string Username { get; set; }
    
    public string FullName { get; set; }
    
    public string RoleName { get; set; }
    
    public string MobileNumber { get; set; }
    
    public string EmailAddress { get; set; }
    
    public string ImageURL { get; set; }
}