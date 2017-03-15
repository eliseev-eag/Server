﻿using System.Web.Http;
using System.Web.Http.Description;
using Ninject.Extensions.Logging;
using Kontur.GameStats.Server.Models;
using Kontur.GameStats.Server.Responces;
using System.Linq;
using System.Collections.Generic;
using Kontur.GameStats.Server.Requests;
using System.Data.Entity;

namespace Kontur.GameStats.Server.Controllers
{
    public class ReportsController : ApiController
    {
        private const int maxPlayersCountInMatch = 100;
        private readonly ILogger logger;
        private DatabaseContext db = new DatabaseContext();

        public ReportsController(ILogger logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("reports/recent-matches/{count?}")]
        [ResponseType(typeof(RecentMath))]
        public IHttpActionResult GetRecentMathes([FromUri]int count = 5)
        {
            if (count <= 0) return Ok(Enumerable.Empty<RecentMath>());

            if (count > 50) count = 50;

            var response = new LinkedList<RecentMath>();
            var recentMathes =
                db.MathesResults.Include("Server")
                .Include("ScoreBoard").Include("Map")
                .Include("GameMode")
                .OrderByDescending(p => p.Timestamp).Take(count);
            foreach (var match in recentMathes)
            {
                RecentMath responseRow = new RecentMath();
                responseRow.Server = match.Server.Endpoint;
                responseRow.Timestamp = match.Timestamp;
                MatchResultRequest matchResults = new MatchResultRequest();
                matchResults.Map = match.Map.Name;
                matchResults.GameMode = match.GameMode.Name;
                matchResults.FragLimit = match.FragLimit;
                matchResults.TimeLimit = match.TimeLimit;
                matchResults.TimeElapsed = match.TimeElapsed;
                matchResults.Scoreboard = new List<ScoreboardElement>(maxPlayersCountInMatch); ;
                foreach (var score in match.ScoreBoard)
                {
                    ScoreboardElement scoreElement = new ScoreboardElement();
                    scoreElement.Name = score.Player.Name;
                    scoreElement.Deaths = score.Deaths;
                    scoreElement.Kills = score.Kills;
                    scoreElement.Frags = score.Frags;
                    matchResults.Scoreboard.Add(scoreElement);
                }

                responseRow.Results = matchResults;

                response.AddLast(responseRow);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("reports/popular-servers/{count?}")]
        [ResponseType(typeof(IEnumerable<PopularServers>))]
        public IHttpActionResult GetPopularServers([FromUri]int count = 5)
        {
            if (count <= 0) return Ok(Enumerable.Empty<PopularServers>());

            if (count > 50) count = 50;

            var response = db.Servers.
                Select(p => new PopularServers
                {
                    AverageMatchesPerDay = p.Mathes.GroupBy(m => DbFunctions.TruncateTime(m.Timestamp)).Average(s => (int?)s.Count()) ?? 0,
                    Endpoint = p.Endpoint,
                    Name = p.Name
                })
                .OrderByDescending(n => n.AverageMatchesPerDay).Take(count);
            return Ok(response);
        }

    }
}