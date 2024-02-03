using System.Net;
using System.Net.Mail;

namespace EmailSender.Services
{
    public class SendMailService
    {
        public void Email(string sendTo, string emailSubject, string emailMessage)
        {
            var appConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var username = appConfig.GetValue<string>("EmailConfig:Username");
            var password = appConfig.GetValue<string>("EmailConfig:Password");
            var host = appConfig.GetValue<string>("EmailConfig:Host");
            var port = int.Parse(appConfig.GetValue<string>("EmailConfig:Port"));

            MailMessage mailMessage = new();
            mailMessage.To.Add(new MailAddress(sendTo));
            mailMessage.From = new MailAddress("profaska.p@gmail.com");
            mailMessage.Subject = emailSubject;
            mailMessage.Body = emailMessage;
            mailMessage.IsBodyHtml = true;
            mailMessage.Priority = MailPriority.Normal;
            SmtpClient smtpClient = new();
            smtpClient.Host = host;
            smtpClient.Port = port;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(username, password);
            smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            smtpClient.Send(mailMessage);
        }
    }
}
