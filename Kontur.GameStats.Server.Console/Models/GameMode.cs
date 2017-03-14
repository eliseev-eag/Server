using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Models
{
    public class GameMode
    {
        [Key]
        public string Name { get; set; }

        public virtual ICollection<ServerInfo> Servers { get; set; }
        public virtual ICollection<MatchResult> Matches { get; set; }

        public GameMode()
        {
            Servers = new HashSet<ServerInfo>();
            Matches = new HashSet<MatchResult>();
        }
    }
}