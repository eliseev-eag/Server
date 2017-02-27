using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Models
{
    public class GameMode
    {
        [Key]
        public string Name { get; set; }
    }
}