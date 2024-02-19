namespace EmailSender.Models.Entities
{
    public class AdressBook
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<EmailReciver> EmailRecivers { get; set; }

    }
}
