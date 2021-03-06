﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Ninject.Extensions.Logging;
using Kontur.GameStats.Server.Models;
using Kontur.GameStats.Server.Requests;
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
        [Route("servers/{endpoint}/matches/{time}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult SaveMatchResult([FromUri]string endpoint, [FromUri]DateTime time, [FromBody]MatchResultRequest matchResultRequest)
        {

            time = time.ToUniversalTime();
            if (!ModelState.IsValid)
            {
                logger.Info("Put запрос servers/{0}/matches/{1} не корректен", endpoint, time);
                return BadRequest(ModelState);
            }

            ServerInfo server;
            try
            {
                server = db.Servers.Single(rec => rec.Endpoint == endpoint);
            }
            catch (InvalidOperationException)
            {
                logger.Info("Put запрос servers/{0}/matches/{1}. Server с заданным endpoint {0} не найден", endpoint, time);
                return NotFound();
            }
            catch (Exception exception)
            {
                logger.Error(exception, " Exception on Put servers/{0}/matches/{1} request", endpoint, time);
                return InternalServerError();
            }

            GameMode gameMode;
            try
            {
                gameMode = db.GameModes.Single(rec => rec.Name == matchResultRequest.GameMode);
            }
            catch (InvalidOperationException)
            {
                logger.Info("Put запрос servers/{0}/matches/{1}. Не корректен GameMode", endpoint, time);
                return BadRequest();
            }
            catch (Exception exception)
            {
                logger.Error(exception, " Exception on Put servers/{0}/matches/{1} request", endpoint, time);
                return InternalServerError();
            }
            try
            {
                logger.Info("Put запрос servers/{0}/matches/{1} добавляю запись", endpoint, time);

                MatchResult matchResult = new MatchResult();
                matchResult.Server = server;
                matchResult.Timestamp = time;
                matchResult.Map = matchResultRequest.Map;
                matchResult.GameMode = gameMode;
                matchResult.FragLimit = matchResultRequest.FragLimit;
                matchResult.TimeLimit = matchResultRequest.TimeLimit;
                matchResult.TimeElapsed = matchResultRequest.TimeElapsed;

                SaveScoreboard(matchResultRequest.Scoreboard, matchResult);

                db.MathesResults.Add(matchResult);
                db.SaveChanges();
            }
            catch (Exception exception)
            {
                logger.Error(exception, " Exception on Put servers/{0}/matches/{1} request", endpoint,time);
                return InternalServerError();
            }

            return Ok();
        }

        private void SaveScoreboard(List<ScoreboardElement> scoreboard, MatchResult match)
        {
            for (int i = 0; i < scoreboard.Count; i++)
            {

                var scoreboardRow = scoreboard[i];
                ScoreboardRecord record = new ScoreboardRecord();
                record.Kills = scoreboardRow.Kills;
                record.Frags = scoreboardRow.Frags;
                record.Deaths = scoreboardRow.Deaths;
                record.Player = scoreboardRow.Name;
                if (scoreboard.Count == 1)
                    record.ScoreboardPercent = 100;
                else
                    record.ScoreboardPercent = (((double)scoreboard.Count - 1 - i) / (scoreboard.Count - 1)) * 100;
                record.Match = match;
                match.ScoreBoard.Add(record);
            }
        }

        [HttpGet]
        [Route("servers/{endpoint}/matches/{time}")]
        [ResponseType(typeof(MatchResultRequest))]
        public IHttpActionResult GetMatchResult([FromUri]string endpoint, [FromUri]DateTime time)
        {
            time = time.ToUniversalTime();
            if (!ModelState.IsValid)
            {
                logger.Info("Get запрос servers/{0}/matches/{1} не корректен", endpoint, time);
                return BadRequest(ModelState);
            }

            MatchResult match;
            try
            {
                match = db.MathesResults.Single(rec => rec.Server.Endpoint == endpoint && rec.Timestamp == time);
            }
            catch (InvalidOperationException)
            {
                logger.Info("Get запрос servers/{0}/matches/{1}. Match с заданным endpoint {0} и timestamp {1} не найден", endpoint, time);
                return BadRequest();
            }
            catch (Exception exception)
            {
                logger.Error(exception, " Exception on Get servers/{0}/matches/{1} request", endpoint, time);
                return InternalServerError();
            }
            try
            {
                MatchResultRequest response = ExtractMatchResultsRequest(match);
                return Ok(response);
            }
            catch (Exception exception)
            {
                logger.Error(exception, " Exception on Get servers/{0}/matches/{1} request", endpoint, time);
                return InternalServerError();
            }
        }

        private MatchResultRequest ExtractMatchResultsRequest(MatchResult match)
        {
            MatchResultRequest result = new MatchResultRequest();
            result.FragLimit = match.FragLimit;
            result.GameMode = match.GameMode.Name;
            result.Map = match.Map;
            result.TimeElapsed = match.TimeElapsed;
            result.TimeLimit = match.TimeLimit;
            result.Scoreboard = new List<ScoreboardElement>();
            foreach (var scoreboardRecord in match.ScoreBoard)
            {
                ScoreboardElement scoreElement = new ScoreboardElement();
                scoreElement.Name = scoreboardRecord.Player;
                scoreElement.Deaths = scoreboardRecord.Deaths;
                scoreElement.Frags = scoreboardRecord.Frags;
                scoreElement.Kills = scoreboardRecord.Kills;
                result.Scoreboard.Add(scoreElement);
            }
            return result;
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
