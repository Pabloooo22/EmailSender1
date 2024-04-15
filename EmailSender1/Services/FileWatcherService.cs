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
using System.Net;

namespace EmailSender.Services
{

    public interface IFileWatcherService
    {
        string[] GetFilesList(string ftpServer, string username, string password, string folderPath);
        void NewFile();
        void NewFileAdded(object sender, FileSystemEventArgs e);
        void WatchFTPFolder();
    }

    public class FileWatcherService : IFileWatcherService
    {
        private string watcherPath = "B:\\Projekty C++ - VS2022\\C#\\EmailSender1\\EmailSender1\\wwwroot\\XmlSendEmailFiles\\";
        private bool downloaded = false;
        private string downloladedFilePath;

        public FileWatcherService()
        {
            FileSystemWatcher watcher = new FileSystemWatcher(watcherPath);
            watcher.Filter = "*.xml";
            watcher.EnableRaisingEvents = true;
            watcher.Created += NewFileAdded;
            Thread monitoringThread = new Thread(() =>
            {
                WatchFTPFolder();
            });
            monitoringThread.IsBackground = true;
            monitoringThread.Start();
        }

        public void WatchFTPFolder()
        {
            var appConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string ftpServer = appConfig.GetValue<string>("FTPConfig:ftpServer");
            string username = appConfig.GetValue<string>("FTPConfig:ftpUsername");
            string password = appConfig.GetValue<string>("FTPConfig:ftpPassword");

            string folderPath = appConfig.GetValue<string>("FTPConfig:ftpMonitoringFolder");

            string[] previousFiles = GetFilesList(ftpServer, username, password, folderPath);

            while (true)
            {
                string[] currentFiles = GetFilesList(ftpServer, username, password, folderPath);

                foreach (string file in currentFiles)
                {
                    if (!Array.Exists(previousFiles, element => element == file))
                    {
                        using (WebClient ftpClient = new WebClient())
                        {
                            string ftpFilePath = Path.Combine(folderPath, file);
                            ftpClient.Credentials = new NetworkCredential(username, password);
                            ftpClient.DownloadFile(ftpServer + ftpFilePath, Path.Combine(watcherPath, file));
                        }
                    }
                }
                if (downloaded)
                {
                    NewFile();
                    downloaded = false;
                 }
                previousFiles = currentFiles;

                Thread.Sleep(5000);
            }
        }

        public string[] GetFilesList(string ftpServer, string username, string password, string folderPath)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServer + folderPath);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(username, password);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                string fileList = reader.ReadToEnd();
                string[] files = fileList.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                reader.Close();
                response.Close();

                return files;
            }
            catch (WebException ex)
            {
                return new string[0];
            }
        }

        public async void NewFileAdded(object sender, FileSystemEventArgs e)
        {
            downloladedFilePath = e.FullPath;
            downloaded = true;
        }

        public async void NewFile()
        {
            string filePath = downloladedFilePath;
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
