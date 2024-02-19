using Microsoft.EntityFrameworkCore;

namespace EmailSender.Models.Entities
{
    public class EmailReciverDbContext : DbContext
    {
        private string _connectionString =
            "Server=localhost;Database=EmailReciversDb1;Trusted_Connection=True;Encrypt=False;";
        public DbSet<EmailReciver> emailRecivers { get; set; }
        public DbSet<AdressBook> adresses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);   
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailReciver>()
                .OwnsOne(c => c.csvInformations);

            //modelBuilder.Entity<EmailReciver>()
            //    .OwnsOne(c => c.csvInformations)
            //    .Property(n => n.Property1)
            //    .HasColumnName("Secondname");

        }
    }
}
