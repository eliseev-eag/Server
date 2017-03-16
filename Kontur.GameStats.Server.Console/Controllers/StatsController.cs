﻿using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Ninject.Extensions.Logging;
using Kontur.GameStats.Server.Models;
using Kontur.GameStats.Server.Responces;
using System.Data.Entity;

namespace Kontur.GameStats.Server.Controllers
{
    public class StatsController : ApiController
    {

        private readonly ILogger logger;
        private DatabaseContext db = new DatabaseContext();

        public StatsController(ILogger logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("servers/{endpoint}/stats")]
        [ResponseType(typeof(ServerStats))]
        public IHttpActionResult GetServerStats([FromUri]string endpoint)
        {
            ServerInfo server;
            try
            {
                server = db.Servers.Include("Mathes").Include("GameModes").Single(rec => rec.Endpoint == endpoint);
            }
            catch (InvalidOperationException)
            {
                logger.Info("Get запрос servers/{0}/stats. Сервер с заданным endpoint {0} не найден", endpoint);
                return BadRequest();
            }
            ServerStats response = new ServerStats();
            var matches = server.Mathes;
            response.TotalMatchesPlayed = matches.Count();
            if (response.TotalMatchesPlayed != 0)
            {
                var matchesGroupByDate = matches.GroupBy(m => m.Timestamp.Date);
                response.MaximumMatchesPerDay = matchesGroupByDate.Max(p => p.Count());
                response.MaximumPopulation = matches.Max(m => m.ScoreBoard.Count());
                response.AveragePopulation = matches.Average(m => m.ScoreBoard.Count());
                response.Top5GameModes = server.GameModes.OrderByDescending(p => p.Matches.Count()).Select(m => m.Name).Take(5);
                response.Top5Maps = matches.GroupBy(p => p.Map).OrderByDescending(m => m.Count()).Select(n => n.Key).Take(5);

                var lastDay = db.MathesResults.Max(x => DbFunctions.TruncateTime(x.Timestamp));
                var firstDay = server.Mathes.Min(y => y.Timestamp);
                var dayCount = (lastDay.Value.Date - firstDay.Date).Days + 1.0;
                response.AverageMatchesPerDay = server.Mathes.Count() / dayCount;
            }
            else
            {
                response.Top5GameModes = Enumerable.Empty<string>();
                response.Top5Maps = Enumerable.Empty<string>();
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("players/{playerName}/stats")]
        [ResponseType(typeof(PlayerStats))]
        public IHttpActionResult GetPlayersStats([FromUri]string playerName)
        {
            var scores = db.ScoreboardRecords.Where(p => string.Compare(p.Player, playerName, true) == 0).ToList();
            if (scores.Count == 0)
            {
                logger.Info("Get запрос players/{0}/stats. Игрок с заданным ником {0} не найден", playerName);
                return BadRequest();
            }
            //Player player;
            /*try
            {
                player = db.Players.Include("Scores").Single(rec => string.Compare(rec.Name, playerName, true) == 0);
            }
            catch (InvalidOperationException)
            {
                logger.Info("Get запрос players/{0}/stats. Игрок с заданным ником {0} не найден", playerName);
                return BadRequest();
            }*/

            PlayerStats response = new PlayerStats();
            response.TotalMatchesPlayed = scores.Count();
            response.TotalMatchesWon = scores.Count(p => p.ScoreboardPercent == 100);

            //scores.GroupBy(p => p.Match.GameMode.Name).OrderByDescending(n => n.Key.Count()).ToList();

            response.FavoriteGameMode = scores.GroupBy(p => p.Match.GameMode.Name).Select(x => x.Key).OrderByDescending(n => n.Count()).First();
            response.FavoriteServer = scores.GroupBy(p => p.Match.Server.Endpoint).Select(x => x.Key).OrderByDescending(n => n.Count()).First();
            response.MaximumMatchesPerDay = scores.GroupBy(p => p.Match.Timestamp.Date).Max(p => p.Count());
            response.UniqueServers = scores.GroupBy(p => p.Match.Server.Endpoint).Count();
            response.AverageScoreboardPercent = scores.Average(p => p.ScoreboardPercent);
            response.LastMatchPlayed = scores.Max(p => p.Match.Timestamp);

            var lastDay = db.MathesResults.Max(x => DbFunctions.TruncateTime(x.Timestamp));
            var firstDay = scores.Min(y => y.Match.Timestamp);
            var dayCount = (lastDay.Value.Date - firstDay.Date).Days + 1.0;
            response.AverageMatchesPerDay = scores.Count() / dayCount;

            response.KillToDeathRatio = (double)scores.Sum(k => k.Kills) / scores.Sum(d => d.Deaths);
            if (double.IsInfinity(response.KillToDeathRatio)) response.KillToDeathRatio = 0;

            return Ok(response);
        }
    }
}
