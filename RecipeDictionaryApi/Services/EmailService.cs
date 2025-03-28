using System.Net;
using System.Net.Mail;

namespace RecipeDictionaryApi.Services;

public class EmailService(IConfiguration configuration)
{
    private readonly IConfigurationSection _config = configuration.GetSection("EmailSender");
    private readonly Random _random = new();
    public async Task<int> SendEmail(string toAddress, string subject)
    {
        using var smtpClient = new SmtpClient("smtp.mail.ru");
        smtpClient.Port = 587;
        smtpClient.Credentials = new NetworkCredential(_config["email"], _config["password"]);

    smtpClient.EnableSsl = true;
        
        using var mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(_config["email"]!);
        mailMessage.Subject = subject;
        mailMessage.Body = _config["mail"] + GenerateCode(out var code);

        mailMessage.To.Add(toAddress);

        await smtpClient.SendMailAsync(mailMessage);

        return code;
    }

    private int GenerateCode(out int result)
    {
        result = _random.Next(100000, 999999);
        return result;
    }
}