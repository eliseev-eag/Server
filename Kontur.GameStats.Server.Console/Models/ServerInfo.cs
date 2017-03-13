using LiteDB;
using System.Collections.Generic;

namespace Kontur.GameStats.Server.Models
{
    public class ServerInfo
    {
        //public ObjectId Id { get; set; }
        [BsonId]
        public string Endpoint { get; set; }
        public string Name { get; set; }
        
        public ICollection<string> GameModes { get; set; }

        public ServerInfo()
        {
            GameModes = new HashSet<string>();
        }
    }
}