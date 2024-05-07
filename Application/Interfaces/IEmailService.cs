using Application.DTOs.Email;

namespace Application.Interfaces;

public interface IEmailService
{
    void SendEmail(EmailDto email);
}