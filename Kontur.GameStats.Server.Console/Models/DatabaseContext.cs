using System.Data.Entity;

namespace Kontur.GameStats.Server.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("name = DatabaseContext") { }
        public DbSet<ServerInfo> ServerInfos { get; set; }
        public DbSet<GameMode> GameModes { get; set; }
    }
}