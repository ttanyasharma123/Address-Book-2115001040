using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Model.Entities;

namespace RepositoryLayer.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        // Address Book Contacts Table
        public DbSet<Contact> Contacts { get; set; }

        // User Authentication Table
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Explicitly define the relationship
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.User)         // Contact belongs to one User
                .WithMany(u => u.Contacts)   // User has many Contacts
                .HasForeignKey(c => c.UserId) // Foreign Key
                .OnDelete(DeleteBehavior.Cascade); // If user is deleted, delete contacts
        }

    }
}
