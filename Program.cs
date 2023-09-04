using Spectre.Console;
using System;
using FileTransfer;
using System.Timers;
using FileTransfer.Connectors;
using System.IO;
using System.CommandLine;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace FileTransfer
{
    internal class Program
    {
        const int DELAY = 5;
        const int REPLAY = 2;
        const bool RECURSE = false;
        const int LOG_LEVEL = 1;
        const string ACTION = "push";

        static async Task Main(string[] args)
        { 
            var rootCommand = new RootCommand("xft - Extreme File Transfer");

            var delayOption = new Option<int>(
                name: "--delay",
                description: "Delay between replay requests in seconds.",
                getDefaultValue: () => DELAY);
                rootCommand.AddGlobalOption(delayOption);

            var replayOption = new Option<int>(
                name: "--replay",
                description: "Number of replays to attempt.",
                getDefaultValue: () => REPLAY);
                rootCommand.AddGlobalOption(replayOption);

            var recurseOption = new Option<bool>(
                name: "--recursive",
                description: "Recursive directory processing.",
                getDefaultValue: () => RECURSE);
                rootCommand.AddGlobalOption(recurseOption);

            var logLevelOption = new Option<int>(
                name: "--logging",
                description: "Logging level for the process.",
                getDefaultValue: () => LOG_LEVEL);
                rootCommand.AddGlobalOption(logLevelOption);

            var connectionArgument = new Argument<string>
                (name: "connection",
                description: "The connection type to use (file, trim).",
                getDefaultValue: () => "trim"
                );

            var actionArgument = new Argument<string>
                (name: "action",
                description: "The action to apply (push, move).",
                getDefaultValue: () => ACTION);

            var pathArgument= new Argument<string>(
                name: "path",
                description: "The directory to be processed."//,
                //getDefaultValue: () => "c:/temp2"
                );

            var queryArgument = new Argument<string>(
                name: "query",
                description: "Connection query string for the connector used."//,
                //getDefaultValue: () => "WorkgroupServerName/Id/Container/ContainerType/RecordType"
                );

            rootCommand.Add(connectionArgument);
            rootCommand.Add(actionArgument);
            rootCommand.Add(pathArgument);
            rootCommand.Add(queryArgument);

            rootCommand.SetHandler((connectionArg, actionArg, pathArg, queryArg, delayOpt, replayOpt, recurseOpt, logOpt) =>
            {
           
                Configuration configuration = new()
                {
                    Command = connectionArg,
                    Action = actionArg,
                    FromPath = pathArg,
                    ToPath = queryArg,
                    ReplayCount = replayOpt,
                    Delay = delayOpt,
                    LogLevel = logOpt,
                    Interactive = false,
                    Query = "*.*",
                    Recurisve = recurseOpt,
                    Prefix = ".xft_"
                };

                if (Path.HasExtension(pathArg))
                {
                    configuration.Query = "*" + Path.GetExtension(pathArg);
                    configuration.FromPath = Path.GetDirectoryName(pathArg);
                }
          
                ProcessFiles(configuration);
            },
            connectionArgument, actionArgument, pathArgument, queryArgument, delayOption, replayOption, recurseOption, logLevelOption);

            await rootCommand.InvokeAsync(args);
        }

        static public void ProcessFiles(Configuration configuration)
        {
            Logging log = new();
            TaskLocker locker = new(configuration.FromPath);
            //Encoder encoder = new Encoder();
            DirectoryService directoryService = new();
            int replayCounter = 0;

            if (locker.IsLocked())
            {
                log.WriteLine("Process Already Locked - Exiting", configuration.LogLevel);
                return;
            }

            locker.Lock();
            log.WriteLine("Process Locked", configuration.LogLevel);

            Connector connector = null;

            switch (configuration.Command.ToLower())
            {
                case "file":
                    connector = new FileSystemConnector();
                    break;

                case "trim":
                    connector = new TrimConnector();
                    break;

                default:
                    break;
            }

            if (connector == null)
            {
                locker.Unlock();
                return;
            }

            connector.Configuration = configuration;
            connector.log = log;

            FileInfo[] files = directoryService.List(configuration.FromPath, configuration.Query, configuration.Recurisve);

            foreach (FileInfo file in files)
            {
                if (!file.Name.StartsWith(".xft_") & !file.Directory.Name.StartsWith(".xft_"))
                {
                    //Thread.Sleep(1000 * configuration.Delay);

                    switch (configuration.Action.ToLower())
                    {
                        case "push":
                            while (!connector.Push(file, configuration.ToPath) & replayCounter < configuration.ReplayCount)
                            {
                                Thread.Sleep(1000 * configuration.Delay);
                                replayCounter++;
                                log.WriteLine("Replay(" + replayCounter.ToString() + ") " + file.FullName);
                            }
                            if (replayCounter == configuration.ReplayCount)
                            {
                                if (!directoryService.Exists(file.DirectoryName + "/" + ".xft_" + "error_que"))
                                {
                                    directoryService.Create(file.DirectoryName + "/" + ".xft_" + "error_que");
                                }
                                file.MoveTo(file.DirectoryName + "/" + ".xft_" + "error_que/" + file.Name);
                            }
                            replayCounter = 0;
                            break;
                    }
                }

            }

            locker.Unlock();
            log.WriteLine("Process Unlocked", configuration.LogLevel);
            return;
        }
    }
}
