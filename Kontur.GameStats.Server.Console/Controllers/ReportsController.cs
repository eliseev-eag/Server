using System.Web.Http;
using System.Web.Http.Description;
using Ninject.Extensions.Logging;
using Kontur.GameStats.Server.Models;
using Kontur.GameStats.Server.Responces;
using System.Linq;
using System.Collections.Generic;
using Kontur.GameStats.Server.Requests;
using System.Data.Entity;
using System;

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

            try
            {
                var response = new LinkedList<RecentMath>();
                var recentMathes =
                    db.MathesResults.Include("Server")
                    .Include("ScoreBoard").Include("GameMode")
                    .OrderByDescending(p => p.Timestamp).Take(count);
                foreach (var match in recentMathes)
                {
                    RecentMath responseRow = new RecentMath();
                    responseRow.Server = match.Server.Endpoint;
                    responseRow.Timestamp = match.Timestamp;
                    MatchResultRequest matchResults = new MatchResultRequest();
                    matchResults.Map = match.Map;
                    matchResults.GameMode = match.GameMode.Name;
                    matchResults.FragLimit = match.FragLimit;
                    matchResults.TimeLimit = match.TimeLimit;
                    matchResults.TimeElapsed = match.TimeElapsed;
                    matchResults.Scoreboard = new List<ScoreboardElement>(maxPlayersCountInMatch); ;
                    foreach (var score in match.ScoreBoard)
                    {
                        ScoreboardElement scoreElement = new ScoreboardElement();
                        scoreElement.Name = score.Player;
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
            catch (Exception exception)
            {
                logger.Error(exception, "Exception on reports/recent-matches/{0} request", count);
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("reports/popular-servers/{count?}")]
        [ResponseType(typeof(IEnumerable<PopularServers>))]
        public IHttpActionResult GetPopularServers([FromUri]int count = 5)
        {
            if (count <= 0) return Ok(Enumerable.Empty<PopularServers>());

            if (count > 50) count = 50;
            try
            {
                var lastDay = db.MathesResults.Max(x => DbFunctions.TruncateTime(x.Timestamp));
                var query = from server in db.Servers
                            let firstDay = server.Mathes.Min(y => y.Timestamp)
                            select new PopularServers
                            {
                                AverageMatchesPerDay = ((double)server.Mathes.Count() / (DbFunctions.DiffDays(firstDay, lastDay) + 1.0) ?? 0.0),
                                Endpoint = server.Endpoint,
                                Name = server.Name
                            };
                var response = query
                    .OrderByDescending(n => n.AverageMatchesPerDay).Take(count);
                return Ok(response);
            }
            catch(Exception exception)
            {
                logger.Error(exception, "Exception on reports/popular-servers/{0} request", count);
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("reports/best-players/{count?}")]
        [ResponseType(typeof(IEnumerable<PopularServers>))]
        public IHttpActionResult GetBestPlayers([FromUri]int count = 5)
        {
            const int minMathesCount = 10;
            if (count <= 0) return Ok(Enumerable.Empty<PopularServers>());

            if (count > 50) count = 50;
            try
            {
                var players = db.ScoreboardRecords.GroupBy(t => t.Player)
              .Where(p => p.Count() >= minMathesCount && p.Sum(d => d.Deaths) > 0)
              .Select(e => new { name = e.Key, killToDeathRatio = (double)e.Sum(k => k.Kills) / e.Sum(d => d.Deaths) });
                var response = players.OrderByDescending(t => t.killToDeathRatio);
                return Ok(response);
            }
            catch(Exception exception)
            {
                logger.Error(exception, "Exception on reports/best-players/{0} request", count);
                return InternalServerError();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
