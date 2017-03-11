using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Kontur.GameStats.Server.Models;
using Ninject.Extensions.Logging;
using System.Text.RegularExpressions;
using System;
using Kontur.GameStats.Server.Requests;
using Kontur.GameStats.Server.Responces;

namespace Kontur.GameStats.Server.Controllers
{
    public class ServerInfoesController : ApiController
    {
        private readonly ILogger logger;
        private DatabaseContext db = new DatabaseContext();

        private const string validIpAddress = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])";
        private const string validPortNumber = @"-([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])$";
        private const string validHostname = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])";
        private Regex validIpAddressRegex = new Regex(validIpAddress + validPortNumber);
        private Regex validHostnameRegex = new Regex(validHostname + validPortNumber);

        public ServerInfoesController(ILogger logger)
        {
            this.logger = logger;
        }

        [HttpGet]     
        [Route("servers/info")]
        [ResponseType(typeof(IEnumerable<GeneralServerInformation>))]
        public IHttpActionResult GetServerInfos()
        {
            logger.Info("GET запрос servers/info принят");
            var response = new List<GeneralServerInformation>();

            foreach(var serverInfo in db.Servers)
            {
                var generalInformation = new GeneralServerInformation();
                generalInformation.Endpoint = serverInfo.Endpoint;
                generalInformation.Info = ExtractServerInfo(serverInfo);
                response.Add(generalInformation);
            }

            return Ok(response);
        }

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

            ServerInfo serverInfo = db.Servers.Find(endpoint);
            if (serverInfo == null)
            {
                return NotFound();
            }

            AdvertiseRequest response = ExtractServerInfo(serverInfo);
            return Ok(response);
        }

        private static AdvertiseRequest ExtractServerInfo(ServerInfo serverInfo)
        {
            AdvertiseRequest response = new AdvertiseRequest();
            response.Name = serverInfo.Name;
            foreach (var gameMode in serverInfo.GameModes)
                response.GameModes.Add(gameMode.Name);
            return response;
        }

        [HttpPut]
        [Route("servers/{endpoint}/info")]
        [ResponseType(typeof(void))]
        public IHttpActionResult SaveServerInfo(string endpoint, AdvertiseRequest advertiseRequest)
        {
            if (!ModelState.IsValid)
            {
                logger.Info("Put запрос servers/{0}/info не корректен", endpoint);
                return BadRequest(ModelState);
            }

            if (!validHostnameRegex.IsMatch(endpoint) && !validHostnameRegex.IsMatch(endpoint))
            {
                logger.Info("Put запрос servers/{0}/info не корректен", endpoint);
                return BadRequest();
            }

            if (!ServerInfoExists(endpoint))
            {
                logger.Info("Put запрос servers/{0}/info добавляю запись", endpoint);

                ServerInfo serverInfo = new ServerInfo();
                serverInfo.Endpoint = endpoint;
                serverInfo.Name = advertiseRequest.Name;
                AddOrChangeGameModes(advertiseRequest, serverInfo);

                db.Servers.Add(serverInfo);
            }
            else
            {
                logger.Info("Put запрос servers/{0}/info обновляю запись ", endpoint);

                var serverInfo = db.Servers.Single(row => row.Endpoint == endpoint);
                serverInfo.Name = advertiseRequest.Name;
                AddOrChangeGameModes(advertiseRequest, serverInfo);
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                if (!ServerInfoExists(endpoint))
                {
                    return NotFound();
                }
                else
                {
                    logger.ErrorException("Put запрос servers/{0}/info", exception);
                    return InternalServerError();
                }
            }

            return Ok();
        }

        private void AddOrChangeGameModes(AdvertiseRequest advertiseRequest, ServerInfo serverInfo)
        {
            serverInfo.GameModes.Clear();
            foreach (var gameModeName in advertiseRequest.GameModes)
            {
                GameMode mode;
                try { mode = db.GameModes.First(gmode => gmode.Name == gameModeName.ToUpper()); }
                catch (InvalidOperationException)
                {
                    mode = new GameMode();
                    mode.Name = gameModeName.ToUpper();
                    db.GameModes.Add(mode);
                    db.SaveChanges();
                }
                serverInfo.GameModes.Add(mode);
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

        private bool ServerInfoExists(string id)
        {
            return db.Servers.Any(e => e.Endpoint == id);
        }
    }
}