using System.Net;
using System.Net.Mail;
using HotelManagementSystem.Application;
namespace HotelManagementSystem.Infrastructure;
public class SmtpEmailSender(IConfiguration config):IEmailSender
{
 public async Task SendAsync(string recipient,string subject,string body){var host=config["Smtp:Host"];var sender=config["Smtp:SenderEmail"];var password=config["Smtp:Password"];if(string.IsNullOrWhiteSpace(host)||string.IsNullOrWhiteSpace(sender)||string.IsNullOrWhiteSpace(password))throw new DomainException("Email is not configured in Docker. Set your Brevo API key and verified sender email before starting Docker.");using var client=new SmtpClient(host,int.Parse(config["Smtp:Port"]??"587")){EnableSsl=true,Credentials=new NetworkCredential(sender,password)};using var message=new MailMessage(sender,recipient,subject,body);await client.SendMailAsync(message);}
}
