using System.Data.Entity;

namespace Kontur.GameStats.Server.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("name = DatabaseContext") { }
        public virtual DbSet<ServerInfo> Servers { get; set; }
        public virtual DbSet<GameMode> GameModes { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}