namespace Application.Records;

public class ChangePasswordRecord
{
    public string CurrentPassword { get; set; }
    
    public string NewPassword { get; set; }
    
    public string ConfirmPassword { get; set; }
}