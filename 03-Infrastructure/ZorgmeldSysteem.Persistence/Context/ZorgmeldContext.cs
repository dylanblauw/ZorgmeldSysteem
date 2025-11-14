using Microsoft.EntityFrameworkCore;
using ZorgmeldSysteem.Domain.Entities;
using ZorgmeldSysteem.Domain.Enums;

namespace ZorgmeldSysteem.Persistence.Context
{
    public class ZorgmeldContext : DbContext
    {
        public ZorgmeldContext(DbContextOptions<ZorgmeldContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Company> Companies { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Mechanic> Mechanics { get; set; }
        public DbSet<Objects> Objects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserCompany> UserCompanies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===================================
            // USER CONFIGURATION
            // ===================================

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.UserLevel);
                entity.HasIndex(e => e.IsActive);

                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.UserLevel).IsRequired();
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.CreatedOn).IsRequired().HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UserLevel).HasConversion<int>();
                entity.Property(e => e.MechanicType).HasConversion<int>();
            });

            // ===================================
            // USERCOMPANY CONFIGURATION
            // ===================================

            modelBuilder.Entity<UserCompany>(entity =>
            {
                entity.HasKey(e => e.UserCompanyID);
                entity.HasIndex(e => e.UserID);
                entity.HasIndex(e => e.CompanyID);
                entity.HasIndex(e => new { e.UserID, e.CompanyID }).IsUnique();

                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.AddedOn).IsRequired().HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserCompanies)
                    .HasForeignKey(e => e.UserID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Company)
                    .WithMany(c => c.UserCompanies)
                    .HasForeignKey(e => e.CompanyID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.AddedBy)
                    .WithMany()
                    .HasForeignKey(e => e.AddedByUserID)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ===================================
            // TICKET CONFIGURATION
            // ===================================

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.TicketID);

                entity.HasOne(t => t.Company)
                    .WithMany(c => c.Tickets)
                    .HasForeignKey(t => t.CompanyID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Object)
                    .WithMany(o => o.Tickets)
                    .HasForeignKey(t => t.ObjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Mechanic)
                    .WithMany(u => u.AssignedTickets)
                    .HasForeignKey(t => t.MechanicID)
                    .HasPrincipalKey(u => u.UserID)
                    .OnDelete(DeleteBehavior.SetNull);
            });

        }
    }
}