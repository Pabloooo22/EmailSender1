using CsvHelper.Configuration;
using CsvHelper;
using EmailSender.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace EmailSender.Services
{
    public class SendMailService
    {
        public async void Email(List<string> sendTo, string emailSubject, string emailMessage, IFormFile emailPostedFile)
        {
            var appConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var username = appConfig.GetValue<string>("EmailConfig:Username");
            var password = appConfig.GetValue<string>("EmailConfig:Password");
            var host = appConfig.GetValue<string>("EmailConfig:Host");
            var port = int.Parse(appConfig.GetValue<string>("EmailConfig:Port"));

            SmtpClient smtpClient = new();
            smtpClient.Host = host;
            smtpClient.Port = port;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(username, password);
            smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

            foreach (var email in sendTo)
            {
                MailMessage mailMessage = new();
                mailMessage.To.Add(new MailAddress(email));
                mailMessage.From = new MailAddress("profaska.p@gmail.com");
                mailMessage.Subject = emailSubject;
                mailMessage.Body = emailMessage;
                mailMessage.IsBodyHtml = true;
                if (emailPostedFile != null)
                {
                    mailMessage.Attachments.Add(new Attachment(emailPostedFile.OpenReadStream(), emailPostedFile.FileName));
                }
                mailMessage.Priority = MailPriority.Normal;
                smtpClient.Send(mailMessage);
                Thread.Sleep(36000);
            }
        }

    }
}