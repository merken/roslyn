using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using adventureworks.data.logic;

namespace adventureworks.data.context
{
    public class AdventureworksLogicContext : AdventureworksLogic
    {
        private readonly string connectionString;
        public AdventureworksLogicContext(string connectionString) : base()
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