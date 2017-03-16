using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontur.GameStats.Server.Models
{
    public class Player
    {
        [Key]
        [Index]
        public string Name { get; set; }
        public ICollection<ScoreboardRecord> Scores { get; set; }
         
        public Player()
        {
            Scores = new HashSet<ScoreboardRecord>();
        }
    }
}
