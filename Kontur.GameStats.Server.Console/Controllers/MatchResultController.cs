﻿using Kontur.GameStats.Server.Models;
using Kontur.GameStats.Server.Requests;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
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
        [Route("servers/{endpoint}/matches/{time}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> SaveServerInfo([FromUri]string endpoint, [FromUri]DateTime time, [FromBody]MatchResultRequest matchResultRequest)
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
                server = await db.Servers.SingleAsync(rec => rec.Endpoint == endpoint);
            }
            catch (InvalidOperationException)
            {
                logger.Info("Put запрос servers/{0}/matches/{1}. Server с заданным endpoint {0} не найден", endpoint, time);
                return NotFound();
            }

            GameMode gameMode;
            try
            {
                gameMode = await db.GameModes.SingleAsync(rec => rec.Name == matchResultRequest.GameMode);
            }
            catch (InvalidOperationException)
            {
                logger.Info("Put запрос servers/{0}/matches/{1}. Не корректен GameMode", endpoint, time);
                return BadRequest();
            }
            Map map;
            try
            {
                map = await db.Maps.SingleAsync(rec => rec.Name == matchResultRequest.Map);
            }
            catch (InvalidOperationException)
            {
                map = new Map();
                map.Name = matchResultRequest.Map;
                db.Maps.Add(map);
                try { await db.SaveChangesAsync(); }
                catch (DbUpdateException)
                {
                    map = await db.Maps.FirstAsync(m => m.Name == matchResultRequest.Map);
                }
            }

            try
            {
                logger.Info("Put запрос servers/{0}/matches/{1} добавляю запись", endpoint, time);
                MatchResult matchResult = new MatchResult();
                matchResult.Server = server;
                matchResult.Timestamp = time;
                matchResult.Map = map;
                matchResult.GameMode = gameMode;
                matchResult.FragLimit = matchResultRequest.FragLimit;
                matchResult.TimeLimit = matchResultRequest.TimeLimit;
                matchResult.TimeElapsed = matchResultRequest.TimeElapsed;
                await SaveScoreboard(matchResultRequest.Scoreboard, matchResult);
                db.MathesResults.Add(matchResult);
                await db.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                logger.Error(exception, " Exception в Put запросе servers/{0}/info", endpoint);
                return InternalServerError();
            }
            return Ok();
        }

        private async Task SaveScoreboard(List<ScoreboardElement> scoreboard, MatchResult match)
        {
            for (int i = 0; i < scoreboard.Count; i++)
            {

                var scoreboardRow = scoreboard[i];
                ScoreboardRecord record = new ScoreboardRecord();
                Player player;
                try { player = await db.Players.SingleAsync(p => p.Name == scoreboardRow.Name); }
                catch (InvalidOperationException)
                {
                    player = new Player() { Name = scoreboardRow.Name };
                    player.Scores.Add(record);
                    db.Players.Add(player);
                }

                record.Kills = scoreboardRow.Kills;
                record.Frags = scoreboardRow.Frags;
                record.Deaths = scoreboardRow.Deaths;
                record.Player = player;
                player.Scores.Add(record);
                if (i == 0)
                    record.ScoreboardPercent = 100;
                else if (i == scoreboard.Count - 1)
                    record.ScoreboardPercent = 0;
                else
                    record.ScoreboardPercent = i / (scoreboard.Count - 1);
                record.Match = match;
                match.ScoreBoard.Add(record);
            }
        }

        [HttpGet]
        [Route("servers/{endpoint}/matches/{time}")]
        [ResponseType(typeof(MatchResultRequest))]
        public async Task<IHttpActionResult> GetServerInfo([FromUri]string endpoint, [FromUri]DateTime time)
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
                match = await db.MathesResults.SingleAsync(rec => rec.Server.Endpoint == endpoint && rec.Timestamp == time);
            }
            catch (InvalidOperationException)
            {
                logger.Info("Get запрос servers/{0}/matches/{1}. Match с заданным endpoint {0} и timestamp {1} не найден", endpoint, time);
                return BadRequest();
            }
            MatchResultRequest response = ExtractMatchResultsRequest(match);
            return Ok(response);
        }

        private MatchResultRequest ExtractMatchResultsRequest(MatchResult match)
        {
            MatchResultRequest result = new MatchResultRequest();
            result.FragLimit = match.FragLimit;
            result.GameMode = match.GameMode.Name;
            result.Map = match.Map.Name;
            result.TimeElapsed = match.TimeElapsed;
            result.TimeLimit = match.TimeLimit;
            result.Scoreboard = new List<ScoreboardElement>();
            foreach (var scoreboardRecord in match.ScoreBoard)
            {
                ScoreboardElement scoreElement = new ScoreboardElement();
                scoreElement.Name = scoreboardRecord.Player.Name;
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
