using Microsoft.EntityFrameworkCore;
using LanguageLearningApp.Domain; // Bu adımı kendi domain klasörünüzle eşleştirin.

namespace LanguageLearningApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tablolar
        public DbSet<Conversation> Conversations { get; set; } 
        public DbSet<Message> Messages { get; set; }

        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // İleride tablo/kolon konfigürasyonu yapacaksak burada ekleyebiliriz.
        }
    }
}
