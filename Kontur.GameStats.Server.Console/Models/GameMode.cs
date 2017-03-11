using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Models
{
    public class GameMode
    {
        [Key]
        public string Name { get; set; }
        public virtual ICollection<ServerInfo> Servers { get; set; }

        public GameMode()
        {
            Servers = new HashSet<ServerInfo>();
        }
    }
}