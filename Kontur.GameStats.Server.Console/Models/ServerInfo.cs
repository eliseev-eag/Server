﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontur.GameStats.Server.Models
{
    public class ServerInfo
    {
        [Index]
        public int Id { get; set; }
        [Index(IsUnique = true)]
        public string Endpoint { get; set; }
        public string Name { get; set; }

        [Index]
        public virtual ICollection<GameMode> GameModes { get; set; }
        [Index]
        public virtual ICollection<MatchResult> Mathes { get; set; }
        public ServerInfo()
        {
            GameModes = new HashSet<GameMode>();
            Mathes = new HashSet<MatchResult>();
        }
    }
}