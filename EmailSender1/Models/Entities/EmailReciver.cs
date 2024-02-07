using System.ComponentModel.DataAnnotations;

namespace EmailSender.Models.Entities
{
    public class EmailReciver
    {
        public required int Id { get; set; }
        public string? Name { get; set; }
        public string? Surename { get; set; }
        [EmailAddress]
        public string? EmailAdress { get; set; }
        public CsvInformations? csvInformations { get; set; }
    }
}
