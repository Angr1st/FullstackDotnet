using Microsoft.EntityFrameworkCore;

namespace Server.Database
{
    public class ServerDbContext : DbContext
    {
        public ServerDbContext(DbContextOptions<ServerDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserMessage>()
                .HasKey(userMessage => userMessage.UserMessageId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserMessage> UserMessages { get; set; }
    }
}
