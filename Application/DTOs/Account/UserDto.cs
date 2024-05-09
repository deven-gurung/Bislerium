namespace Application.DTOs.Account;

public class UserDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Username { get; set; }
    
    public string ImageUrl { get; set; }

    public Guid RoleId { get; set; }
    
    public string Role { get; set; }
    
    public string EmailAddress { get; set; }
    
    public string Token { get; set; }
}