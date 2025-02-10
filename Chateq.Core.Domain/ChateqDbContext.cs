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
}