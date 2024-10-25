using Microsoft.EntityFrameworkCore;
using ChessLiteServerAPI.Models;

namespace ChessLiteServerAPI.Data
{
    public class ChessLiteServerAPIContext : DbContext
    {
        public ChessLiteServerAPIContext(DbContextOptions<ChessLiteServerAPIContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Game> Games { get; set; } = default!;
        public DbSet<GameStep> GameSteps { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .HasMany(g => g.GameSteps)
                .WithOne(gs => gs.Game)
                .HasForeignKey(gs => gs.GameId)
                .OnDelete(DeleteBehavior.Cascade); // Ensure steps are deleted if the game is deleted

            modelBuilder.Entity<User>()
                .HasMany(u => u.Games)
                .WithOne(g => g.User)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Ensure games are deleted if the user is deleted

            base.OnModelCreating(modelBuilder);
        }
    }
}
