using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection.Emit;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
       
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EventModel>()
            .HasOne(e => e.User)
            .WithMany(u => u.Events)
            .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<UserTaskModel>()
         .HasOne(e => e.User)
         .WithMany(u => u.UserTasks)
         .HasForeignKey(e => e.UserId);
    }

    public DbSet<UserModel> Users{ get; set; }
    public DbSet<EventModel> Events { get; set; }
    public DbSet<UserTaskModel> Tasks { get; set; }
}