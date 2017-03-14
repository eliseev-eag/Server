using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Models
{
    public class Player
    {
        //public long Id { get; set; }
        [Key]
        public string Name { get; set; }
        public ICollection<ScoreboardRecord> Scores { get; set; }
         
        public Player()
        {
            Scores = new HashSet<ScoreboardRecord>();
        }
    }
}
