using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using DAL.Models;

namespace DAL.Database.Context
{
    public class ChatDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messaages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings[0].ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .IsRequired();
        }
    }
}
