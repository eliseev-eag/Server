using Kontur.GameStats.Server.Controllers;
using Kontur.GameStats.Server.Models;
using Kontur.GameStats.Server.Requests;
using Kontur.GameStats.Server.Responces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;

namespace Kontur.GameStats.Server.Test
{
    [TestClass]
    public class UnitTest1
    {
        private const string endpoint = "10.0.10.0-8080";
        private const string name = "] My Perf3ct Serv3r [";
        private readonly List<string> gameModes = new List<string>() { "DM", "TDM" };
        private ServerInfoesController controller;

        [TestInitialize()]
        public void InitializeDb()
        {
            var logger = new Mock<ILogger>();
            logger.Setup(x => x.Error(It.IsAny<string>()));
            controller = new ServerInfoesController(logger.Object);
            Database.SetInitializer(new DropCreateDatabaseAlways<DatabaseContext>());

            using(DatabaseContext db = new DatabaseContext())
            {
                var server = new ServerInfo() { Endpoint = endpoint, Name = name };

                foreach (var gameModeName in gameModes)
                {
                    var gameMode = new GameMode() { Name = gameModeName };
                    db.GameModes.Add(gameMode);
                    server.GameModes.Add(gameMode);
                }

                db.Servers.Add(server);
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void PutCorrectServerInfoTest()
        {
            GeneralServerInformation expected = new GeneralServerInformation() { Endpoint = endpoint };
            expected.Info = new AdvertiseRequest() { Name = name, GameModes = new List<string>() };
            foreach (var gameModeName in gameModes)
                expected.Info.GameModes.Add(gameModeName);

            IHttpActionResult actionResult = controller.GetServerInfos();
            var contentResult = actionResult as OkNegotiatedContentResult<ICollection<GeneralServerInformation>>;

            var actualResponse = contentResult.Content.FirstOrDefault();
            Assert.AreNotEqual(null, contentResult);
            Assert.AreEqual(1, contentResult.Content.Count);
            Assert.AreEqual(expected, actualResponse);

        }
    }
}
