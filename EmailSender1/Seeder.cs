using CsvHelper;
using CsvHelper.Configuration;
using EmailSender.Models.Entities;
using System.Globalization;

namespace EmailSender
{
    public class Seeder
    {
        private EmailReciverDbContext _dbContext;
        private const string CSVpath = "EmailRecivers.csv";

        public Seeder(EmailReciverDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect()) 
            {
                if (!_dbContext.emailRecivers.Any())
                {
                    if (File.Exists(CSVpath))
                    {
                        var recivers = GetRecivers();
                        _dbContext.emailRecivers.AddRange(recivers);
                        _dbContext.SaveChanges();
                    }
                }
            }
        }

        private IEnumerable<EmailReciver> GetRecivers()
        {
            using var reader = new StreamReader(CSVpath);
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
            return recivers;
        }
    }
}
