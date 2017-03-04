using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Kontur.GameStats.Server.Models;
using Ninject.Extensions.Logging;
using System.Text.RegularExpressions;
using System;

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

        [ResponseType(typeof(IEnumerable<ServerInfo>))]
        [Route("servers/info")]
        [HttpGet]
        public IHttpActionResult GetServerInfos()
        {
            logger.Info("GET запрос servers/info принят");
            return Ok(db.ServerInfos);
        }

        [ResponseType(typeof(ServerInfo))]
        [Route("servers/{endpoint}/info")]
        [HttpGet]
        public IHttpActionResult GetServerInfo(string endpoint)
        {
            
            try
            {
                if (!validIpAddressRegex.IsMatch(endpoint) && !validHostnameRegex.IsMatch(endpoint))
                {
                    logger.Info("GET запрос servers/{0}/info не корректен",endpoint);
                    return BadRequest();
                }
            }
            catch (ArgumentNullException)
            {
                logger.Info("GET запрос servers/{0}/info не корректен", endpoint);
                return BadRequest();
            }

            ServerInfo serverInfo = db.ServerInfos.Find(endpoint);
            if (serverInfo == null)
            {
                return NotFound();
            }

            return Ok(serverInfo);
        }

        [ResponseType(typeof(void))]
        [Route("servers/{endpoint}/info")]
        [HttpPut]
        public IHttpActionResult SaveServerInfo(string endpoint, ServerInfo advertiseRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!validHostnameRegex.IsMatch(endpoint) && !validHostnameRegex.IsMatch(endpoint))
            {
                return BadRequest();
            }

            if (!ServerInfoExists(endpoint))
            {
                ServerInfo serverInfo = new ServerInfo() { Id = endpoint, Name = advertiseRequest.Name };
                /* foreach (string value in advertiseRequest.GameModes)
                 {
                     var findedValue = db.GameModes.Find();
                 }*/
                db.ServerInfos.Add(serverInfo);
            }
            else
            {
                var serverInfo = db.ServerInfos.Single(row => row.Id == endpoint);
                serverInfo.Name = advertiseRequest.Name;
                serverInfo.GameModes = advertiseRequest.GameModes;
                db.Entry(serverInfo).State = EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServerInfoExists(endpoint))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
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
            return db.ServerInfos.Count(e => e.Id == id) > 0;
        }
    }
}