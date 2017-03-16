using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontur.GameStats.Server.Models
{
    public class MatchResult
    {
        public long Id { get; set; }
        [Index]
        public DateTime Timestamp { get; set; }
        public int FragLimit { get; set; }
        public double TimeLimit { get; set; }
        public double TimeElapsed { get; set; }
        [Index]
        public virtual Map Map { get; set; }
        [Index]
        public virtual GameMode GameMode { get; set; }
        [Index]
        public virtual ServerInfo Server { get; set; }
        [Index]
        public virtual ICollection<ScoreboardRecord> ScoreBoard { get; set; }

        public MatchResult()
        {
            ScoreBoard = new HashSet<ScoreboardRecord>();
        }
    }
}
