using Microsoft.EntityFrameworkCore;
using WebApplicationEntity.Models;

namespace WebApplicationEntity.Data
{
    public class ModelContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<SubPicture> SubPictures { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source = 127.0.0.1; Initial Catalog = test_entity; Persist Security Info = True; User ID = admin; Password = 12345678; Multiple Active Result Sets = True");
        }
    }
}
