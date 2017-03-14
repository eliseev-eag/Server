using Kontur.GameStats.Server.Models;
using Kontur.GameStats.Server.Requests;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace Kontur.GameStats.Server.Controllers
{
    public class MatchResultController : ApiController
    {
        private readonly ILogger logger;
        private DatabaseContext db = new DatabaseContext();

        public MatchResultController(ILogger logger)
        {
            this.logger = logger;
        }


        [HttpPut]
        [Route("servers/{endpoint}/matches/{timestamp}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult SaveServerInfo(string endpoint, DateTime timestamp, [FromBody]MatchResultRequest matchResultRequest)
        {
            if (!ModelState.IsValid)
            {
                logger.Info("Put запрос servers/{0}/matches/{1} не корректен", endpoint, timestamp);
                return BadRequest(ModelState);
            }

            ServerInfo server;
            try
            {
                server = db.Servers.Single(rec => rec.Endpoint == endpoint);
            }
            catch (InvalidOperationException)
            {
                logger.Info("Put запрос servers/{0}/matches/{1}. Server с заданным endpoint {0} не найден", endpoint, timestamp);
                return BadRequest();
            }
            GameMode gameMode;
            try
            {
                gameMode = db.GameModes.Single(rec => rec.Name == matchResultRequest.GameMode);
            }
            catch (InvalidOperationException)
            {
                logger.Info("Put запрос servers/{0}/matches/{1}. Не корректен GameMode", endpoint, timestamp);
                return BadRequest();
            }

            // try
            {
                logger.Info("Put запрос servers/{0}/matches/{1} добавляю запись", endpoint, timestamp);
                MatchResult matchResult = new MatchResult();
                matchResult.Server = server;
                matchResult.Timestamp = timestamp;
                matchResult.Map = matchResultRequest.Map;
                matchResult.GameMode = gameMode;
                matchResult.FragLimit = matchResultRequest.FragLimit;
                matchResult.TimeLimit = matchResultRequest.TimeLimit;
                matchResult.TimeEllapsed = matchResultRequest.TimeEllapsed;
                SaveScoreboard(matchResultRequest.Scoreboard, matchResult);

                db.SaveChanges();
            }
            /*          catch (Exception exception)
                      {
                          logger.ErrorException("Put запрос servers/{0}/info", exception);
                          return InternalServerError();
                      }
                      */
            return Ok();
        }

        private void SaveScoreboard(List<ScoreboardElement> scoreboard, MatchResult match)
        {
            for (int i = 0; i < scoreboard.Count; i++)
            {
                var scoreboardRow = scoreboard[i];
                Player player;
                try { player = db.Players.Single(p => p.Name == scoreboardRow.Name); }
                catch (InvalidOperationException)
                {
                    player = new Player() { Name = scoreboardRow.Name };
                    db.Players.Add(player);
                }
                ScoreboardRecord record = new ScoreboardRecord();
                record.Kills = scoreboardRow.Kills;
                record.Frags = scoreboardRow.Frags;
                record.Deaths = scoreboardRow.Deaths;
                record.Player = player;
                record.ScoreboardPosition = i+1;
                record.Match = match;

            }
        }
        /*
        [HttpGet]
        [Route("servers/{endpoint}/info")]
        [ResponseType(typeof(AdvertiseRequest))]
        public IHttpActionResult GetServerInfo(string endpoint)
        {
            try
            {
                if (!validIpAddressRegex.IsMatch(endpoint) && !validHostnameRegex.IsMatch(endpoint))
                {
                    logger.Info("GET запрос servers/{0}/info не корректен", endpoint);
                    return BadRequest();
                }
            }
            catch (ArgumentNullException)
            {
                logger.Info("GET запрос servers/{0}/info не корректен", endpoint);
                return BadRequest();
            }
            try
            {
                ServerInfo serverInfo = db.Servers.Find(endpoint);
                if (serverInfo == null)
                {
                    return NotFound();
                }

                AdvertiseRequest response = ExtractServerInfo(serverInfo);
                return Ok(response);
            }
            catch (Exception exception)
            {
                logger.ErrorException("Put запрос servers/{0}/info", exception);
                return InternalServerError();
            }
        }*/

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
