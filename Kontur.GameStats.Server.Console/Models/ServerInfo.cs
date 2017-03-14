using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Models
{
    public class ServerInfo
    {
        [Key]
        public string Endpoint { get; set; }
        public string Name { get; set; }
        
        public virtual ICollection<GameMode> GameModes { get; set; }
        public virtual ICollection<MatchResult> Mathes { get; set; }
        public ServerInfo()
        {
            GameModes = new HashSet<GameMode>();
            Mathes = new HashSet<MatchResult>();
        }
    }
}