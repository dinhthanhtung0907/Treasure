using Microsoft.EntityFrameworkCore;
using Treasure.Data.Entitites;

namespace Treasure.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext() : base() { }

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<MatrixInputHistory> MatrixInputHistories { get; set; }

        /// <summary>
        /// OnConfiguring
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        /// <summary>
        /// OnModelCreating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
