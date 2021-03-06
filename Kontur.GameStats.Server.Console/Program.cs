﻿using Fclp;
using Kontur.GameStats.Server.Models;
using Microsoft.Owin.Hosting;
using NLog;
using System;
using System.Linq;

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
            try
            {
                using (DatabaseContext context = new DatabaseContext())
                using (WebApp.Start<Startup>(options.Prefix))
                {
                    context.MathesResults.FirstOrDefault();

                    logger.Info("Сервер запустился с префиксом {0}", options.Prefix);
                    logger.Info("Нажмите любую кнопку для завершения");
                    Console.ReadKey();
                }
            }
            catch(Exception exception)
            {
                logger.Error(exception, "Internal exception");
            }
        }

        private class Options
        {
            public string Prefix { get; set; }
        }
    }
}
