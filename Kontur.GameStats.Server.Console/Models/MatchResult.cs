using System;
using System.Collections.Generic;

namespace Kontur.GameStats.Server.Models
{
    public class MatchResult
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int FragLimit { get; set; }
        public double TimeLimit { get; set; }
        public double TimeElapsed { get; set; }

        public virtual Map Map { get; set; }
        public virtual GameMode GameMode { get; set; }
        public virtual ServerInfo Server { get; set; }
        public virtual ICollection<ScoreboardRecord> ScoreBoard { get; set; }

        public MatchResult()
        {
            ScoreBoard = new HashSet<ScoreboardRecord>();
        }
    }
}
