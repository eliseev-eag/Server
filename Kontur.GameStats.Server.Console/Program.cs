using Fclp;
using Microsoft.Owin.Hosting;
using NLog;
using System;

namespace Kontur.GameStats.Server
{
    internal class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Info("Сервер запускается");
            var commandLineParser = new FluentCommandLineParser<Options>();

            commandLineParser
                .Setup(options => options.Prefix)
                .As("prefix")
                .SetDefault("http://+:8080/")
                .WithDescription("HTTP prefix to listen on");

            commandLineParser
                .SetupHelp("h", "help")
                .WithHeader($"{AppDomain.CurrentDomain.FriendlyName} [--prefix <prefix>]")
                .Callback(text => Console.WriteLine(text));

            if (commandLineParser.Parse(args).HelpCalled)
                return;

            RunServer(commandLineParser.Object);
        }

        private static void RunServer(Options options)
        {
            //using (WebApp.Start<Startup>(options.Prefix))
            using (WebApp.Start<Startup>("http://localhost:12345/"))
            {

                // Keep the server going until we're done
                logger.Info("Сервер запустился с префиксом {0}", options.Prefix);
                Console.ReadKey();
            }
        }

        private class Options
        {
            public string Prefix { get; set; }
        }
    }
}
