using System.Net.Http.Json;
using HotelManagementSystem.Application;
namespace HotelManagementSystem.Infrastructure;
public class BrevoEmailSender(HttpClient client,IConfiguration config):IEmailSender
{
 public async Task SendAsync(string recipient,string subject,string body){var key=config["Brevo:ApiKey"];var sender=config["Brevo:SenderEmail"];if(string.IsNullOrWhiteSpace(key)||string.IsNullOrWhiteSpace(sender))throw new DomainException("Email is not configured in Docker. Set your Brevo API key and verified sender email before starting Docker.");using var request=new HttpRequestMessage(HttpMethod.Post,"https://api.brevo.com/v3/smtp/email"){Content=JsonContent.Create(new{sender=new{email=sender,name="Vacay"},to=new[]{new{email=recipient}},subject,textContent=body})};request.Headers.Add("api-key",key);var response=await client.SendAsync(request);if(!response.IsSuccessStatusCode)throw new DomainException("Brevo could not send the email. Check that the API key and sender Gmail are correct and verified in Brevo.");}
}
