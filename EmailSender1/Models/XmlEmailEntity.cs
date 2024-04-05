using System.Xml.Serialization;

namespace EmailSender.Models
{
    [XmlRoot(ElementName = "SendingDetails")]
    public class XmlEmailEntity
    {
        [XmlElement(ElementName = "ToAdressBook")]
        public string ToAdressBook { get; set; }

        [XmlElement(ElementName = "EmailSubject")]
        public string EmailSubject { get; set; }

        [XmlElement(ElementName = "EmailContent")]
        public string EmailContent { get; set; }

        [XmlElement(ElementName = "EmailPostedFile")]
        public string EmailPostedFile { get; set; }

        [XmlElement(ElementName = "EmailsPerHour")]
        public int EmailsPerHour { get; set; } = 100;
    }
}
