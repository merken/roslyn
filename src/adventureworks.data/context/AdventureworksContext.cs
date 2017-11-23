using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using adventureworks.data.model;

namespace adventureworks.data.context
{
        public class AdventureworksContext : Adventureworks
        {
            private readonly string connectionString;
            public AdventureworksContext(string connectionString) : base()
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