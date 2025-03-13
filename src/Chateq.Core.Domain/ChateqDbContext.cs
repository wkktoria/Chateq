using Chateq.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Chateq.Core.Domain;

public sealed class ChateqDbContext : DbContext
{
    public ChateqDbContext(DbContextOptions<ChateqDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Chat> Chats { get; set; }

    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.SenderId);

        modelBuilder.Entity<Chat>().HasData(
            new Chat
            {
                Id = Guid.NewGuid(),
                Name = "Global",
                CreatedAt = DateTime.Now
            }
        );
    }
}