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
        private static ServerInfoesController controller;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<DatabaseContext>());
        }

        [ClassInitialize]
        public static void ControllerInitialize(TestContext testContext)
        {
            var logger = new Mock<ILogger>();
            logger.Setup(x => x.Error(It.IsAny<string>()));
            controller = new ServerInfoesController(logger.Object);
        }

        [TestInitialize()]
        public void DropCreateDatabase()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                db.Database.Initialize(true);
            }
        }


        [TestMethod]
        public void PutCorrectServerInfo()
        {
            AdvertiseRequest advertiseRequest = new AdvertiseRequest();
            advertiseRequest.Name = name;
            advertiseRequest.GameModes = gameModes;

            IHttpActionResult actionResult = controller.SaveServerInfo(endpoint, advertiseRequest);
            Assert.IsInstanceOfType(actionResult, typeof(OkResult));
            ServerInfo serverInfo;
            List<string> gameModesNames;
            using (DatabaseContext dbContext = new DatabaseContext())
            {
                serverInfo = dbContext.Servers.Single(p => p.Endpoint == endpoint);
                gameModesNames = serverInfo.GameModes.Select(x => x.Name).ToList();
            }

            Assert.AreEqual(endpoint, serverInfo.Endpoint);
            Assert.AreEqual(name, serverInfo.Name);
            CollectionAssert.AreEqual(gameModes, gameModesNames);
        }

        [TestMethod]
        public void GetServerInfoesTest()
        {
            InitialiseDbCorrectServerData();
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

        private void InitialiseDbCorrectServerData()
        {
            using (DatabaseContext db = new DatabaseContext())
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
        public void PutIncorrectServerInfo()
        {
            string endpointWithoutPort = "localhost";
            string endpointWithIncorrectFormat = "my.site.ru - 8080";
            AdvertiseRequest advertiseRequest = new AdvertiseRequest();
            advertiseRequest.Name = name;
            advertiseRequest.GameModes = gameModes;

            IHttpActionResult endpointWithoutPortResult =
                controller.SaveServerInfo(endpointWithoutPort, advertiseRequest);
            IHttpActionResult endpointWithIncorrectFormatResult =
               controller.SaveServerInfo(endpointWithIncorrectFormat, advertiseRequest);
            Assert.IsInstanceOfType(endpointWithoutPortResult, typeof(BadRequestResult));
            Assert.IsInstanceOfType(endpointWithIncorrectFormatResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public void GetServerInfo()
        {
            InitialiseDbCorrectServerData();
            AdvertiseRequest expected = new AdvertiseRequest() { Name = name, GameModes = new List<string>() };
            foreach (var gameModeName in gameModes)
                expected.GameModes.Add(gameModeName);

            IHttpActionResult actionResult = controller.GetServerInfo(endpoint);
            var contentResult = actionResult as OkNegotiatedContentResult<AdvertiseRequest>;

            var actualResponse = contentResult.Content;
            Assert.AreNotEqual(null, contentResult);
            Assert.AreEqual(expected, actualResponse);
        }

    }
}
