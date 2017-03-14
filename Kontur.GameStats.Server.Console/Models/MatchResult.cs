using System.Collections.Generic;

namespace Kontur.GameStats.Server.Models
{
    public class MatchResult
    {
        public long Id { get; set; }
        public string Map { get; set; }
        public double FragLimit { get; set; }
        public double TimeLimit { get; set; }
        public double TimeEllapsed { get; set; }

        public virtual GameMode GameMode { get; set; }
        public virtual ServerInfo Server { get; set; }
        public virtual ICollection<ScoreboardRecord> ScoreBoard { get; set; }

        public MatchResult()
        {
            ScoreBoard = new HashSet<ScoreboardRecord>();
        }
    }
}
