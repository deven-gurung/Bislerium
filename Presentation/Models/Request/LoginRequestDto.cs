using System.ComponentModel.DataAnnotations;

namespace SocialFeed.Models.Request;

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; }

    [Required]
    public string Password { get; set; }
}