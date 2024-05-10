using System.ComponentModel.DataAnnotations;

namespace Application.Records;

public class LoginRecord
{
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; }

    [Required]
    public string Password { get; set; }
}