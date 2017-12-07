using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace welfare.scripting.model
{
    public partial class UnemploymentContext : DbContext
    {
        public virtual DbSet<HandicapType> HandicapType { get; set; }
        public virtual DbSet<MaritalStatus> MaritalStatus { get; set; }
        public virtual DbSet<WelfareLogic> WelfareLogic { get; set; }
        public virtual DbSet<WelfareScript> WelfareScript { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Data Source=127.0.0.1;Initial Catalog=unemployment;persist security info=True;user id=sa;password=abc123$%");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MaritalStatus>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<WelfareLogic>(entity =>
            {
                entity.Property(e => e.IdHandicap).HasColumnName("idHandicap");

                entity.HasOne(d => d.IdHandicapNavigation)
                    .WithMany(p => p.WelfareLogic)
                    .HasForeignKey(d => d.IdHandicap)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WelfareLogic_HandicapType");

                entity.HasOne(d => d.IdMaritalStatusNavigation)
                    .WithMany(p => p.WelfareLogic)
                    .HasForeignKey(d => d.IdMaritalStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WelfareLogic_MaritalStatus");
            });

            modelBuilder.Entity<WelfareScript>(entity =>
            {
                entity.Property(e => e.Middleware)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });
        }
    }
}
