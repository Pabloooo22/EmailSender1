using CsvHelper.Configuration;
using CsvHelper;
using EmailSender.Models;
using EmailSender.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Web;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace EmailSender.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAdressBookService _adressBookServiceService;

        public HomeController(ILogger<HomeController> logger, IAdressBookService adressBookServiceService)
        {
            _logger = logger;
            _adressBookServiceService = adressBookServiceService;
        }

        public IActionResult Index()
        {
            ViewBag.AdressBookList = _adressBookServiceService.GetAdressBookList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
 
        [HttpPost]
        public IActionResult SendEmail(EmailEntity emailParameters, SendMailService mailService)
        {
            try
            {
                var toEmails = _adressBookServiceService.GetEmailsListFromAdressBook(emailParameters.ToAdressBook);
                mailService.Email(toEmails, emailParameters.EmailSubject, emailParameters.EmailContent);
                TempData["alert"] = "Email Sucessfully Sent";
            }
            catch (Exception)
            {
                TempData["alert"] = "Problem Sending mail please check the configuration";

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Index(IFormFile postedFile)
        {
            if (postedFile != null && 
                Path.GetExtension(postedFile.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase) && 
                _adressBookServiceService.IsNameUsed(postedFile))
            {
                _adressBookServiceService.UploadFile(postedFile);
                TempData["alert"] = "File sucessfully upload";
            }
            else
            {
                TempData["alert"] = "Upload failed. File has the wrong extension or the name is taken";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete()
        {
            var adressBookId = 1;
            _adressBookServiceService.DeleteAddresBook(adressBookId);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
