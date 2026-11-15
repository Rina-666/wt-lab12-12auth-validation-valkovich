using Microsoft.EntityFrameworkCore;
using wt_lab8_8razor_postgresql_valkovich.Models;

namespace wt_lab8_8razor_postgresql_valkovich.Data
{
    public class AppDbContext : DbContext
    {
        // Конструктор, который принимает настройки подключения из Program.cs
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Таблица "Books" в базе данных
        public DbSet<Book> Books => Set<Book>();
    }
}