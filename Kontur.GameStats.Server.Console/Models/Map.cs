using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Models
{
    public class Map
    {
        [Key]
        public string Name { get; set; }
        public virtual ICollection<MatchResult> Matches { get; set; } 

        public Map()
        {
            Matches = new HashSet<MatchResult>();
        }
    }
}
