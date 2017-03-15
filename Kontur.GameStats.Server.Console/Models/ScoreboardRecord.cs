namespace Kontur.GameStats.Server.Models
{
    public class ScoreboardRecord
    {
        public long Id { get; set; }
        public int Frags { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public double ScoreboardPercent { get; set; }
        public virtual MatchResult Match { get; set; }
        public virtual Player Player { get; set; }
    }
}
