using System.ComponentModel.DataAnnotations;

namespace EmailSender.Models
{
    public class EmailEntity
    {
        [EmailAddress]
        public string ToEmailAdress { get; set; }
        public string EmailSubject { get; set; }
        public string EmailContent { get; set; }
    }
}
