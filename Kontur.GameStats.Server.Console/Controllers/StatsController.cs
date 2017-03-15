using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Ninject.Extensions.Logging;
using Kontur.GameStats.Server.Models;
using Kontur.GameStats.Server.Responces;

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
                response.AverageMatchesPerDay = matchesGroupByDate.Average(p => p.Count());
                response.MaximumPopulation = matches.Max(m => m.ScoreBoard.Count());
                response.AveragePopulation = matches.Average(m => m.ScoreBoard.Count());
                response.Top5GameModes = server.GameModes.OrderByDescending(p => p.Matches.Count()).Select(m => m.Name).Take(5);
                response.Top5Maps = matches.GroupBy(p => p.Map).OrderByDescending(m => m.Count()).Select(n => n.Key.Name).Take(5);
            }
            else
            {
                response.Top5GameModes = Enumerable.Empty<string>();
                response.Top5Maps = Enumerable.Empty<string>();
            }
            return Ok(response);
        }
    }
}
