using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Kontur.GameStats.Server.Models;
using Ninject.Extensions.Logging;

namespace Kontur.GameStats.Server.Controllers
{
    public class ServerInfoesController : ApiController
    {
        public ServerInfoesController(ILogger logger)
        {
            this.logger = logger;
        }

        private readonly ILogger logger;
        private DatabaseContext db = new DatabaseContext();

        // GET: api/ServerInfoes
        [ResponseType(typeof(IEnumerable<ServerInfo>))]
        [Route("servers/info")]
        [HttpGet]
        public IHttpActionResult GetServerInfos()
        {
            logger.Info("GET запрос servers/info принят");
            return Ok(db.ServerInfos);
        }

        // GET: api/ServerInfoes/5
        [ResponseType(typeof(ServerInfo))]
        [Route("servers/{endpoint}/info")]
        [HttpGet]
        public IHttpActionResult GetServerInfo(string endpoint)
        {
            ServerInfo serverInfo = db.ServerInfos.Find(endpoint);
            if (serverInfo == null)
            {
                return NotFound();
            }

            return Ok(serverInfo);
        }

        // PUT: api/ServerInfoes/5
        [ResponseType(typeof(void))]
        [Route("servers/{endpoint}/info")]
        [HttpPut]
        public IHttpActionResult SaveServerInfo(string endPoint, ServerInfo advertiseRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!ServerInfoExists(endPoint))
            {
                ServerInfo serverInfo = new ServerInfo() { Id = endPoint, Name = advertiseRequest.Name };
                /* foreach (string value in advertiseRequest.GameModes)
                 {
                     var findedValue = db.GameModes.Find();
                 }*/
                db.ServerInfos.Add(serverInfo);
            }
            else
            {
                var serverInfo = db.ServerInfos.Single(row => row.Id == endPoint);
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
                if (!ServerInfoExists(endPoint))
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