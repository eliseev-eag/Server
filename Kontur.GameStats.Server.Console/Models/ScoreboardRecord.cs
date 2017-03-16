using System.ComponentModel.DataAnnotations.Schema;

namespace Kontur.GameStats.Server.Models
{
    public class ScoreboardRecord
    {
        [Index]
        public long Id { get; set; }
        public int Frags { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public double ScoreboardPercent { get; set; }
        [Index]
        public virtual MatchResult Match { get; set; }
        [Index]
        public virtual Player Player { get; set; }
    }
}
