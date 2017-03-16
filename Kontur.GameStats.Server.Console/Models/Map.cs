using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontur.GameStats.Server.Models
{
    public class Map
    {
        [Key]
        [Index]
        public string Name { get; set; }
        public virtual ICollection<MatchResult> Matches { get; set; } 

        public Map()
        {
            Matches = new HashSet<MatchResult>();
        }
    }
}
