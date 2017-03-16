using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontur.GameStats.Server.Models
{
    public class GameMode
    {
        [Key]
        public string Name { get; set; }
        [Index]
        public virtual ICollection<ServerInfo> Servers { get; set; }
        [Index]
        public virtual ICollection<MatchResult> Matches { get; set; }

        public GameMode()
        {
            Servers = new HashSet<ServerInfo>();
            Matches = new HashSet<MatchResult>();
        }
    }
}