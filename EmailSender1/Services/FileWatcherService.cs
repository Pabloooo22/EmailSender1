using CsvHelper.Configuration;
using CsvHelper;
using EmailSender.Models;
using EmailSender.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Globalization;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using EmailSender.Controllers;
using System.Net.Mail;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.IO.Pipes;
using System.Xml;

namespace EmailSender.Services
{
    public interface IFileWatcherService
    {
        void NewFileAdded(object sender, FileSystemEventArgs e);
    }

    public class FileWatcherService : IFileWatcherService
    {
        private FileSystemWatcher watcher;
        private string watcherPath = "B:\\Projekty C++ - VS2022\\C#\\EmailSender1\\EmailSender1\\wwwroot\\XmlSendEmailFiles\\";

        public FileWatcherService()
        {
            FileSystemWatcher watcher = new FileSystemWatcher(watcherPath);
            watcher.Filter = "*.xml";
            watcher.EnableRaisingEvents = true;
            watcher.Created += NewFileAdded;
        }

        public async void NewFileAdded(object sender, FileSystemEventArgs e)
        {
            string filePath = e.FullPath;
            XmlEmailEntity emailDetails = new XmlEmailEntity();
            SendMailService mailService = new SendMailService();
            EmailReciverDbContext dbContext = new EmailReciverDbContext();
            //XmlSerializer serializer = new XmlSerializer(typeof(XmlEmailEntity));
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            using (XmlReader readerXml = XmlReader.Create(filePath, settings))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XmlEmailEntity));
                emailDetails = (XmlEmailEntity)serializer.Deserialize(readerXml);
            }
            //await using (FileStream fs = new FileStream(filePath, FileMode.Open))
            //{
            //    emailDetails = (XmlEmailEntity)serializer.Deserialize(fs);
            //}

            string attachmentPath = Path.Combine(watcherPath, emailDetails.EmailPostedFile);
            string adressBookFilePath = Path.Combine(watcherPath, emailDetails.ToAdressBook);

            if (!File.Exists(adressBookFilePath))
            {
                return;
            }
            using var reader = new StreamReader(adressBookFilePath);
            var conf = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                HeaderValidated = null
            };
            using var csvRead = new CsvReader(reader, conf);
            List<EmailReciver> recivers = csvRead.GetRecords<EmailReciver>().ToList();
            reader.Close();
            List<string> toEmails = recivers.Select(x => x.EmailAdress).ToList();

            if (File.Exists(attachmentPath))
            {
                using (var fileStream = System.IO.File.OpenRead(attachmentPath))
                {
                    IFormFile attachment = new FormFile(fileStream, 0, fileStream.Length, null, Path.GetFileName(attachmentPath))
                    {
                        Headers = new HeaderDictionary(),
                    };
                    mailService.Email(toEmails, emailDetails.EmailSubject, emailDetails.EmailContent, attachment, emailDetails.EmailsPerHour);
                }
                return;
            }
            mailService.Email(toEmails, emailDetails.EmailSubject, emailDetails.EmailContent, null, emailDetails.EmailsPerHour);
        }
    }
}
