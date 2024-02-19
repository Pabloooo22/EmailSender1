using CsvHelper.Configuration;
using CsvHelper;
using EmailSender.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace EmailSender.Services
{
    public interface IAdressBookService
    {
        void UploadFile(IFormFile postedFile);
        void DeleteAddresBook(int id);
        IEnumerable<SelectListItem> GetAdressBookList();
        List<string> GetEmailsListFromAdressBook(string adresBook);
        bool IsNameUsed(IFormFile postedFile);
    }

    public class AdressBookService : IAdressBookService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly EmailReciverDbContext _dbContext;

        public AdressBookService(IWebHostEnvironment environment, EmailReciverDbContext dbContext)
        {
            _environment = environment;
            _dbContext = dbContext;
        }
        public void UploadFile(IFormFile postedFile)
        {
            string path = Path.Combine(_environment.WebRootPath, "Uploads\\");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var title = Path.GetFileName(postedFile.FileName);
            string filePath = path + Path.GetFileName(postedFile.FileName);
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                postedFile.CopyTo(stream);
            }
            AdressBook adressBook = new AdressBook();
            using var reader = new StreamReader(filePath);
            var conf = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                HeaderValidated = null
            };
            using var csvRead = new CsvReader(reader, conf);
            List<EmailReciver> recivers = csvRead.GetRecords<EmailReciver>().ToList();
            adressBook.Title = title;
            adressBook.EmailRecivers = recivers;
            reader.Close();
            _dbContext.adresses.AddRange(adressBook);
            _dbContext.SaveChanges();
        }

        public bool IsNameUsed(IFormFile postedFile)
        {
            var title = Path.GetFileName(postedFile.FileName);
            if (_dbContext.adresses.Any(a => a.Title == title))
            {
                return false;
            }
            return true;
        }

        public void DeleteAddresBook(int id)
        {
            if(_dbContext.adresses.FirstOrDefault(a => a.Id == id) != null)
            {
                while (_dbContext.emailRecivers.FirstOrDefault(a => a.AdressesBookId == id) != null)
                {
                    var emailReciver = _dbContext.emailRecivers.FirstOrDefault(a => a.AdressesBookId == id);
                    _dbContext.emailRecivers.Remove(emailReciver);
                    _dbContext.SaveChanges();
                }

                var adressBook = _dbContext.adresses.FirstOrDefault(a => a.Id == id);
                _dbContext.adresses.Remove(adressBook);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<SelectListItem> GetAdressBookList()
        {
            IEnumerable<SelectListItem> adressBooks = _dbContext.adresses.Select(i => new SelectListItem
            {
                Text = i.Title
            });
            return adressBooks;
        }

        public List<string> GetEmailsListFromAdressBook(string adressBook)
        {
            var _adressBook = _dbContext.adresses.FirstOrDefault(i => i.Title == adressBook);
            var emailsList = _dbContext.emailRecivers.Where(x => x.AdressesBookId == _adressBook.Id).Select(x => x.EmailAdress).ToList();
            return emailsList;
        }
    }
}
