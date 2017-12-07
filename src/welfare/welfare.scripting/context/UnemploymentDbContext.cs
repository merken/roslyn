using Microsoft.EntityFrameworkCore;
using welfare.scripting.model;

namespace welfare.scripting.context
{
    public class UnemploymentDbContext : UnemploymentContext
    {
        private readonly string connectionString;
        public UnemploymentDbContext(string connectionString) : base()
        {
            this.connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}