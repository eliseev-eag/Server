using System.Collections.Generic;

namespace Kontur.GameStats.Server.Models
{
    public class Player
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<ScoreboardRecord> Scores { get; set; }
         
        public Player()
        {
            Scores = new HashSet<ScoreboardRecord>();
        }
    }
}
