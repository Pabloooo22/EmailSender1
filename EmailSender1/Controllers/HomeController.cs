using EmailSender.Models;
using EmailSender.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace EmailSender.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
 
        [HttpPost]
        public IActionResult SendEmail(EmailEntity emailParameters, SendMailService mailService, IFormCollection formValues)
        {
            emailParameters.EmailContent = formValues["EmailContent"];

            try
            {
                mailService.Email(emailParameters.ToEmailAdress, emailParameters.EmailSubject, emailParameters.EmailContent);
                TempData["alert"] = "Email Sucessfully Sent";
            }
            catch (Exception)
            {
                TempData["alert"] = "Problem Sending mail please check the configuration";

            }

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
